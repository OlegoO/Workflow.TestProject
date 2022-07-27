using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.TestProject.EventBus
{
    public interface IEventBusSubscriptionsManager
    {
        void AddConnection(ProviderConnection connection);

        void AddSubscription(SubscriptionInfo subscription);
    }
}
