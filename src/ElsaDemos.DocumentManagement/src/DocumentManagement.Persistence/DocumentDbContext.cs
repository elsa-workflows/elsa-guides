using DocumentManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Persistence
{
    public class DocumentDbContext : DbContext
    {
        public DocumentDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; } = default!;
        public DbSet<DocumentType> DocumentTypes { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentType>().HasData(
                CreateDocumentType("ChangeRequest", "Change Request"),
                CreateDocumentType("LeaveRequest", "Leave Request"),
                CreateDocumentType("IdentityVerification", "Identity Verification"));
        }

        private static DocumentType CreateDocumentType(string id, string name) => new DocumentType
        {
            Id = id,
            Name = name
        };
    }
}