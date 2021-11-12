using System;

namespace SFA.DAS.Charities.Import.Infrastructure
{
    public interface IDateTimeProvider
    {
        DateTime Today { get;  }
    }

    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Today { get; } = DateTime.Today;
    }
}
