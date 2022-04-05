using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using Elsa.Scripting.Liquid.Messages;
using ElsaGuides.Orders.Web.Activities;
using ElsaGuides.Orders.Web.Contracts;
using ElsaGuides.Orders.Web.Handlers;
using ElsaGuides.Orders.Web.Persistence;
using ElsaGuides.Orders.Web.Providers;
using ElsaGuides.Orders.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var connectionString = "Data Source=orders.db;Cache=Shared;";

services
    .AddElsa(elsa => elsa
        .UseEntityFrameworkPersistence(options => options.UseSqlite(connectionString))
        .AddConsoleActivities()
        .AddHttpActivities(options =>
        {
            options.BasePath = "/workflows";
            options.BaseUrl = new Uri(builder.Configuration["ASPNETCORE_URLS"]);
        })
        .AddJavaScriptActivities()
        .AddQuartzTemporalActivities()
        .AddActivitiesFrom<CreateOrder>()
    );

services.AddWorkflowContextProvider<OrderContextProvider>();
services.AddNotificationHandler<EvaluatingLiquidExpression, ConfigureLiquidEngine>();
services.AddDbContext<OrdersDbContext>(options => SqliteDbContextOptionsBuilderExtensions.UseSqlite(options, connectionString));
services.AddScoped<IOrderService, OrderService>();
services.AddElsaApiEndpoints();
services.AddRazorPages();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseHttpActivities();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();