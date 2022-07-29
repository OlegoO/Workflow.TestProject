using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Events;

namespace Workflow.TestProject.BusinessForms
{
    public class BusinessFormSubmitedEvent: DomainEvent
    {
        public string Type { get; set; }
        public JObject Form { get; set; }
    }
}
