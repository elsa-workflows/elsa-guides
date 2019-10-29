using Elsa.Activities.Email.Extensions;
using Elsa.Activities.Http.Extensions;
using Elsa.Activities.Timers.Extensions;
using Elsa.Extensions;
using Elsa.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Guides.DocumentApproval.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddElsa()
                .AddHttpActivities(options => options.Bind(Configuration.GetSection("Http")))
                .AddEmailActivities(options => options.Bind(Configuration.GetSection("Smtp")))
                .AddTimerActivities(options => options.Bind(Configuration.GetSection("BackgroundRunner")))
                .AddWorkflow<DocumentApprovalWorkflow>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpActivities();
        }
    }
}