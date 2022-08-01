using System;
using System.Threading.Tasks;
using Czf.Engine.RadiocomDataCollector;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Czf.App.RadiocomDataCollector
{
    public class RadiocomEngineRunner
    {
        private readonly RadiocomDataCollectorEngine _radiocomDataCollectorEngine;

        public RadiocomEngineRunner(RadiocomDataCollectorEngine radiocomDataCollectorEngine)
        {
            _radiocomDataCollectorEngine = radiocomDataCollectorEngine;
        }

        [FunctionName("RadiocomEngineRunner")]
        public async Task Run([TimerTrigger("0 0 */4 * * *")] TimerInfo myTimer, ILogger log)
        {
            DateTimeOffset now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
            log.LogInformation($"C# Timer trigger function executed at: {now}.  Is past due: {myTimer.IsPastDue}");
            await _radiocomDataCollectorEngine.Run();
        }
    }

}
