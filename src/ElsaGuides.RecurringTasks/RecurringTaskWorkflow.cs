using Elsa.Activities.Console;
using Elsa.Activities.Temporal;
using Elsa.Builders;
using NodaTime;

namespace ElsaGuides.RecurringTasks
{
    public class RecurringTaskWorkflow : IWorkflow
    {
        private readonly IClock _clock;

        public RecurringTaskWorkflow(IClock clock) => _clock = clock;

        public void Build(IWorkflowBuilder builder) =>
            builder
                .Timer(Duration.FromSeconds(5))
                .WriteLine(() => $"It's now {_clock.GetCurrentInstant()}. Let's do this thing!");
    }
}