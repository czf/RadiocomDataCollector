using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Czf.Engine.RadiocomDataCollector;

namespace Czf.App.RadiocomDataCollector
{
    public class Function1
    {

        private readonly RadiocomDataCollectorEngine _radiocomDataCollectorEngine;

        public Function1(RadiocomDataCollectorEngine radiocomDataCollectorEngine)
        {
            _radiocomDataCollectorEngine = radiocomDataCollectorEngine;
        }

        [Disable("TEST_RUNNER")]
        [FunctionName("test")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            await _radiocomDataCollectorEngine.Run();

            return new OkResult();
        }
    }
}
