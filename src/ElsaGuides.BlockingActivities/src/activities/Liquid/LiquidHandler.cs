using System.Threading;
using System.Threading.Tasks;
using Elsa.Scripting.Liquid.Messages;
using Fluid;
using MediatR;
using MyActivityLibrary.Models;

namespace MyActivityLibrary.Liquid
{
    public class LiquidHandler : INotificationHandler<EvaluatingLiquidExpression>
    {
        public Task Handle(EvaluatingLiquidExpression notification, CancellationToken cancellationToken)
        {
            notification.TemplateContext.Options.MemberAccessStrategy.Register<FileModel>();
            return Task.CompletedTask;
        }
    }
}