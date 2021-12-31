using System.Collections.Generic;
using System.Threading.Tasks;
using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;

namespace ElsaGuides.PollingWorkflow.Workflow
{
    public class AbortActivity : Activity
    {
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            return Suspend();
        }


        protected override async ValueTask<IActivityExecutionResult> OnResumeAsync(ActivityExecutionContext context)
        {
            return Done();
        }
    }

    public class AbortActivityBookmark : IBookmark { }
    
    public class AbortActivityBookmarkProvider : BookmarkProvider<AbortActivityBookmark, AbortActivity> 
    {
        public override IEnumerable<BookmarkResult> GetBookmarks(BookmarkProviderContext<AbortActivity> context)
        {
            return new[] {Result(new AbortActivityBookmark())};
        }
    }
}