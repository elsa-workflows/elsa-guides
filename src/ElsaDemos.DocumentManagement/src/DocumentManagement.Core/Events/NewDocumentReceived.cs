using DocumentManagement.Core.Models;
using MediatR;

namespace DocumentManagement.Core.Events
{
    /// <summary>
    /// Published when a new document was uploaded into the system.
    /// </summary>
    public record NewDocumentReceived(Document Document) : INotification;
}