using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Queues;
using Microsoft.Extensions.Options;

namespace Czf.Engine.RadiocomDataCollector.Czf.Notification.Radiocom
{
    public class AzureStorageQueuePublishCollectorEventCompleted : IPublishCollectorEventCompleted
    {
        private const string COLLECTOR_EVENT_MESSAGE = "COMPLETED";
        private readonly QueueClient _client;


        public AzureStorageQueuePublishCollectorEventCompleted(IOptions<AzureStorageQueuePublishCollectorEventCompletedOptions> options)
        {
            
            
            
            
            
            if(options == null)
            {
                throw new ArgumentNullException("options is null");
            }
            
            
            
            
            
            
            Uri queueUri = new Uri(options.Value.QueueUri);
            _client = new QueueClient(queueUri, new DefaultAzureCredential());
        }

        public Task NotifyCollectorEventCompleted()
        {
            try
            {
                return _client.SendMessageAsync(Base64Encode(COLLECTOR_EVENT_MESSAGE));
            }
            catch
            {
                return Task.CompletedTask;
            }
        }

        public class AzureStorageQueuePublishCollectorEventCompletedOptions
        {
            public const string AzureStorageQueuePublishCollectorEventCompleted = "AzureStorageQueuePublishCollectorEventCompletedOptions";
            public string QueueUri { get; set; }
        }
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
