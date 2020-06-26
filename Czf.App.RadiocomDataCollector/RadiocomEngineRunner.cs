using System;
using System.Threading.Tasks;
using Czf.Engine.RadiocomDataCollector;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Czf.App.RadiocomDataCollector
{
    public class RadiocomEngineRunner
    {
        private readonly RadiocomDataCollectorEngine _radiocomDataCollectorEngine;

        public RadiocomEngineRunner( RadiocomDataCollectorEngine radiocomDataCollectorEngine)
        {
            _radiocomDataCollectorEngine = radiocomDataCollectorEngine;
        }

        [FunctionName("RadiocomEngineRunner")]
        public async Task Run([TimerTrigger("0 */50 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            //await _radiocomDataCollectorEngine.Run();
        }
    }
    public static class RadiocomTestRunner { 

        public static async Task<IActionResult> Run(
                    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
                    ILogger log, RadiocomDataCollectorEngine radiocomDataCollectorEngine)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await radiocomDataCollectorEngine.Run();
            return new OkResult();
        }
    }
}
