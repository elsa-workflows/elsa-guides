using System.IO;
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
    public record DocumentFile(Document Document, Stream FileStream);

    [Action(Category = "Document Management", Description = "Gets the specified document from the database.")]
    public class GetDocument : Activity
    {
        private readonly IDocumentStore _documentStore;
        private readonly IFileStorage _fileStorage;

        public GetDocument(IDocumentStore documentStore, IFileStorage fileStorage)
        {
            _documentStore = documentStore;
            _fileStorage = fileStorage;
        }


        [ActivityInput(
            Label = "Document ID",
            Hint = "The ID of the document to load",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string DocumentId { get; set; } = default!;

        [ActivityOutput(
            Hint = "The document + file.",
            DefaultWorkflowStorageProvider = TransientWorkflowStorageProvider.ProviderName,
            DisableWorkflowProviderSelection = true)]
        public DocumentFile Output { get; set; } = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var document = await _documentStore.GetAsync(DocumentId, context.CancellationToken);
            var fileStream = await _fileStorage.ReadAsync(document!.FileName, context.CancellationToken);

            Output = new DocumentFile(document, fileStream);
            return Done();
        }
    }
}