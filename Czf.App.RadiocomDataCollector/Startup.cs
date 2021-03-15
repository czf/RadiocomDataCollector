using System;
using Czf.ApiWrapper.Radiocom;
using Czf.Engine.RadiocomDataCollector;
using Czf.Engine.RadiocomDataCollector.Czf.Notification.Radiocom;
using Czf.Repository.Radiocom;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Czf.Engine.RadiocomDataCollector.Czf.Notification.Radiocom.AzureStorageQueuePublishCollectorEventCompleted;
using static Czf.Repository.Radiocom.SqlConnectionFactory;
using static Czf.Repository.Radiocom.SqlRadiocomRepository;

[assembly: FunctionsStartup(typeof(Czf.App.RadiocomDataCollector.Startup))]
namespace Czf.App.RadiocomDataCollector
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient()
                .AddLogging()
                .AddSingleton<RadiocomDataCollectorEngine>()
                .AddSingleton<IRadiocomClient, RadiocomClient>()
                .AddSingleton<IRadiocomRepository, SqlRadiocomRepository>()
                .AddSingleton<IDateTimeOffsetProvider, SystemDateTimeOffsetProvider>()
                .AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
            if (Convert.ToBoolean(Environment.GetEnvironmentVariable("IsLocalEnvironment")))
            {
                builder.Services
                    .AddSingleton<IPublishCollectorEventCompleted, LocalStorageQueuePublishCollectorEventCompleted>();
            }
            else {
                builder.Services
                    .AddSingleton<IPublishCollectorEventCompleted, AzureStorageQueuePublishCollectorEventCompleted>();

                builder.Services
                .AddOptions()
                .AddOptions<AzureStorageQueuePublishCollectorEventCompletedOptions>()
                .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(AzureStorageQueuePublishCollectorEventCompletedOptions.AzureStorageQueuePublishCollectorEventCompleted).Bind(settings));
            }
            builder.Services
                .AddOptions()
                .AddOptions<RadiocomDataCollectorEngineOptions>()
                .Configure<IConfiguration>((settings, configuration) => { 
                    configuration.GetSection("RadiocomDataCollectorEngine").Bind(settings);
                });
            builder.Services
                .AddOptions()
                .AddOptions<SqlConnectionFactoryOptions>()
                .Configure<IConfiguration>((settings, configuration) => configuration.GetSection("SqlConnectionFactoryOptions").Bind(settings));


            builder.Services
                .AddOptions()
                .AddOptions<SqlRadiocomRepositoryOptions>()
                .Configure<IConfiguration>((settings, configuration) => configuration.GetSection("SqlRadiocomRepositoryOptions").Bind(settings));

            

            
        }
    }
}
