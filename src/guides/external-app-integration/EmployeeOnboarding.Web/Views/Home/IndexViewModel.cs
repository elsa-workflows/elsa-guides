using EmployeeOnboarding.Web.Entities;

namespace EmployeeOnboarding.Web.Views.Home;

public class IndexViewModel(ICollection<OnboardingTask> tasks)
{
    public ICollection<OnboardingTask> Tasks { get; set; } = tasks;
}