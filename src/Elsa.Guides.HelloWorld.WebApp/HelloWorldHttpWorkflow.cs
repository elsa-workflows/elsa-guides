using System;
using System.Net;
using System.Net.Http;
using Elsa.Activities.Http.Activities;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Guides.HelloWorld.WebApp
{
    public class HelloWorldHttpWorkflow : IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {
            builder
                .StartWith<ReceiveHttpRequest>(
                    x =>
                    {
                        x.Method = HttpMethod.Get.Method;
                        x.Path = new Uri("/hello-world", UriKind.Relative);
                    }
                )
                .Then<WriteHttpResponse>(
                    x =>
                    {
                        x.Content = new LiteralExpression("<h1>Hello World!</h1>");
                        x.ContentType = "text/html";
                        x.StatusCode = HttpStatusCode.OK;
                        x.ResponseHeaders = new LiteralExpression("X-Powered-By=Elsa Workflows");
                    }
                );
        }
    }
}