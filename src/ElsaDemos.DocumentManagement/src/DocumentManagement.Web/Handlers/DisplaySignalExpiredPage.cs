using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Http.Events;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DocumentManagement.Web.Handlers
{
    /// <summary>
    /// Redirects to a generic "Signal Expired" page.
    /// </summary>
    public class DisplaySignalExpiredPage : INotificationHandler<HttpTriggeredSignal>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DisplaySignalExpiredPage(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task Handle(HttpTriggeredSignal notification, CancellationToken cancellationToken)
        {
            var affectedWorkflows = notification.AffectedWorkflows;

            if (affectedWorkflows.Count > 0)
                return Task.CompletedTask;

            var response = _httpContextAccessor.HttpContext!.Response;
            response.Redirect("/signal-expired");
            
            return Task.CompletedTask;
        }
    }
}