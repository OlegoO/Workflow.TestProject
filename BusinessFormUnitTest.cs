using Workflow.TestProject.EventBus;
using Moq;
using Workflow.TestProject.Workflow;
using Workflow.TestProject.BusinessForms;
using VirtoCommerce.Platform.Core.Events;
using Workflow.TestProject.TaskAssignments;
using Newtonsoft.Json.Linq;

namespace Workflow.TestProject
{
    public class BusinessFormUnitTest
    {
        IEventBusSubscriptionsManager _subscriptionsManager;
        IWorkflowManager _workflowManager;
        IEventPublisher _eventPublisher;

        public string WorkflowIdTemplate { get; private set; }

        public BusinessFormUnitTest()
        {
            _subscriptionsManager = new Mock<IEventBusSubscriptionsManager>().Object;
            _workflowManager = new Mock<IWorkflowManager>().Object;
            _eventPublisher = new Mock<IEventPublisher>().Object;

            CreateTemporalConnection();
            AddSubmitFormSubsription();
            AddTaskCompletedSubsription();
        }


        public void CreateTemporalConnection()
        {
            var temporalConnection = new ProviderConnection
            {
                Name = "VirtoCommerceTemporalWorkflow",
                Provider = "Workflow",
                ConnectionString = "path=https://localhost;connectionType=Localhost;tlsEnabled=0;namespace=Namespace;serverCertAuthorityPemFilePath="
            };

            _subscriptionsManager.AddConnection(temporalConnection);
        }

        public void AddSubmitFormSubsription()
        {
            var submitFormSubscription = new SubscriptionInfo
            {
                ProviderConnectionName = "VirtoCommerceTemporalWorkflow",
                Configuration = new WorkflowConfiguration
                {
                    ActionType = WorkflowActionType.StartWorkflow,
                    ActionNameTemplate = "{{type}}",
                    WorkflowIdTemplate = "{{type}}-{{form.companyName}}" // Support Liqued Templates to resolve workflow id from event data

                },
                Events = new string[] { "Workflow.TestProject.BusinessForms.BusinessFormSubmitedEvent" }
            };

            _subscriptionsManager.AddSubscription(submitFormSubscription);
        }

        public void AddTaskCompletedSubsription()
        {
            var taskCompletedSubscription = new SubscriptionInfo
            {
                ProviderConnectionName = "VirtoCommerceTemporalWorkflow",
                Configuration = new WorkflowConfiguration
                {
                    ActionType = WorkflowActionType.SendSignal,
                    ActionNameTemplate = "{{entity.resultData.result}}", // Approved Or Declined
                    WorkflowIdTemplate = "{{entity.outerId}}" // Support Liqued Templates to resolve workflow id from event data

                },
                Events = new string[] { "Workflow.TestProject.TaskAssignments.TaskCompletedEvent" }
            };

            _subscriptionsManager.AddSubscription(taskCompletedSubscription);
        }



        [Fact]
        public void CustomerSubmitCompanyRegistrationForm()
        {
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
            _eventPublisher.Publish(companyRegistrationForm); // Event-Event
            // Or
            //_stateMachine.Run(companyRegistrationForm); // Run (BusinessForm/Dummy) - Event | Workflow Meta Data, States, UI
            // How to add a new workflow
            // Start workflow + state machine meta data

            // TODO: Sleep ???

            // Workflow Should be started on SubmitFormSubsription
            var workflowId = "CompanyRegistration-VirtoCommerce"; // Like WorkflowIdTemplate resolve it
            var workflowInfo = _workflowManager.GetWorkflow(workflowId);

            Assert.NotNull(workflowId);
            Assert.Equal("Running", workflowInfo.Status);
            Assert.Equal(workflowId, workflowInfo.Id);
            Assert.Equal("BusinessFormSubmitedEvent", workflowInfo.Name);

            // TODO: Workflow calls API and creates task
            var task = new TaskAssignment
            {
                Id = "{819F54D3-36C7-4828-B408-4F6772DC1114}",
                Type = "ApproveDecline",
                Description = "Review a new Company Registration request.",
                AssignedTo = "ozh@virtoway.com",
                OuterId = workflowId,
                InputData = JObject.FromObject(companyRegistrationForm)
            };
        }

        [Fact]
        public void StateMachine_StartWorkflow()
        {

        }


        [Fact]
        public void Manager_ApprovedRequest()
        {
            // Load Task
            var task = new TaskAssignment
            {
                Id = "{819F54D3-36C7-4828-B408-4F6772DC1114}",
                Type = "ApproveDecline",
                Description = "Review a new Company Registration request.",
                AssignedTo = "ozh@virtoway.com",
                OuterId = "BusinessFormSubmitedEvent-VirtoCommerce",
            };

            // Complete Tasks
            task.ResultData = new JObject
            {
                ["Result"] = "Approved",
            };

            // Fire and Forger
            _eventPublisher.Publish(new TaskCompletedEvent { Entity = task });


            // TODO: Sleep ???

            // Workflow Should be started on SubmitFormSubsription
            var workflowId = "BusinessFormSubmitedEvent-VirtoCommerce"; // Like WorkflowIdTemplate resolve it
            var workflowInfo = _workflowManager.GetWorkflow(workflowId);

            Assert.NotNull(workflowId);
            Assert.Equal("Completed", workflowInfo.Status);
        }

        [Fact]
        public void Manager_DeclineRequest()
        {
            // Load Task
            var task = new TaskAssignment
            {
                Id = "{819F54D3-36C7-4828-B408-4F6772DC1114}",
                Type = "ApproveDecline",
                Description = "Review a new Company Registration request.",
                AssignedTo = "ozh@virtoway.com",
                OuterId = "BusinessFormSubmitedEvent-VirtoCommerce",
            };

            // Complete Tasks
            task.ResultData = new JObject
            {
                ["Result"] = "Declined",
                ["Comment"] = "You did not signe the contract",
            };
            task.IsCompleted = true;

            // Fire and Forger
            _eventPublisher.Publish(new TaskCompletedEvent { Entity = task });

            // TODO: Sleep ???

            // Workflow Should be started on SubmitFormSubsription
            var workflowId = "BusinessFormSubmitedEvent-VirtoCommerce"; // Like WorkflowIdTemplate resolve it
            var workflowInfo = _workflowManager.GetWorkflow(workflowId);

            Assert.NotNull(workflowId);
            Assert.Equal("Completed", workflowInfo.Status);
        }
    }
}