using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;
using Elsa.Services.Bookmarks;
using MyActivityLibrary.Activities;

namespace MyActivityLibrary.Bookmarks
{
    public class FileReceivedBookmarkProvider : BookmarkProvider<FileReceivedBookmark, FileReceived>
    {
        public override async ValueTask<IEnumerable<BookmarkResult>> GetBookmarksAsync(BookmarkProviderContext<FileReceived> context, CancellationToken cancellationToken)
        {
            var supportedExtensions = (await context.ReadActivityPropertyAsync<FileReceived, ICollection<string>>(x => x.SupportedFileExtensions, cancellationToken))?.ToList() ?? new List<string>();

            return !supportedExtensions.Any() 
                ? new[] {Result(new FileReceivedBookmark())} 
                : supportedExtensions.Select(x => Result(new FileReceivedBookmark(x)));
        }
    }
}