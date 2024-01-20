using System.Dynamic;
using System.Net;
using Elsa.Http;
using Elsa.Http.Models;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;

namespace WorkflowApp.Web.Workflows;

public class GetUser : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var routeDataVariable = builder.WithVariable<IDictionary<string, object>>();
        var userIdVariable = builder.WithVariable<string>();
        var userVariable = builder.WithVariable<ExpandoObject>();

        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("users/{userid}"),
                    SupportedMethods = new(new[] { HttpMethods.Get }),
                    CanStartWorkflow = true,
                    RouteData = new(routeDataVariable)
                },
                new SetVariable
                {
                    Variable = userIdVariable,
                    Value = new(context =>
                    {
                        var routeData = routeDataVariable.Get(context)!;
                        var userId = routeData["userid"].ToString();
                        return userId;
                    })
                },
                new SendHttpRequest
                {
                    Url = new(context =>
                    {
                        var userId = userIdVariable.Get(context);
                        return new Uri($"https://reqres.in/api/users/{userId}");
                    }),
                    Method = new(HttpMethods.Get),
                    ParsedContent = new(userVariable),
                    ExpectedStatusCodes =
                    {
                        new HttpStatusCodeCase
                        {
                            StatusCode = StatusCodes.Status200OK,
                            Activity = new WriteHttpResponse
                            {
                                Content = new(context =>
                                {
                                    var user = (dynamic)userVariable.Get(context)!;
                                    return user.data;
                                }),
                                StatusCode = new(HttpStatusCode.OK)
                            }
                        },
                        new HttpStatusCodeCase
                        {
                            StatusCode = StatusCodes.Status404NotFound,
                            Activity = new WriteHttpResponse
                            {
                                Content = new("User not found"),
                                StatusCode = new(HttpStatusCode.NotFound)
                            }
                        }
                    }
                }
            }
        };
    }
}