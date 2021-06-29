using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyActivityLibrary.Models;
using MyActivityLibrary.Services;

namespace Elsa.Server.Web.HostedServices
{
    public class FileMonitorService : IHostedService, IDisposable
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private FileSystemWatcher _watcher;

        public FileMonitorService(IHostEnvironment hostEnvironment, IContentTypeProvider contentTypeProvider, IServiceScopeFactory serviceScopeFactory)
        {
            _hostEnvironment = hostEnvironment;
            _contentTypeProvider = contentTypeProvider;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var folderPath = Path.Combine(_hostEnvironment.ContentRootPath, "Files");

            // Ensure the path exists.
            Directory.CreateDirectory(folderPath);

            _watcher = new FileSystemWatcher(folderPath)
            {
                NotifyFilter = NotifyFilters.Attributes
                    | NotifyFilters.CreationTime
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.FileName
                    | NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    | NotifyFilters.Security
                    | NotifyFilters.Size
            };

            _watcher.Created += OnFileCreated;
            _watcher.Filter = "*.*";
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _watcher.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }

        private async void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            await using var fileStream = File.OpenRead(e.FullPath);
            var content = await fileStream.ReadBytesToEndAsync();
            var fileName = e.Name!;
            var mimeType = GetMimeType(fileName);

            var fileModel = new FileModel
            {
                FileName = fileName,
                MimeType = mimeType,
                Content = content
            };

            using var scope = _serviceScopeFactory.CreateScope();
            var invoker = scope.ServiceProvider.GetRequiredService<IFileReceivedInvoker>();
            await invoker.DispatchWorkflowsAsync(fileModel);
        }

        private string GetMimeType(string fileName) => _contentTypeProvider.TryGetContentType(fileName, out var mimeType) ? mimeType : "application/octet-stream";
    }
}