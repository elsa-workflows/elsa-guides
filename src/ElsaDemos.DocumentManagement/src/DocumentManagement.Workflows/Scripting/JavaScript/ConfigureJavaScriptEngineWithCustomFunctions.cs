using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Scripting.JavaScript.Events;
using Elsa.Scripting.JavaScript.Messages;
using MediatR;
using Microsoft.AspNetCore.StaticFiles;

namespace DocumentManagement.Workflows.Scripting.JavaScript
{
    /// <summary>
    /// Registers custom JS functions and type definitions.
    /// </summary>
    public class ConfigureJavaScriptEngineWithCustomFunctions : INotificationHandler<EvaluatingJavaScriptExpression>, INotificationHandler<RenderingTypeScriptDefinitions>
    {
        private readonly IContentTypeProvider _contentTypeProvider;

        public ConfigureJavaScriptEngineWithCustomFunctions(IContentTypeProvider contentTypeProvider)
        {
            _contentTypeProvider = contentTypeProvider;
        }

        public Task Handle(EvaluatingJavaScriptExpression notification, CancellationToken cancellationToken)
        {
            var engine = notification.Engine;

            engine.SetValue("contentTypeFromFileName", (Func<string, string>) GetContentType);

            return Task.CompletedTask;
        }

        public Task Handle(RenderingTypeScriptDefinitions notification, CancellationToken cancellationToken)
        {
            var output = notification.Output;

            output.AppendLine("declare function contentTypeFromFileName(fileName: string): string");

            return Task.CompletedTask;
        }

        private string GetContentType(string fileName) => _contentTypeProvider.TryGetContentType(fileName, out var contentType) ? contentType : "application/octet-stream";
    }
}