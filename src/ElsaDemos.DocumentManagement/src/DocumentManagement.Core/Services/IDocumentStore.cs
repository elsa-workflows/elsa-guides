using System.Threading;
using System.Threading.Tasks;
using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Services
{
    public interface IDocumentStore
    {
        Task SaveAsync(Document entity, CancellationToken cancellationToken = default);
        Task<Document?> GetAsync(string id, CancellationToken cancellationToken = default);
    }
}