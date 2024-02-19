using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;
using Elsa.Workflows.Runtime.Activities;
using JetBrains.Annotations;

namespace ElsaServer.Workflows;

[UsedImplicitly]
public class ParentWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Name = "Parent Workflow";
        builder.Root = new Sequence
        {
            Activities =
            {
                new WriteLine("Parent started"),
                new DispatchWorkflow
                {
                    WorkflowDefinitionId = new("ChildWorkflow"),
                    WaitForCompletion = new(true),
                    Input = new(_ => new Dictionary<string, object>
                    {
                        ["Message"] = "Hello from Parent"
                    })
                },
                new WriteLine("Parent completed")
            }
        };
    }
}