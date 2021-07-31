using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Http.Events;
using Elsa.Persistence;
using Elsa.Persistence.Specifications.WorkflowInstances;
using Elsa.Services;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DocumentManagement.Web.Handlers
{
    /// <summary>
    /// Responds to the Accept or Reject signal when the user clicks either option from the email.
    /// </summary>
    public class DisplaySignalReceivedPage : INotificationHandler<HttpTriggeredSignal>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWorkflowInstanceStore _workflowInstanceStore;
        private readonly IWorkflowRegistry _workflowRegistry;

        public DisplaySignalReceivedPage(IHttpContextAccessor httpContextAccessor, IWorkflowInstanceStore workflowInstanceStore, IWorkflowRegistry workflowRegistry)
        {
            _httpContextAccessor = httpContextAccessor;
            _workflowInstanceStore = workflowInstanceStore;
            _workflowRegistry = workflowRegistry;
        }

        public Task Handle(HttpTriggeredSignal notification, CancellationToken cancellationToken)
        {
            var affectedWorkflows = notification.AffectedWorkflows;

            if (affectedWorkflows.Count == 0)
                return Task.CompletedTask;

            var signalName = notification.SignalModel.Name;
            var response = _httpContextAccessor.HttpContext!.Response;

            switch (signalName)
            {
                case "Approve":
                    response.Redirect("/leave-request-approved");
                    break;
                case "Reject":
                    response.Redirect("/leave-request-denied");
                    break;
                case "Valid":
                    response.Redirect("/identity-confirmed");
                    break;
                case "Fake":
                    response.Redirect("/identity-theft-confirmed");
                    break;
            }

            return Task.CompletedTask;
        }
    }
}