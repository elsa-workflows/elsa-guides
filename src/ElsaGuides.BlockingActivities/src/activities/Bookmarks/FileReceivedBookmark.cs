using Elsa.Services;
using Elsa.Services.Bookmarks;

namespace MyActivityLibrary.Bookmarks
{
    public class FileReceivedBookmark : IBookmark
    {
        public FileReceivedBookmark()
        {
        }

        public FileReceivedBookmark(string fileExtension)
        {
            FileExtension = fileExtension;
        }
        
        public string? FileExtension { get; set; }
    }
}