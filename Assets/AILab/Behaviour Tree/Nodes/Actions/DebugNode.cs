using UnityEngine;

namespace AILab.BehaviourTree.Nodes
{
    public class DebugNode : ActionNode
    {
        [SerializeField]
        private string message = "Tehe";

        protected override void OnStart()
        {
            Debug.Log($"On Start: {message}");
        }

        protected override void OnStop()
        {
            Debug.Log($"On Stop: {message}");
        }

        protected override NodeState OnTick()
        {
            Debug.Log($"On Tick: {message}");
            return NodeState.Success;
        }
    }
}