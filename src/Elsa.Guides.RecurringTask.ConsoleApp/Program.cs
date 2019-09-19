﻿using System.Threading.Tasks;
using Elsa.Activities.Console.Extensions;
using Elsa.Activities.Timers.Extensions;
using Elsa.Extensions;
using Elsa.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Elsa.Guides.RecurringTask.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(logging => logging.AddConsole())
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                var services = host.Services;
                var registry = services.GetService<IWorkflowRegistry>();
                
                registry.RegisterWorkflow<RecurringTaskWorkflow>();
                
                await host.StartAsync();
                await host.WaitForShutdownAsync();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddWorkflows()
                .AddConsoleActivities()
                .AddTimerActivities(options => options.Configure(x => x.SweepInterval = Period.FromSeconds(1)));
        }
    }
}