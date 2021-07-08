using ElsaGuides.WorkflowContexts.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaGuides.WorkflowContexts.Web.Data
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; } = default!;
    }
}