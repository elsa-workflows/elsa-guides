using Elsa.Http;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;

namespace ElsaWeb.Workflows;

public class HttpHelloWorld : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var queryStringsVariable = builder.WithVariable<IDictionary<string, object>>();
        var messageVariable = builder.WithVariable<string>();

        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("/hello-world"),
                    CanStartWorkflow = true,
                    QueryStringData = new(queryStringsVariable)
                },
                new SetVariable
                {
                    Variable = messageVariable,
                    Value = new(context =>
                    {
                        var queryStrings = queryStringsVariable.Get(context)!;
                        var message = queryStrings.TryGetValue("message", out var messageValue) ? messageValue.ToString() : "Hello world of HTTP workflows!";
                        return message;
                    })
                },
                new WriteHttpResponse
                {
                    Content = new(messageVariable)
                }
            }
        };
    }
}