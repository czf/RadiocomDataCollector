using System;


namespace Czf.Repository.Radiocom
{
    public class RawArtistWorkStationOccurrence
    {
        public DateTimeOffset StartTime { get; set; }
        public long StationId { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
    }
}

