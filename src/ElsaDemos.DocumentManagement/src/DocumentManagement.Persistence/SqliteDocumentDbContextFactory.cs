﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DocumentManagement.Persistence
{
    public class SqliteDocumentDbContextFactory : IDesignTimeDbContextFactory<DocumentDbContext>
    {
        public DocumentDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DocumentDbContext>();
            var connectionString = "Data Source=elsa.db;Cache=Shared";

            builder.UseSqlite(connectionString, db => db
                .MigrationsAssembly(typeof(SqliteDocumentDbContextFactory).Assembly.GetName().Name));

            return new DocumentDbContext(builder.Options);
        }
    }
}