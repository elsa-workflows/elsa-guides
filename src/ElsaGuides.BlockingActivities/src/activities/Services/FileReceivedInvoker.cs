using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;
using MyActivityLibrary.Activities;
using MyActivityLibrary.Bookmarks;
using MyActivityLibrary.Models;
using Open.Linq.AsyncExtensions;

namespace MyActivityLibrary.Services
{
    public class FileReceivedInvoker : IFileReceivedInvoker
    {
        private readonly IWorkflowLaunchpad _workflowLaunchpad;

        public FileReceivedInvoker(IWorkflowLaunchpad workflowLaunchpad)
        {
            _workflowLaunchpad = workflowLaunchpad;
        }

        public async Task<IEnumerable<CollectedWorkflow>> DispatchWorkflowsAsync(FileModel file, CancellationToken cancellationToken = default)
        {
            var collectedWorkflows = await CollectWorkflowsAsync(file, cancellationToken).Select(x => x).ToList();
            await _workflowLaunchpad.DispatchPendingWorkflowsAsync(collectedWorkflows, new WorkflowInput(file), cancellationToken);

            return collectedWorkflows;
        }

        public async Task<IEnumerable<CollectedWorkflow>> ExecuteWorkflowsAsync(FileModel file, CancellationToken cancellationToken = default)
        {
            var collectedWorkflows = await CollectWorkflowsAsync(file, cancellationToken).Select(x => x).ToList();
            await _workflowLaunchpad.ExecutePendingWorkflowsAsync(collectedWorkflows, new WorkflowInput(file), cancellationToken);

            return collectedWorkflows;
        }

        private async Task<IEnumerable<CollectedWorkflow>> CollectWorkflowsAsync(FileModel fileModel, CancellationToken cancellationToken)
        {
            var wildcardContext = new WorkflowsQuery(nameof(FileReceived), new FileReceivedBookmark());
            var filteredContext = new WorkflowsQuery(nameof(FileReceived), new FileReceivedBookmark(Path.GetExtension(fileModel.FileName)));

            var wildcardWorkflows = await _workflowLaunchpad.FindWorkflowsAsync(wildcardContext, cancellationToken).ToList();
            var filteredWorkflows = await _workflowLaunchpad.FindWorkflowsAsync(filteredContext, cancellationToken).ToList();

            return wildcardWorkflows.Concat(filteredWorkflows);
        }
    }
}