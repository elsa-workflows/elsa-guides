using System;
using Elsa.Activities.Console.Activities;
using Elsa.Activities.Timers.Activities;
using Elsa.Expressions;
using Elsa.Scripting.JavaScript;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Guides.RecurringTask.ConsoleApp
{
    public class RecurringTaskWorkflow : IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {
            builder
                .AsSingleton()
                .StartWith<TimerEvent>(x => x.TimeoutExpression = new LiteralExpression<TimeSpan>("00:00:05"))
                .Then<WriteLine>(x => x.TextExpression = new JavaScriptExpression<string>("`It's now ${new Date()}. Let's do this thing!`"));
        }
    }
}