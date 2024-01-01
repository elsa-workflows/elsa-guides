using EmployeeOnboarding.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding.Web.Data;

public class OnboardingDbContext(DbContextOptions<OnboardingDbContext> options) : DbContext(options)
{
    public DbSet<OnboardingTask> Tasks { get; set; } = default!;
}