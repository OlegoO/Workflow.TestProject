using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.TestProject.Workflow
{
    public interface IWorkflowManager
    {
        WorkflowInfo GetWorkflow(string workflowId);
    }
}
