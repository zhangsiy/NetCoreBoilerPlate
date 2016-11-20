using System.IO;
using MyWebService.Data.ElasticSearch;
using MyWebService.Filters;
using MyWebService.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using NLog.Extensions.Logging;
using Swashbuckle.Swagger.Model;

namespace MyWebService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc( options =>
            {
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

                // Register global exception handling
                options.Filters.Add(typeof (ExceptionLoggingFilter));
            });

            // Swagger
            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                // [TO_FILL application information for swagger]
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "My Test API",
                    Description = "The Web API for testing the awesome Asp.Net Core boiler plate",
                    TermsOfService = "None"
                });
                options.IncludeXmlComments(GetXmlCommentsPath(PlatformServices.Default.Application));
                options.DescribeAllEnumsAsStrings();
            });

            // Inject Elastic Search Repo
            services.Configure<EsRepoConfiguration>(Configuration.GetSection("AwsEsConfiguration"));
            services.AddScoped<IEsRepository, EsRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Register custom middlewars
            app.UseCorrelationId();
            app.UseRequestLogger();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUi();

            // Add NLog
            loggerFactory.AddNLog();
            env.ConfigureNLog("nlog.config");
        }

        private string GetXmlCommentsPath(ApplicationEnvironment appEnvironment)
        {
            return Path.Combine(appEnvironment.ApplicationBasePath, $"{appEnvironment.ApplicationName}.xml");
        }
    }
}
