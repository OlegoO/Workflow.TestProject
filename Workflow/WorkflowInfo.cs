namespace Workflow.TestProject.Workflow
{
    public class WorkflowInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string Status { get; set; }

        internal IEnumerable<char> GetParameter(string workflowId, string v)
        {
            throw new NotImplementedException();
        }
    }
}