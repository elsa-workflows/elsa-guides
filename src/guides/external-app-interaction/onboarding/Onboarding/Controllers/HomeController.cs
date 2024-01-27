using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Onboarding.Data;
using Onboarding.Models;
using Onboarding.Services;
using Onboarding.Views.Home;

namespace Onboarding.Controllers;

public class HomeController(OnboardingDbContext dbContext, ElsaClient elsaClient, ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var tasks = await dbContext.Tasks.Where(x => !x.IsCompleted).ToListAsync(cancellationToken: cancellationToken);
        var model = new IndexViewModel(tasks);
        return View(model);
    }
    
    public async Task<IActionResult> CompleteTask(int taskId, CancellationToken cancellationToken)
    {
        var task = dbContext.Tasks.FirstOrDefault(x => x.Id == taskId);

        if (task == null)
            return NotFound();

        await elsaClient.ReportTaskCompletedAsync(task.ExternalId, cancellationToken: cancellationToken);

        task.IsCompleted = true;
        task.CompletedAt = DateTimeOffset.Now;

        dbContext.Tasks.Update(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
