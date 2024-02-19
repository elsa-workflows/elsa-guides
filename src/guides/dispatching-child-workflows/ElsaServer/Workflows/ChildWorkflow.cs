using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;
using Elsa.Workflows.Models;
using JetBrains.Annotations;

namespace ElsaServer.Workflows;

[UsedImplicitly]
public class ChildWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var messageInput = new InputDefinition
        {
            Name = "Message",
            DisplayName = "Message",
            Description = "The message to write to the console.",
            Type = typeof(string)
        };
        
        builder.Name = "Child Workflow";
        builder.Inputs.Add(messageInput);
        builder.Root = new WriteLine(context => context.GetInput<string>("Message"));
    }
}