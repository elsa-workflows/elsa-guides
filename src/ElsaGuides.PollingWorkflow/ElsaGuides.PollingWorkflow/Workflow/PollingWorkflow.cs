using Elsa.Activities.ControlFlow;
using Elsa.Builders;

namespace ElsaGuides.PollingWorkflow.Workflow
{
    public class PollingWorkflow : IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {
            builder
                .Then<Fork>(fork => fork.WithBranches("abort", "poll"), fork =>
                {
                    fork
                        .When("abort")
                        .Then<AbortActivity>()
                        .ThenNamed("join");

                    fork
                        .When("poll")
                        .Then<PollingActivity>()
                        .ThenNamed("join");

                })
                .Then<Join>(join => join.WithMode(Join.JoinMode.WaitAny)).WithName("join");
        }
    }
}