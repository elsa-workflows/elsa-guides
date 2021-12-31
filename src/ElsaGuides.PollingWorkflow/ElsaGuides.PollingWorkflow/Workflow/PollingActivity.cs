using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Elsa.Activities.Temporal.Common.ActivityResults;
using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using NodaTime;

namespace ElsaGuides.PollingWorkflow.Workflow
{
    public class PollingActivity : Activity
    {
        private readonly IClock _clock;
        private readonly HttpClient _httpClient;

        public PollingActivity(IClock clock, HttpClient httpClient)
        {
            _clock = clock;
            _httpClient = httpClient;
        }

        private async ValueTask<IActivityExecutionResult> Execute(ActivityExecutionContext context)
        {
            var result = await _httpClient.GetAsync("http://www.randomnumberapi.com/api/v1.0/random?min=0&max=10&count=1");
            var content = await result.Content.ReadAsStringAsync();
            var number = JsonSerializer.Deserialize<int[]>(content)?.FirstOrDefault() ?? 0;

            Console.WriteLine($"Polling workflow with correlation id: {context.CorrelationId} number: {number}");

            if (number == 5)
            {
                return Done();
            }
            
            return Combine(Suspend(), new ScheduleWorkflowResult(_clock.GetCurrentInstant().Plus(Duration.FromSeconds(2))));
        }
        
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
            => await Execute(context);

        protected override async ValueTask<IActivityExecutionResult> OnResumeAsync(ActivityExecutionContext context)
            => await Execute(context);
    }
}