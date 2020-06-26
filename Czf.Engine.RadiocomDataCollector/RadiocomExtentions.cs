using System.Collections.Generic;
using System.Data;
using System.Linq;
using Czf.ApiWrapper.Radiocom;
using Czf.Repository.Radiocom;
using DayOfWeek = Czf.ApiWrapper.Radiocom.DayOfWeek;

namespace Czf.Engine.RadiocomDataCollector
{
    public static class RadiocomExtentions
    {
        /// <summary>
        /// Convert to RawArtistWorkStationOccurrence
        /// </summary>
        /// <param name="response">data to convert</param>
        /// <returns>response data converted to RawArtistWorkStationOccurrence </returns>
        public static IEnumerable<RawArtistWorkStationOccurrence> ToRawOccurrences(this StationRecentlyPlayedResponse response)
        {
            List<ScheduleItem> items = response.Schedule;
            return items.Select(x =>
            new RawArtistWorkStationOccurrence()
            {
                Artist = x.Artist,
                Title = x.Title,
                StartTime = x.StartTime,
                StationId = response.Station.Id
            });
        }

        public static DayOfWeek ToRadiocomDayOfWeek(this System.DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case System.DayOfWeek.Friday:
                    return DayOfWeek.Friday;
                case System.DayOfWeek.Monday:
                    return DayOfWeek.Monday;
                case System.DayOfWeek.Saturday:
                    return DayOfWeek.Saturday;
                case System.DayOfWeek.Sunday:
                    return DayOfWeek.Sunday;
                case System.DayOfWeek.Thursday:
                    return DayOfWeek.Thursday;
                case System.DayOfWeek.Tuesday:
                    return DayOfWeek.Tuesday;
                case System.DayOfWeek.Wednesday:
                    return DayOfWeek.Wednesday;
                default:
                    throw new System.ComponentModel.InvalidEnumArgumentException(nameof(dayOfWeek), (int)dayOfWeek,typeof(DayOfWeek));
            }
        }
    }
}
