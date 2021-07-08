using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ElsaGuides.WorkflowContexts.Web.Data
{
    public class SqliteBlogContextFactory : IDesignTimeDbContextFactory<BlogContext>
    {
        public BlogContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<BlogContext>();
            var connectionString = args.Any() ? args[0] : "Data Source=blog.db;Cache=Shared";
            
            builder.UseSqlite(connectionString, db => db
                .MigrationsAssembly(typeof(SqliteBlogContextFactory).Assembly.GetName().Name));
            
            return new BlogContext(builder.Options);
        }
    }
}