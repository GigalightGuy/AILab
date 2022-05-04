using UnityEngine;

namespace AILab.BehaviourTree.Nodes
{
    public class WaitNode : ActionNode
    {
        [SerializeField, Min(0.0f)]
        private float delay = 1.0f;

        private float timer;

        protected override void OnStart()
        {
            timer = Time.time + delay;
        }

        protected override void OnStop()
        {
            
        }

        protected override NodeState OnTick()
        {
            if (timer < Time.time) return NodeState.Success;
            return NodeState.Running;
        }
    }
}