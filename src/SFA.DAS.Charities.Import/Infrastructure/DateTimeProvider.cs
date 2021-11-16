using System;

namespace SFA.DAS.Charities.Import.Infrastructure
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Today { get; } = DateTime.Today;
    }
}
