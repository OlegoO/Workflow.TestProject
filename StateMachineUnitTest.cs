using Workflow.TestProject.EventBus;
using Moq;
using Workflow.TestProject.Workflow;
using Workflow.TestProject.BusinessForms;
using VirtoCommerce.Platform.Core.Events;
using Workflow.TestProject.TaskAssignments;
using Newtonsoft.Json.Linq;
using Workflow.TestProject.StateMachine;

namespace Workflow.TestProject
{
    public class StateMachineUnitTest
    {
        IEventBusSubscriptionsManager _subscriptionsManager;
        IWorkflowManager _workflowManager;
        IEventPublisher _eventPublisher;
        IStateMachine _stateMachine;

        public string WorkflowIdTemplate { get; private set; }

        public StateMachineUnitTest()
        {
        }

        [Fact]
        public void InitialScenarios()
        {
            var stateMachineYaml = @"
            Entity: CustomerOrder
            EventId: VirtoCommerce.Order.CustomerOrderChanged
            InitialState: Pending
            States:
            - Value: Pending
                Transitions: Paid, Cancelled
            - Value: Paid
                Transitions: Confirmed, Cancelled
            - Value: Confirmed
                Transitions: Shipped
            - Value: Shipped
                Transitions: Completed
            - Value: Completed
                Transitions:
            - Value: Cancelled
                Transitions:
";

            _stateMachine.LoadStateMachine("CustomerOrder", stateMachineYaml);

            // TODO: Create a new CustomerOrder via CustomerOrder CRUD
            var customerOrderId = "54ED6B7F-EABF-47DB-8162-CBEF8A2DCC38";

            var state = _stateMachine.GetCurrentState("CustomerOrder", customerOrderId);
            Assert.Equal("Pending", state);

            // Can use this API to build UI
            var availabeTransitions = _stateMachine.GetTransitions("CustomerOrder", customerOrderId);
            Assert.Equal(2, availabeTransitions.Length);
            Assert.Equal("Paid", availabeTransitions[0]);
            Assert.Equal("Cancelled", availabeTransitions[1]);

            _stateMachine.ChangeState("CustomerOrder", customerOrderId, "Paid");
            var statePaid = _stateMachine.GetCurrentState("CustomerOrder", customerOrderId);
            Assert.Equal("Paid", statePaid);

            var history = _stateMachine.GetStateHistory("CustomerOrder", customerOrderId);
        }

        [Fact]
        public void WithPermissions()
        {
            var stateMachineYaml = @"
            Entity: CustomerOrder
            EventId: VirtoCommerce.Order.CustomerOrderChanged
            InitialState: Pending
            States:
            - Value: Pending
                Transitions: Paid, Cancelled
                Permission: ecommerce:ordermanager
            - Value: Paid
                Transitions: Confirmed, Cancelled
                Permission: vendor:manager
            - Value: Confirmed
                Transitions: Shipped
                Permission: vendor:manager
            - Value: Shipped
                Transitions: Completed
                 Permission: system:job, customer
            - Value: Completed
                Transitions:
            - Value: Cancelled
                Transitions:
";
        }


        [Fact]
        public void StartWorkflow()
        {
            // TODO: Define Create Temporal Connection String with Name VirtoCommerceTemporalWorkflow

            var stateMachineYaml = @"
            Entity: CompanyRegistration
            EventId: Workflow.TestProject.BusinessForms.BusinessFormSubmitedEvent

            Subscription: 
                Provider: TemporalWorkflow
                WorkflowId: {{type}}-{{form.companyName}}

            InitialState: WaitingForApprove
            States:
            - Value: WaitingForApprove
                Transitions: Approved, Cancelled
            - Value: Approved
                Transitions: Rejected
            - Value: Cancelled
                Transitions: 
                
 ";

            _stateMachine.LoadStateMachine("CompanyRegistration", stateMachineYaml);

            var companyRegistrationForm = new BusinessFormSubmitedEvent
            {
                Id = "54ED6B7F-EABF-47DB-8162-CBEF8A2DCC38",
                Type = "CompanyRegistration",
                Form = new JObject
                {
                    ["FirstName"] = "Ben",
                    ["LastName"] = "Black",
                    ["Email"] = "ben.black@virtoway.com",
                    ["CompanyName"] = "VirtoCommerce",
                    ["Phone"] = "+36586633256",
                    ["TermConditonsAccepted"] = true
                }
            };

            // Fire and Forget
            _eventPublisher.Publish(companyRegistrationForm);

            var state = _stateMachine.GetCurrentState("CompanyRegistration", "54ED6B7F-EABF-47DB-8162-CBEF8A2DCC38");
            Assert.Equal("WaitingForApprove", state);

            // Workflow Should be started on SubmitFormSubsription
            var workflowId = "CompanyRegistration-VirtoCommerce"; // Like WorkflowIdTemplate resolve it
            var workflowInfo = _workflowManager.GetWorkflow(workflowId);

            Assert.NotNull(workflowId);
            Assert.Equal("Running", workflowInfo.Status);

            // State Machine has been added to Workflow Context
            Assert.Equal(stateMachineYaml, workflowInfo.GetParameter(workflowId, "StateMachine"));
        }



        [Fact]
        public void CreateTaskAssignment()
        {
            // TODO: Define Create Temporal Connection String with Name VirtoCommerceTemporalWorkflow

            var stateMachineYaml = @"
            Entity: CompanyRegistration
            EventId: Workflow.TestProject.BusinessForms.BusinessFormSubmitedEvent, Workflow.TestProject.TaskAssignments.TaskCompletedEvent

            Subscription: 
                Provider: TemporalWorkflow
                ActionNameTemplate: {{type}}
                WorkflowIdTemplate: {{type}}-{{form.companyName}}

            InitialState: WaitingForApprove
            States:
            - Value: WaitingForApprove
                Transitions: Approved, Cancelled
                Actions:
                    CreateTaskAssignment
                        Type: ApproveDecline
                        Description: 'Review a new Company Registration request.'
                        AssignedTo: 'ozh@virtoway.com'  
                        ApproveTransition: Approved
                        DeclineTransition: Cancelled
            - Value: Approved
                Transitions: Rejected
            - Value: Cancelled
                Transitions: 
                
 ";

            _stateMachine.LoadStateMachine("CompanyRegistration", stateMachineYaml);

            var companyRegistrationForm = new BusinessFormSubmitedEvent
            {
                Id = "54ED6B7F-EABF-47DB-8162-CBEF8A2DCC38",
                Type = "CompanyRegistration",
                Form = new JObject
                {
                    ["FirstName"] = "Ben",
                    ["LastName"] = "Black",
                    ["Email"] = "ben.black@virtoway.com",
                    ["CompanyName"] = "VirtoCommerce",
                    ["Phone"] = "+36586633256",
                    ["TermConditonsAccepted"] = true
                }
            };

            // Fire and Forget
            _eventPublisher.Publish(companyRegistrationForm);

            var state = _stateMachine.GetCurrentState("CompanyRegistration", "54ED6B7F-EABF-47DB-8162-CBEF8A2DCC38");
            Assert.Equal("WaitingForApprove", state);

            // Workflow Should be started on SubmitFormSubsription
            var workflowId = "CompanyRegistration-VirtoCommerce"; // Like WorkflowIdTemplate resolve it
            var workflowInfo = _workflowManager.GetWorkflow(workflowId);

            Assert.NotNull(workflowId);
            Assert.Equal("Running", workflowInfo.Status);

            // State Machine has been added to Workflow Context
            Assert.Equal(stateMachineYaml, workflowInfo.GetParameter(workflowId, "StateMachine"));


            // TODO: Load Task
            object task = null;
            Assert.NotNull(task);

        }
    }
}