using System.Threading;
using System.Threading.Tasks;
using DocumentManagement.Core.Events;
using Elsa.Models;
using Elsa.Services;
using MediatR;
using Open.Linq.AsyncExtensions;

namespace DocumentManagement.Workflows.Handlers
{
    /// <summary>
    /// Handles the <see cref="NewDocumentReceived"/> event by starting new workflows associated with the document type.
    /// </summary>
    public class StartDocumentWorkflows : INotificationHandler<NewDocumentReceived>
    {
        private readonly IWorkflowRegistry _workflowRegistry;
        private readonly IWorkflowDefinitionDispatcher _workflowDispatcher;

        public StartDocumentWorkflows(IWorkflowRegistry workflowRegistry, IWorkflowDefinitionDispatcher workflowDispatcher)
        {
            _workflowRegistry = workflowRegistry;
            _workflowDispatcher = workflowDispatcher;
        }
        
        public async Task Handle(NewDocumentReceived notification, CancellationToken cancellationToken)
        {
            var document = notification.Document;
            var documentTypeId = document.DocumentTypeId;
            
            // Get all workflow blueprints tagged with the received document type ID.
            var workflowBlueprints = await _workflowRegistry.FindManyAsync(x => x.IsPublished && x.Tag == documentTypeId, cancellationToken).ToList();

            // Dispatch each workflow. Each workflow will be correlated by Document ID.
            foreach (var workflowBlueprint in workflowBlueprints) 
                await _workflowDispatcher.DispatchAsync(new ExecuteWorkflowDefinitionRequest(workflowBlueprint.Id, CorrelationId: document.Id, Input: new WorkflowInput(document.Id)), cancellationToken);
        }
    }
}