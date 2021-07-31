using DocumentManagement.Core.Options;
using DocumentManagement.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Storage.Net;

namespace DocumentManagement.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.Configure<DocumentStorageOptions>(options => options.BlobStorageFactory = StorageFactory.Blobs.InMemory);

            return services
                .AddSingleton<ISystemClock, SystemClock>()
                .AddSingleton<IFileStorage, FileStorage>()
                .AddScoped<IDocumentService, DocumentService>();
        }
    }
}