using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.TestProject.TaskAssignments
{
    public class TaskAssignment
    {
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;

        public string Type { get; set; }

        public string OuterId { get; set; }

        public string AssignedTo { get; set; }

        public string Id { get; internal set; }

        public string Comment { get; set; }

        public JObject InputData { get; set; }

        public JObject ResultData { get; set; }
    }
}
