namespace Workflow.TestProject.Workflow
{
    public enum WorkflowActionType
    {
        StartWorkflow,
        SendSignal
    }

    public class WorkflowConfiguration
    {
        public WorkflowActionType ActionType { get; set; }
        public string WorkflowIdTemplate { get; internal set; }
        public string ActionNameTemplate { get; internal set; }
    }
}