using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.SqlServer.Server;


namespace Czf.Repository.Radiocom
{
    public class RawArtistWorkStationOccurrenceDataRecords : IEnumerable<SqlDataRecord>
    {
        private static readonly SqlMetaData startTimeMetaData = new SqlMetaData(nameof(RawArtistWorkStationOccurrence.StartTime), SqlDbType.DateTimeOffset);
        private static readonly SqlMetaData stationIdMetaData = new SqlMetaData(nameof(RawArtistWorkStationOccurrence.StationId), SqlDbType.BigInt);
        private static readonly SqlMetaData artistMetaData = new SqlMetaData(nameof(RawArtistWorkStationOccurrence.Artist), SqlDbType.NVarChar, 100);
        private static readonly SqlMetaData titleMetaData = new SqlMetaData(nameof(RawArtistWorkStationOccurrence.Title), SqlDbType.NVarChar, 100);

        private const int STARTTIME_INDEX = 0;
        private const int STATIONID_INDEX = 1;
        private const int ARTIST_INDEX = 2;
        private const int TITLE_INDEX = 3;

        private IEnumerable<RawArtistWorkStationOccurrence> _occurrences;
        public RawArtistWorkStationOccurrenceDataRecords(IEnumerable<RawArtistWorkStationOccurrence> occurrences)
        {
            _occurrences = occurrences;
        }

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            foreach (var occurrence in _occurrences)
            {
                var record = new SqlDataRecord(
                    startTimeMetaData,
                    stationIdMetaData,
                    artistMetaData,
                    titleMetaData);

                record.SetDateTimeOffset(STARTTIME_INDEX, occurrence.StartTime);
                record.SetInt64(STATIONID_INDEX, occurrence.StationId);
                record.SetString(ARTIST_INDEX, occurrence.Artist);
                record.SetString(TITLE_INDEX, occurrence.Title);
                yield return record;
            }

        }

        public IEnumerator GetEnumerator() => throw new NotImplementedException();
    }
}

