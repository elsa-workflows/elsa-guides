using System;

namespace DocumentManagement.Core.Services
{
    public interface ISystemClock
    {
        DateTime UtcNow { get; }
    }
}