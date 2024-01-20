using Elsa.Extensions;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;
using Microsoft.Extensions.DependencyInjection;

// Setup service container.
var services = new ServiceCollection();

// Add Elsa services to the container.
services.AddElsa();

// Build the service container.
var serviceProvider = services.BuildServiceProvider();

// Instantiate an activity to run.
var activity = new Sequence
{
    Activities =
    {
        new WriteLine("Hello World!"),
        new WriteLine("We can do more than a one-liner!")
    }
};

// Resolve a workflow runner to execute the activity.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();

// Execute the activity.
await workflowRunner.RunAsync(activity);