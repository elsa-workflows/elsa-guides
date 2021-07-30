using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentManagement.Core.Services
{
    public interface IFileStorage
    {
        Task WriteAsync(Stream data, string fileName, CancellationToken cancellationToken = default);
        Task<Stream> ReadAsync(string fileName, CancellationToken cancellationToken = default);
    }
}