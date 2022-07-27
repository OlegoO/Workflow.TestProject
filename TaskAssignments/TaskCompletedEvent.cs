using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Events;

namespace Workflow.TestProject.TaskAssignments
{
    public class TaskCompletedEvent: DomainEvent
    {
        public TaskAssignment Entity { get; set; }
    }
}
