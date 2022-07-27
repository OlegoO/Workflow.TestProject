using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflow.TestProject.Workflow;

namespace Workflow.TestProject.EventBus
{
    public class SubscriptionInfo 
    {
        public string Id { get; set; }
        public string ProviderConnectionName { get; set; }

        public string JsonPathFilter { get; set; }

        public string PayloadTransformationTemplate { get; set; }

        public string SettingsTemplate { get; set; }

        public string[] Events { get; set; }
        public WorkflowConfiguration Configuration { get; internal set; }
    }
}
