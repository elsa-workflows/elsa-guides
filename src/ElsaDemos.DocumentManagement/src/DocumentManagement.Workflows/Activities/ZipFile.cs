using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Providers.WorkflowStorage;
using Elsa.Services;
using Elsa.Services.Models;

namespace DocumentManagement.Workflows.Activities
{
    [Action(Category = "Document Management", Description = "Zips the specified file.")]
    public class ZipFile : Activity
    {
        [ActivityInput(
            Hint = "The file stream to zip.",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript},
            DefaultWorkflowStorageProvider = TransientWorkflowStorageProvider.ProviderName,
            DisableWorkflowProviderSelection = true
        )]
        public Stream Stream { get; set; } = default!;

        [ActivityInput(
            Hint = "The file name to use for the zip entry.",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript}
        )]
        public string FileName { get; set; } = default!;

        [ActivityOutput(
            Hint = "The zipped file stream.",
            DefaultWorkflowStorageProvider = TransientWorkflowStorageProvider.ProviderName,
            DisableWorkflowProviderSelection = true
        )]
        public Stream Output { get; set; } = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var outputStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, true))
            {
                var zipEntry = zipArchive.CreateEntry(FileName, CompressionLevel.Optimal);
                await using var zipStream = zipEntry.Open();
                await Stream.CopyToAsync(zipStream);
            }

            // Reset position.
            outputStream.Seek(0, SeekOrigin.Begin);
            Output = outputStream;
            return Done();
        }
    }
}