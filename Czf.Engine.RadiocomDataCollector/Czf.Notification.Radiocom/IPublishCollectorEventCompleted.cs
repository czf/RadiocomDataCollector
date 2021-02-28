using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Engine.RadiocomDataCollector.Czf.Notification.Radiocom
{
    public interface IPublishCollectorEventCompleted
    {
        Task NotifyCollectorEventCompleted();
    }
}
