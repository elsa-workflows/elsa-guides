using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentManagement.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Open.Linq.AsyncExtensions;

namespace DocumentManagement.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IDocumentTypeStore _documentTypeStore;
        private readonly IDocumentService _documentService;

        public IndexModel(IDocumentTypeStore documentTypeStore, IDocumentService documentService)
        {
            _documentTypeStore = documentTypeStore;
            _documentService = documentService;
        }

        [BindProperty] public string DocumentTypeId { get; set; } = default!;
        [BindProperty] public IFormFile FileUpload { get; set; } = default!;

        public ICollection<SelectListItem> DocumentTypes { get; set; } = new List<SelectListItem>();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var documentTypes = await _documentTypeStore.ListAsync(cancellationToken).ToList();
            DocumentTypes = documentTypes.Select(x => new SelectListItem(x.Name, x.Id)).ToList();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            var extension = Path.GetExtension(FileUpload.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fileStream = FileUpload.OpenReadStream();
            var document = await _documentService.SaveDocumentAsync(fileName, fileStream, DocumentTypeId, cancellationToken);

            return RedirectToPage("FileReceived", new {DocumentId = document.Id});
        }
    }
}