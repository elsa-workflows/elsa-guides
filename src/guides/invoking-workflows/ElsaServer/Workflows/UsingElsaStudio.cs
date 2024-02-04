using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;
using JetBrains.Annotations;

namespace ElsaServer.Workflows;

[UsedImplicitly]
public class UsingElsaStudio : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Root = new WriteLine("Hello from Elsa Studio!");
    }
}