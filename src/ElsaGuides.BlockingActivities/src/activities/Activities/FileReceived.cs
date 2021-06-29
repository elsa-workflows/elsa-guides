using System.Collections.Generic;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using MyActivityLibrary.Models;

namespace MyActivityLibrary.Activities
{
    [Trigger(
        Category = "Elsa Guides",
        Description = "Triggers when a file is received"
    )]
    public class FileReceived : Activity
    {
        [ActivityInput(
            Hint = "Specify a list of file extensions to filter. LEave empty to allow any file extension.",
            UIHint = ActivityInputUIHints.MultiText, 
            DefaultSyntax = SyntaxNames.Json,
            DefaultValue = new string[0],
            SupportedSyntaxes = new[] {SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid})]
        public ICollection<string> SupportedFileExtensions { get; set; } = new List<string>();

        [ActivityOutput] public FileModel Output { get; set; }
        
        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            return context.WorkflowExecutionContext.IsFirstPass ? OnExecuteInternal(context) : Suspend();
        }

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            return OnExecuteInternal(context);
        }

        private IActivityExecutionResult OnExecuteInternal(ActivityExecutionContext context)
        {
            var file = context.GetInput<FileModel>();
            Output = file;
            return Done();
        }
    }
}