using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Models;
using Elsa.Providers.WorkflowStorage;
using Elsa.Services;
using Elsa.Services.Models;
using Hangfire;

namespace DocumentManagement.Workflows.Activities
{
    [Activity(
        Category = "Document Management",
        Description = "Saves a hash of the specified file onto the blockchain to prevent tampering."
    )]
    public class UpdateBlockchain : Activity
    {
        public record UpdateBlockchainContext(string WorkflowInstanceId, string ActivityId, string FileSignature);

        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IWorkflowInstanceDispatcher _workflowInstanceDispatcher;

        public UpdateBlockchain(IBackgroundJobClient backgroundJobClient, IWorkflowInstanceDispatcher workflowInstanceDispatcher)
        {
            _backgroundJobClient = backgroundJobClient;
            _workflowInstanceDispatcher = workflowInstanceDispatcher;
        }

        [ActivityInput(
            Label = "File",
            Hint = "The file to store its hash of onto the blockchain. Can be byte[] or Stream.",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid},
            DefaultWorkflowStorageProvider = TransientWorkflowStorageProvider.ProviderName
        )]
        public object File { get; set; } = default!;
        
        [ActivityOutput(Hint = "The computed file signature as stored on the blockchain.")]
        public string Output { get; set; } = default!;

        /// <summary>
        /// Invoked by Hangfire as a background job.
        /// </summary>
        public async Task SubmitToBlockChainAsync(UpdateBlockchainContext context, CancellationToken cancellationToken)
        {
            // Simulate storing it on an imaginary blockchain out there.
            await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);

            // Resume the suspended workflow.
            await _workflowInstanceDispatcher.DispatchAsync(new ExecuteWorkflowInstanceRequest(context.WorkflowInstanceId, context.ActivityId, new WorkflowInput(context.FileSignature)), cancellationToken);
        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            // Compute hash.
            var bytes = File is Stream stream ? await stream.ReadBytesToEndAsync() : File is byte[] buffer ? buffer : throw new NotSupportedException();
            var fileSignature = ComputeSignature(bytes);

            // Schedule background work using Hangfire.
            _backgroundJobClient.Enqueue(() => SubmitToBlockChainAsync(new UpdateBlockchainContext(context.WorkflowInstance.Id, context.ActivityId, fileSignature), CancellationToken.None));

            // Suspend the workflow.
            return Suspend();
        }

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            // When we resume, simply complete this activity.
            var fileSignature = context.GetInput<string>();
            Output = fileSignature;
            return Done();
        }

        private static string ComputeSignature(byte[] bytes)
        {
            using var algorithm = SHA256.Create();
            var hashValue = algorithm.ComputeHash(bytes);
            return Convert.ToBase64String(hashValue);
        }
    }
}