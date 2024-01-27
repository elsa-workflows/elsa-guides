using Onboarding.Entities;
using Microsoft.EntityFrameworkCore;

namespace Onboarding.Data;

public class OnboardingDbContext(DbContextOptions<OnboardingDbContext> options) : DbContext(options)
{
    public DbSet<OnboardingTask> Tasks { get; set; } = default!;
}