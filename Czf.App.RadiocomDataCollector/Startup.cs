using System;
using Czf.ApiWrapper.Radiocom;
using Czf.Engine.RadiocomDataCollector;
using Czf.Repository.Radiocom;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Czf.Repository.Radiocom.SqlRadiocomRepository;

[assembly: FunctionsStartup(typeof(Czf.App.RadiocomDataCollector.Startup))]
namespace Czf.App.RadiocomDataCollector
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            
            builder.Services.AddHttpClient()
                //.Configure<IOptionsSnapshot<RadiocomDataCollectorEngineOptions>>(configuration.GetSection(""))
                .AddLogging()
                .AddSingleton<RadiocomDataCollectorEngine>()
                .AddSingleton<IRadiocomClient, RadiocomClient>()
                .AddSingleton<IRadiocomRepository, SqlRadiocomRepository>()
                .AddSingleton<IDateTimeOffsetProvider, SystemDateTimeOffsetProvider>()
                .AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
            
            builder.Services
                .AddOptions()
                .AddOptions<RadiocomDataCollectorEngineOptions>()
                .Configure<IConfiguration>((settings, configuration) => { 
                    configuration.GetSection("RadiocomDataCollectorEngine").Bind(settings);
                });



            builder.Services
                .AddOptions()
                .AddOptions<SqlRadiocomRepositoryOptions>()
                .Configure<IConfiguration>((settings, configuration) => configuration.GetSection("SqlRadiocomRepositoryOptions").Bind(settings));


            //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();
        }
    }
}



//https://blog.bitscry.com/2020/01/22/dependency-injection-in-azure-functions-v3/

  //"RadiocomDataCollectorEngine": {
  //      "HoursBackToRetrive": 5
  //  },
  //  "SqlRadiocomRepositoryOptions" :{
  //      "ConnectionString": "local"
  //  }