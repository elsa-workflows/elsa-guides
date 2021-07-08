using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Http.Models;
using Elsa.Services;
using Elsa.Services.Models;
using ElsaGuides.WorkflowContexts.Web.Data;
using ElsaGuides.WorkflowContexts.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaGuides.WorkflowContexts.Web.Providers.WorkflowContexts
{
    public class BlogPostWorkflowContextProvider : WorkflowContextRefresher<BlogPost>
    {
        private readonly IDbContextFactory<BlogContext> _blogContextFactory;

        public BlogPostWorkflowContextProvider(IDbContextFactory<BlogContext> blogContextFactoryFactory)
        {
            _blogContextFactory = blogContextFactoryFactory;
        }

        /// <summary>
        /// Loads a BlogPost entity from the database.
        /// </summary>
        public override async ValueTask<BlogPost?> LoadAsync(LoadWorkflowContext context, CancellationToken cancellationToken = default)
        {
            var blogPostId = context.ContextId;
            await using var dbContext = _blogContextFactory.CreateDbContext();
            return await dbContext.BlogPosts.AsQueryable().FirstOrDefaultAsync(x => x.Id == blogPostId, cancellationToken);
        }

        /// <summary>
        /// Updates a BlogPost entity in the database.
        /// If there's no actual workflow context, we will get it from the input. This works because we know we have a workflow that starts with an HTTP Endpoint activity that receives BlogPost models.
        /// This is a design choice for this particular demo. In real world scenarios, you might not even need this since your workflows may be dealing with existing entities, or have (other) workflows that handle initial entity creation.
        /// The key take away is: you can do whatever you want with these workflow context providers :) 
        /// </summary>
        public override async ValueTask<string?> SaveAsync(SaveWorkflowContext<BlogPost> context, CancellationToken cancellationToken = default)
        {
            var blogPost = context.Context;
            await using var dbContext = _blogContextFactory.CreateDbContext();
            var dbSet = dbContext.BlogPosts;
            
            if (blogPost == null)
            {
                // We are handling a newly posted blog post.
                blogPost = ((HttpRequestModel) context.WorkflowExecutionContext.Input!).GetBody<BlogPost>();
                
                // Generate a new ID.
                blogPost.Id = Guid.NewGuid().ToString("N");
                
                // Set IsPublished to false to prevent caller from cheating ;)
                blogPost.IsPublished = false;
                
                // Set context.
                context.WorkflowExecutionContext.WorkflowContext = blogPost;
                context.WorkflowExecutionContext.ContextId = blogPost.Id;
                
                // Add blog post to DB.
                await dbSet.AddAsync(blogPost, cancellationToken);
            }
            else
            {
                var blogPostId = blogPost.Id;
                var existingBlogPost = await dbSet.AsQueryable().Where(x => x.Id == blogPostId).FirstAsync(cancellationToken);
                
                dbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);
            }
            
            await dbContext.SaveChangesAsync(cancellationToken);
            return blogPost.Id;
        }
    }
}