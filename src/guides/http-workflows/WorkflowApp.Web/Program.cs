using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using WorkflowApp.Web.Workflows;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Add Elsa services.
builder.Services.AddElsa(elsa =>
{
    elsa.UseIdentity(identity =>
    {
        identity.UseAdminUserProvider();
        identity.TokenOptions = tokenOptions => tokenOptions.SigningKey = "my-long-256-bit-secret-token-signing-key";
    });
    elsa.UseDefaultAuthentication();
    elsa.UseWorkflowManagement(management =>
    {
        management.UseEntityFrameworkCore();
        management.AddVariableType<WeatherForecast>(category: "Weather");
    });
    elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore());
    elsa.UseJavaScript();
    elsa.UseLiquid();
    elsa.UseWorkflowsApi();
    elsa.UseHttp(http => http.ConfigureHttpOptions = options =>
    {
        options.BaseUrl = new Uri("https://localhost:5001");
        options.BasePath = "/workflows";
    });
    elsa.AddWorkflow<WeatherForecastWorkflow>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.MapControllers();
app.MapRazorPages();

// Create a sample weather forecast API.
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
        .ToArray();
    return forecast;
});

// Run the application.
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}