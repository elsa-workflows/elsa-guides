using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;
using Elsa.Workflows.Runtime.Activities;
using Parallel = Elsa.Workflows.Activities.Parallel;

namespace ElsaServer.Workflows;

public class Onboarding : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var employee = builder.WithVariable<object>();
        builder.Root = new Sequence
        {
            Activities =
            {
                new SetVariable
                {
                    Variable = employee,
                    Value = new(context => context.GetInput("Employee"))
                },
                new RunTask("Create Email Account")
                {
                    Payload = new(context => new Dictionary<string, object>
                    {
                        ["Employee"] = employee.Get(context)!,
                        ["Description"] = "Create an email account for the new employee."
                    })
                },
                new Parallel
                {
                    Activities =
                    {
                        new RunTask("Create Slack Account")
                        {
                            Payload = new(context => new Dictionary<string, object>
                            {
                                ["Employee"] = employee.Get(context)!,
                                ["Description"] = "Create a Slack account for the new employee."
                            })
                        },
                        new RunTask("Create GitHub Account")
                        {
                            Payload = new(context => new Dictionary<string, object>
                            {
                                ["Employee"] = employee.Get(context)!,
                                ["Description"] = "Create a GitHub account for the new employee."
                            })
                        },
                        new RunTask("Add to HR System")
                        {
                            Payload = new(context => new Dictionary<string, object>
                            {
                                ["Employee"] = employee.Get(context)!,
                                ["Description"] = "Add the new employee to the HR system."
                            })
                        }
                    }
                },
                new End()
            }
        };
    }
}