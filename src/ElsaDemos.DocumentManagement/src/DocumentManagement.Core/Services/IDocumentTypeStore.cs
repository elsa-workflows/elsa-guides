using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Services
{
    public interface IDocumentTypeStore
    {
        Task<IEnumerable<DocumentType>> ListAsync(CancellationToken cancellationToken = default);
        Task<DocumentType?> GetAsync(string id, CancellationToken cancellationToken = default);
    }
}