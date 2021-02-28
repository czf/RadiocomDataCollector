using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Czf.ApiWrapper.Radiocom;
using Czf.Engine.RadiocomDataCollector.Czf.Notification.Radiocom;
using Czf.Repository.Radiocom;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Czf.Engine.RadiocomDataCollector
{
    public class RadiocomDataCollectorEngine
    {
        #region static/consts
        private const int KISW_STATION_ID = 902;
        #endregion static/consts

        #region private        
        private IRadiocomClient _client;
        private IRadiocomRepository _radiocomRepository;
        private readonly RadiocomDataCollectorEngineOptions _radiocomDataCollectorEngineOptions;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private ILogger _log;
        private readonly IPublishCollectorEventCompleted _publishCollectorEventCompleted;
        #endregion private
        public RadiocomDataCollectorEngine(
            IRadiocomClient client, 
            IRadiocomRepository radiocomRepository, 
            IOptions<RadiocomDataCollectorEngineOptions> radiocomDataCollectorEngineOptions, 
            IDateTimeOffsetProvider dateTimeOffsetProvider, 
            ILogger<RadiocomDataCollectorEngine> log,
            IPublishCollectorEventCompleted publishCollectorEventCompleted)
        {
            _client = client;
            _radiocomRepository = radiocomRepository;
            _radiocomDataCollectorEngineOptions = radiocomDataCollectorEngineOptions.Value;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _log = log;
            _publishCollectorEventCompleted = publishCollectorEventCompleted;
        }


        public async Task Run()
        {
            HashSet<DateTimeOffset> timestamps = new HashSet<DateTimeOffset>();
            byte runs = 0;
            DateTimeOffset now = _dateTimeOffsetProvider.UtcNow;
            bool continueRunning;
            do
            {
                DateTimeOffset date = now.AddHours(runs * -1);
                int hour = date.Hour;
                StationRecentlyPlayedResponse response = await GetResponse(date, hour);
                response.Schedule.RemoveAll(x => x.Playing);
                IEnumerable<RawArtistWorkStationOccurrence> rawOccurrences = response.ToRawOccurrences();

                rawOccurrences = rawOccurrences.Where(x =>  !timestamps.Contains(x.StartTime)).AsList();
                timestamps.UnionWith(rawOccurrences.Select(x => x.StartTime));
                int occurrences = rawOccurrences.Count();

                runs++;
                continueRunning = runs <= _radiocomDataCollectorEngineOptions.HoursBackToRetrive;
                if (continueRunning && occurrences > 0)
                {
                    Thread.Sleep(100);//throttle requests
                    _log.LogInformation($"Will process {occurrences} raw occurrences.");
                    int totalNeededProcessing = await _radiocomRepository.ProcessRawOccurrances(rawOccurrences);
                    continueRunning =  totalNeededProcessing == occurrences;
                    _log.LogInformation($"Total occurrences needed processing: {totalNeededProcessing}.");
                }
            } while (continueRunning);
            await _publishCollectorEventCompleted.NotifyCollectorEventCompleted();

        }

        private async Task<StationRecentlyPlayedResponse> GetResponse(DateTimeOffset date, int hour)
        {
            DateTimeOffset time = TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
            StationRecentlyPlayedResponse response = null;
            byte tries = 0;
            bool tryAgain;
            do
            {
                try
                {
                    tries += 1;
                    response = await _client.StationRecentlyPlayed(KISW_STATION_ID, hour, time.DayOfWeek.ToRadiocomDayOfWeek());
                    tryAgain = false;
                }
                catch (HttpRequestException httpRequestException)
                {
                    _log.LogError(httpRequestException, $"requesting station:{KISW_STATION_ID}, hour:{hour}, day:{time.DayOfWeek}");
                    tryAgain = tries < 3;
                    if (!tryAgain) throw;
                }
            } while (tryAgain);
            return response;
        }
    }
}
