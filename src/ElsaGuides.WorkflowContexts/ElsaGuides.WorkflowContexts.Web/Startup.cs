using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using Elsa.Runtime;
using ElsaGuides.WorkflowContexts.Web.Data;
using ElsaGuides.WorkflowContexts.Web.Data.StartupTasks;
using ElsaGuides.WorkflowContexts.Web.Providers.WorkflowContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ElsaGuides.WorkflowContexts.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Sqlite");

            services
                .AddDbContextFactory<BlogContext>(options => options.UseSqlite(connectionString, sql => sql.MigrationsAssembly(typeof(Startup).Assembly.FullName)))
                .AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()))
                .AddElsa(elsa => elsa
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlite(connectionString))
                    .AddConsoleActivities()
                    .AddJavaScriptActivities()
                    .AddHttpActivities(options => options.BasePath = "/workflows"))
                .AddWorkflowContextProvider<BlogPostWorkflowContextProvider>()
                .AddStartupTask<RunBlogMigrations>()
                .AddElsaApiEndpoints();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseCors();
            app.UseHttpActivities();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseWelcomePage();
        }
    }
}