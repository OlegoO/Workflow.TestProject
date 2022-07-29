using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.TestProject.StateMachine
{
    public interface IStateMachine
    {
        void LoadStateMachine(string entityType, string stateMachineYaml);
        string GetCurrentState(string entityType, string id);
        string[] GetTransitions(string entityType, string id);
        void ChangeState(string entityType, string id, string newState);
        object GetStateHistory(string entityType, string id);
    }
}
