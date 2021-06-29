using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services.Models;
using MyActivityLibrary.Models;

namespace MyActivityLibrary.Services
{
    public interface IFileReceivedInvoker
    {
        Task<IEnumerable<CollectedWorkflow>> DispatchWorkflowsAsync(FileModel file, CancellationToken cancellationToken = default);
        Task<IEnumerable<CollectedWorkflow>> ExecuteWorkflowsAsync(FileModel file, CancellationToken cancellationToken = default);
    }
}