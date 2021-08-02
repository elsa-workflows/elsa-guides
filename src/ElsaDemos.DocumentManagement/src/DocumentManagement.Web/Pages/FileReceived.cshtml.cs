using System.Threading;
using System.Threading.Tasks;
using DocumentManagement.Core.Models;
using DocumentManagement.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DocumentManagement.Web.Pages
{
    public class FileReceivedModel : PageModel
    {
        private readonly IDocumentStore _documentStore;

        public FileReceivedModel(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        [BindProperty(SupportsGet = true)] public string DocumentId { get; set; } = default!;

        public Document? Document { get; set; }

        public async Task<IActionResult?> OnGetAsync(CancellationToken cancellationToken)
        {
            Document = await _documentStore.GetAsync(DocumentId, cancellationToken);

            return Document == null ? NotFound() : default(IActionResult?);
        }
    }
}