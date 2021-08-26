using System.Threading;
using System.Threading.Tasks;
using DocumentManagement.Core.Models;
using DocumentManagement.Workflows.Activities;
using Elsa.Scripting.Liquid.Messages;
using Fluid;
using MediatR;

namespace DocumentManagement.Workflows.Scripting.Liquid
{
    public class ConfigureLiquidEngine : INotificationHandler<EvaluatingLiquidExpression>
    {
        public Task Handle(EvaluatingLiquidExpression notification, CancellationToken cancellationToken)
        {
            var memberAccessStrategy = notification.TemplateContext.Options.MemberAccessStrategy;
            
            memberAccessStrategy.Register<Document>();
            memberAccessStrategy.Register<DocumentFile>();
            
            return Task.CompletedTask;
        }
    }
}