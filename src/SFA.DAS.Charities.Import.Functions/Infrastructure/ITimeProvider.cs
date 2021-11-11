using System;

namespace SFA.DAS.Charities.Import.Functions.Infrastructure
{
    public interface ITimeProvider
    {
        DateTime Today { get;  }
    }

    internal class TimeProvider : ITimeProvider
    {
        public DateTime Today { get; } = DateTime.Today;
    }
}
