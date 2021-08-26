using System.IO;
using DocumentManagement.Core.Extensions;
using DocumentManagement.Core.Options;
using DocumentManagement.Persistence.Extensions;
using DocumentManagement.Workflows.Extensions;
using Elsa;
using Elsa.Activities.Email.Options;
using Elsa.Activities.Http.Options;
using Elsa.Persistence.EntityFramework.Sqlite;
using Hangfire;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Storage.Net;

namespace DocumentManagement.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbConnectionString = Configuration.GetConnectionString("Sqlite");

            // Razor Pages (for UI).
            services.AddRazorPages();

            // Hangfire (for background tasks).
            AddHangfire(services, dbConnectionString);

            // Elsa (workflows engine).
            AddWorkflowServices(services, dbConnectionString);

            // Domain services.
            AddDomainServices(services);
            
            // Persistence.
            AddPersistenceServices(services, dbConnectionString);

            // Allow arbitrary client browser apps to access the API for demo purposes only.
            // In a production environment, make sure to allow only origins you trust.
            services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("Content-Disposition")));

            // Register all Mediatr event handlers from this assembly.
            services.AddNotificationHandlersFrom<Startup>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app
                .UseStaticFiles()
                .UseCors()
                .UseRouting()
                .UseHttpActivities() // Install middleware for triggering HTTP Endpoint activities. 
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();
                    endpoints.MapControllers(); // Elsa API Endpoints are implemented as ASP.NET API controllers.
                });
        }

        private void AddHangfire(IServiceCollection services, string dbConnectionString)
        {
            services
                .AddHangfire(config => config
                    // Use same SQLite database as Elsa for storing jobs. 
                    .UseSQLiteStorage(dbConnectionString)
                    .UseSimpleAssemblyNameTypeSerializer()

                    // Elsa uses NodaTime primitives, so Hangfire needs to be able to serialize them.
                    .UseRecommendedSerializerSettings(settings => settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)))
                .AddHangfireServer((sp, options) =>
                {
                    // Bind settings from configuration.
                    Configuration.GetSection("Hangfire").Bind(options);
                });
        }

        private void AddWorkflowServices(IServiceCollection services, string dbConnectionString)
        {
            services.AddWorkflowServices(Environment, dbContext => dbContext.UseSqlite(dbConnectionString));

            // Configure SMTP.
            services.Configure<SmtpOptions>(options => Configuration.GetSection("Elsa:Smtp").Bind(options));
            
            // Configure HTTP activities.
            services.Configure<HttpActivityOptions>(options => Configuration.GetSection("Elsa:Server").Bind(options));
            
            // Elsa API (to allow Elsa Dashboard to connect for checking workflow instances).
            services.AddElsaApiEndpoints();
        }

        private void AddDomainServices(IServiceCollection services)
        {
            services.AddDomainServices();
            
            // Configure Storage for DocumentStorage.
            services.Configure<DocumentStorageOptions>(options => options.BlobStorageFactory = () => StorageFactory.Blobs.DirectoryFiles(Path.Combine(Environment.ContentRootPath, "App_Data/Uploads")));
        }
        
        private void AddPersistenceServices(IServiceCollection services, string dbConnectionString)
        {
            services.AddDomainPersistence(dbConnectionString);
        }
    }
}