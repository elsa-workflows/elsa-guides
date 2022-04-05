using Elsa.Scripting.Liquid.Messages;
using ElsaGuides.Orders.Web.Models.Entities;
using Fluid;
using MediatR;

namespace ElsaGuides.Orders.Web.Handlers
{
    public class ConfigureLiquidEngine : INotificationHandler<EvaluatingLiquidExpression>
    {
        public Task Handle(EvaluatingLiquidExpression notification, CancellationToken cancellationToken)
        {
            var memberAccessStrategy = notification.TemplateContext.Options.MemberAccessStrategy; 
            memberAccessStrategy.Register<Order>();
            return Task.CompletedTask;
        }
    }
}