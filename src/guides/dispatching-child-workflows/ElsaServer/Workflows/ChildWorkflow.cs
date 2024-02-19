using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;
using Elsa.Workflows.Management.Activities.SetOutput;
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
        
        var messageOutput = new OutputDefinition
        {
            Name = "Response",
            DisplayName = "Response",
            Description = "The message to provide as output.",
            Type = typeof(string)
        };
        
        builder.Name = "Child Workflow";
        builder.Inputs.Add(messageInput);
        builder.Outputs.Add(messageOutput);
        builder.Root = new Sequence
        {
            Activities =
            {
                new WriteLine(context => context.GetInput<string>("Message")),
                new SetOutput
                {
                    OutputName = new(messageOutput.Name),
                    OutputValue = new("Hello from Child")
                }
            }
        };
    }
}