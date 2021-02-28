using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;

namespace Czf.Engine.RadiocomDataCollector.Czf.Notification.Radiocom
{
    /// <summary>
    /// Used for local testing instead of AzureStorageQueuePublishCollectorEventCompleted
    /// </summary>
    public class LocalStorageQueuePublishCollectorEventCompleted : IPublishCollectorEventCompleted
    {
        private const string COLLECTOR_EVENT_MESSAGE = "COMPLETED";
        private readonly QueueClient _client;

        public LocalStorageQueuePublishCollectorEventCompleted()
        {
            _client = new QueueClient("http://127.0.0.1:10001/", "Collector");
        }

        public Task NotifyCollectorEventCompleted()
        {
            try
            {
                return _client.SendMessageAsync(COLLECTOR_EVENT_MESSAGE);
            }
            catch
            {
                return Task.CompletedTask;
            }
        }
    }
}
