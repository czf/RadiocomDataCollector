using System;

namespace Czf.Engine.RadiocomDataCollector
{
    public class SystemDateTimeOffsetProvider : IDateTimeOffsetProvider
    {
        public DateTimeOffset Now { get => DateTimeOffset.Now; }
        public DateTimeOffset UtcNow { get => DateTimeOffset.UtcNow; }
    }
}

