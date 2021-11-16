using System;

namespace SFA.DAS.Charities.Import.Infrastructure
{
    public interface IDateTimeProvider
    {
        DateTime Today { get;  }
    }
}
