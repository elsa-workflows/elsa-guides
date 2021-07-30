using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentManagement.Core.Events;
using DocumentManagement.Core.Models;
using MediatR;

namespace DocumentManagement.Core.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IFileStorage _fileStorage;
        private readonly ISystemClock _systemClock;
        private readonly IMediator _mediator;
        private readonly IDocumentStore _documentStore;

        public DocumentService(IFileStorage fileStorage, IDocumentStore documentStore, ISystemClock systemClock, IMediator mediator)
        {
            _fileStorage = fileStorage;
            _documentStore = documentStore;
            _systemClock = systemClock;
            _mediator = mediator;
        }

        public async Task<Document> SaveDocumentAsync(string fileName, Stream data, string documentTypeId, CancellationToken cancellationToken = default)
        {
            // Persist the uploaded file.
            await _fileStorage.WriteAsync(data, fileName, cancellationToken);

            // Create a document record.
            var document = new Document
            {
                Id = Guid.NewGuid().ToString("N"),
                Status = DocumentStatus.New,
                DocumentTypeId = documentTypeId,
                CreatedAt = _systemClock.UtcNow,
                FileName = fileName
            };

            // Save the document.
            await _documentStore.SaveAsync(document, cancellationToken);
            
            // Publish a domain event.
            await _mediator.Publish(new NewDocumentReceived(document), cancellationToken);

            return document;
        }
    }
}