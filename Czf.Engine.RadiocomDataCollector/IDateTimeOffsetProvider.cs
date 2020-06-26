using System;

namespace Czf.Engine.RadiocomDataCollector
{


    public interface IDateTimeOffsetProvider
    {
        DateTimeOffset Now { get; }
        DateTimeOffset UtcNow { get; }
    }
}

