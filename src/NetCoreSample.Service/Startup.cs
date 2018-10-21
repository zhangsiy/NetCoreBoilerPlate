using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NetCoreSample.Service.ActionFilters;
using NetCoreSample.Service.Data.DeveloperSample;
using NetCoreSample.Service.Middlewares;
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

            // Setup Json serialization preferences globally
            JsonConvert.DefaultSettings =
                () => new JsonSerializerSettings
                {
                    Converters = {
                        new StringEnumConverter { CamelCaseText = true }
                    },
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure Serilog as the logging service
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddMvcOptions(options =>
                {
                    // Register action filter to automatic return 400 when model
                    // validation fails (this is applied to all actions in all
                    // controllers)
                    options.Filters.Add(typeof(ValidateModelStateAttribute));

                    // Default profile to cache things for 1 hour
                    options.CacheProfiles.Add("PublicOneHour",
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

                // Try include documentation file, if found
                string docFile = GetXmlCommentsPath();
                if (File.Exists(docFile))
                {
                    c.IncludeXmlComments(docFile);
                }

                // Enable annotations, such as tags
                c.EnableAnnotations();
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

            if (env.IsEnvironment("Local"))
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

        private string GetXmlCommentsPath()
        {
            return Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
        }
    }
}
