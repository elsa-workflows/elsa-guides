using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyActivityLibrary.Models;
using MyActivityLibrary.Services;

namespace Elsa.Server.Web.Endpoints.Files
{
    [ApiController]
    [Route("files")]
    public class Post : Controller
    {
        private readonly IFileReceivedInvoker _invoker;

        public Post(IFileReceivedInvoker invoker)
        {
            _invoker = invoker;
        }

        [HttpPost]
        public async Task<IActionResult> Handle(IFormFile file)
        {
            var fileModel = new FileModel
            {
                FileName = Path.GetFileName(file.FileName),
                Content = await file.OpenReadStream().ReadBytesToEndAsync(CancellationToken.None),
                MimeType = file.ContentType
            };
            
            var collectedWorkflows = await _invoker.DispatchWorkflowsAsync(fileModel);
            return Ok(collectedWorkflows.ToList());
        }
    }
}