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
using NetCoreSample.Service.Common.AwsDynamoDB;
using W4k.AspNetCore.Correlator.Extensions;
using W4k.AspNetCore.Correlator;

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

            // Add Correlation ID
            services.AddCorrelator();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddMvcOptions(options =>
                {
                    options.Filters.Add(typeof(ActionLoggingContextFilter));

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
            // Register Correlation ID hanlding
            app.UseCorrelator();

            // Register custom middlewars
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

        private void ConfigureDependencyInjections(IServiceCollection services)
        {
            // AWS Service Clients
            services.AddAWSService<IAmazonS3>();
            services.AddAWSService<IAmazonDynamoDB>();

            // This is the custom client wrapper on top of DynamoDB API
            // This is needed to add a layer of abstraction to facilitate better testability
            services.AddSingleton<IDynamoDBClient, DynamoDBClient>();

            // Sample Developer Usage
            // My Products
            // Also shown with example configuration injection
            services.Configure<Configurations.DeveloperSample.ServiceDependenciesConfig>(
                Configuration.GetSection("ServiceDependencies"));

            // Inject HttpClient, via HttpClientFactory, and also attach the correlation forwarding behavior
            services.AddHttpClient<IMyProductRepository, MyProductRepository>(config =>
            {
                // HTTP client configuration
            })
            .AddHttpMessageHandler<CorrelatorHttpMessageHandler>();
        }

        private string GetXmlCommentsPath()
        {
            return Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
        }
    }
}
