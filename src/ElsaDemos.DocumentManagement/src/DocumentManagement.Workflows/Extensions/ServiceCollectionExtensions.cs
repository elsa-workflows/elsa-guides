using System;
using System.IO;
using DocumentManagement.Core.Options;
using DocumentManagement.Workflows.Activities;
using DocumentManagement.Workflows.Handlers;
using DocumentManagement.Workflows.Scripting.JavaScript;
using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Providers.Workflows;
using Elsa.Server.Hangfire.Extensions;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Storage.Net;

namespace DocumentManagement.Workflows.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflowServices(this IServiceCollection services, IHostEnvironment environment, Action<DbContextOptionsBuilder> configureDb)
        {
            services.Configure<DocumentStorageOptions>(options => options.BlobStorageFactory = StorageFactory.Blobs.InMemory);

            return services
                .AddElsa(environment, configureDb)
                .AddNotificationHandlersFrom<StartDocumentWorkflows>()
                .AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
        }

        private static IServiceCollection AddElsa(this IServiceCollection services, IHostEnvironment environment, Action<DbContextOptionsBuilder> configureDb)
        {
            services
                .AddElsa(elsa => elsa

                    // Use EF Core's SQLite provider to store workflow instances and bookmarks.
                    .UseEntityFrameworkPersistence(configureDb)
                    
                    // Ue Console activities for testing & demo purposes.
                    .AddConsoleActivities()

                    // Use Hangfire to dispatch workflows from.
                    .UseHangfireDispatchers()

                    // Configure Email activities.
                    .AddEmailActivities()

                    // Configure HTTP activities.
                    .AddHttpActivities()

                    // Add custom activities.
                    .AddActivitiesFrom<ArchiveDocument>()
                );

            // Register custom type definition provider for JS intellisense.
            services.AddJavaScriptTypeDefinitionProvider<CustomTypeDefinitionProvider>();

            // Configure Storage for BlobStorageWorkflowProvider with a directory on disk from where to load workflow definition JSON files from the local "Workflows" folder.
            var currentAssemblyPath = Path.GetDirectoryName(typeof(ServiceCollectionExtensions).Assembly.Location);
            services.Configure<BlobStorageWorkflowProviderOptions>(options => options.BlobStorageFactory = () => StorageFactory.Blobs.DirectoryFiles(Path.Combine(currentAssemblyPath, "Workflows")));

            return services;
        }
    }
}