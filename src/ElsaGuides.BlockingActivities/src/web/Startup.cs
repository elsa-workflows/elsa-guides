using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using Elsa.Server.Web.HostedServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyActivityLibrary.Activities;
using MyActivityLibrary.Bookmarks;
using MyActivityLibrary.JavaScript;
using MyActivityLibrary.Liquid;
using MyActivityLibrary.Services;

namespace Elsa.Server.Web
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        private IWebHostEnvironment Environment { get; }
        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Elsa services.
            services
                .AddElsa(elsa => elsa
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlite())
                    .AddConsoleActivities()
                    .AddEmailActivities(options => Configuration.GetSection("Elsa:Smtp").Bind(options))
                    .AddActivity<FileReceived>()
                );

            services.AddBookmarkProvider<FileReceivedBookmarkProvider>();
            services.AddScoped<IFileReceivedInvoker, FileReceivedInvoker>();
            services.AddNotificationHandlersFrom<LiquidHandler>();
            services.AddJavaScriptTypeDefinitionProvider<MyTypeDefinitionProvider>();
            services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
            services.AddHostedService<FileMonitorService>();

            // Elsa API endpoints.
            services.AddElsaApiEndpoints();

            // Allow arbitrary client browser apps to access the API.
            // In a production environment, make sure to allow only origins you trust.
            services.AddCors(cors => cors.AddDefaultPolicy(policy => policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .WithExposedHeaders("Content-Disposition"))
            );
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app
                .UseCors()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
                    endpoints.MapControllers();
                })
                .UseWelcomePage();
        }
    }
}