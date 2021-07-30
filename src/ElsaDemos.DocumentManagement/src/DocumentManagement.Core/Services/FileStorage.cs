using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentManagement.Core.Options;
using Microsoft.Extensions.Options;
using Storage.Net.Blobs;

namespace DocumentManagement.Core.Services
{
    public class FileStorage : IFileStorage
    {
        private readonly IBlobStorage _blobStorage;
        
        public FileStorage(IOptions<DocumentStorageOptions> options) => _blobStorage = options.Value.BlobStorageFactory();

        public Task WriteAsync(Stream data, string fileName, CancellationToken cancellationToken = default) =>
            _blobStorage.WriteAsync(fileName, data, false, cancellationToken);

        public Task<Stream> ReadAsync(string fileName, CancellationToken cancellationToken = default) =>
            _blobStorage.OpenReadAsync(fileName, cancellationToken);
    }
}