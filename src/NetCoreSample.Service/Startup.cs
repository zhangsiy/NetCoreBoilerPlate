using Amazon.DynamoDBv2;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using NetCoreSample.Service.ActionFilters;
using NetCoreSample.Service.Data.DeveloperSample;
using NetCoreSample.Service.Middlewares;
using Newtonsoft.Json.Converters;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Net.Http;

namespace NetCoreSample.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Setup logging
            // This uses Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure Serilog as the logging service
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            services.AddMvc(options =>
            {
                // Register action filter to automatic return 400 when model
                // validation fails (this is applied to all actions in all
                // controllers)
                options.Filters.Add(typeof(ValidateModelStateAttribute));

                // Default profile to cache things for 1 hour
                options.CacheProfiles.Add("Default",
                    new CacheProfile()
                    {
                        Duration = 3600,
                        Location = ResponseCacheLocation.Any
                    });

                // Use this profile to get "no-cache" behavior
                options.CacheProfiles.Add("Never",
                    new CacheProfile()
                    {
                        Location = ResponseCacheLocation.None,
                        NoStore = true
                    });
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter
                {
                    CamelCaseText = true
                });
            });

            // Configure CORS Policies
            services.AddCors(options =>
            {
                // Visualizer End point CORS Policy
                options.AddPolicy("ReadOnlyDefault",
                    builder => builder
                        .WithMethods("GET", "HEAD", "OPTIONS")
                        .AllowAnyHeader()
                        .AllowAnyOrigin());
            });

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.FullName);
                c.SwaggerDoc("v1", new Info { Title = "NetCoreSample API", Version = "v1" });
                c.IncludeXmlComments(GetXmlCommentsPath(PlatformServices.Default.Application));
            });

            // Configure the dependency injection structure
            ConfigureDependencyInjections(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Register custom middlewars
            app.UseCorrelationId();
            app.UseRequestLogger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCoreSample API V1");
            });

            app.UseMvc();
        }

        private static void ConfigureDependencyInjections(IServiceCollection services)
        {
            // HttpClient as Singleton
            services.AddSingleton<HttpClient>();

            // AWS Service Clients
            services.AddAWSService<IAmazonS3>();
            services.AddAWSService<IAmazonDynamoDB>();

            // Sample Developer Usage
            // My Products
            services.AddScoped<IMyProductRepository, MyProductRepository>();
        }

        private string GetXmlCommentsPath(ApplicationEnvironment appEnvironment)
        {
            return Path.Combine(appEnvironment.ApplicationBasePath, $"{appEnvironment.ApplicationName}.xml");
        }
    }
}
