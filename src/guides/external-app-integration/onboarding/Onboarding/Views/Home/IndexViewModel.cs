using Onboarding.Entities;

namespace Onboarding.Views.Home;

public class IndexViewModel(ICollection<OnboardingTask> tasks)
{
    public ICollection<OnboardingTask> Tasks { get; set; } = tasks;
}