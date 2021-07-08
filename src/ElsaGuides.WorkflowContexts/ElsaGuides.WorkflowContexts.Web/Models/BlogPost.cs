namespace ElsaGuides.WorkflowContexts.Web.Models
{
    public class BlogPost
    {
        public string Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;
        public bool IsPublished { get; set; }
    }
}