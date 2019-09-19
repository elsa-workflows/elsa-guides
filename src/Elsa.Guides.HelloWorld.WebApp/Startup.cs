using Elsa.Activities.Http.Extensions;
using Elsa.Extensions;
using Elsa.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Guides.HelloWorld.WebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddWorkflows()
                .AddHttpActivities();
        }

        public void Configure(IApplicationBuilder app, IWorkflowRegistry workflowRegistry)
        {
            app.UseHttpActivities();
            workflowRegistry.RegisterWorkflow<HelloWorldHttpWorkflow>();
        }
    }
}