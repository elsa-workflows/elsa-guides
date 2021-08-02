using DocumentManagement.Core.Services;
using DocumentManagement.Persistence.HostedServices;
using DocumentManagement.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentManagement.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainPersistence(this IServiceCollection services, string connectionString)
        {
            var migrationsAssemblyName = typeof(SqliteDocumentDbContextFactory).Assembly.GetName().Name;

            return services
                .AddPooledDbContextFactory<DocumentDbContext>(x => x.UseSqlite(connectionString, db => db.MigrationsAssembly(migrationsAssemblyName)))
                .AddSingleton<IDocumentStore, EFCoreDocumentStore>()
                .AddSingleton<IDocumentTypeStore, EFCoreDocumentTypeStore>()
                .AddHostedService<RunMigrations>();
        }
    }
}