using System.Threading.Tasks;
using DocumentManagement.Core.Models;
using DocumentManagement.Core.Services;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Providers.WorkflowStorage;
using Elsa.Services;
using Elsa.Services.Models;

namespace DocumentManagement.Workflows.Activities
{
    [Activity(Category = "Document Management", Description = "Archives the specified document.")]
    public class ArchiveDocument : Activity
    {
        private readonly IDocumentStore _documentStore;

        public ArchiveDocument(IDocumentStore documentStore, IFileStorage fileStorage)
        {
            _documentStore = documentStore;
        }
        
        [ActivityInput(
            Label = "Document",
            Hint = "The document to archive",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid},
            DefaultWorkflowStorageProvider = TransientWorkflowStorageProvider.ProviderName
        )]
        public Document Document { get; set; } = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            Document.Status = DocumentStatus.Archived;
            await _documentStore.SaveAsync(Document);
            return Done();
        }
    }
}