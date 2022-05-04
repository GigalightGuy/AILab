using UnityEngine;

namespace AILab.BehaviourTree.Nodes
{
    public class RepeatNode : DecoratorNode
    {
        [SerializeField, Min(-1)]
        private int reps = 5;

        private int count;

        protected override void OnStart()
        {
            count = 0;
        }

        protected override void OnStop()
        {
            
        }

        protected override NodeState OnTick()
        {
            if(child.Tick() != NodeState.Running)
            {
                count++;
                state = count == reps ? NodeState.Success : NodeState.Running;
            }

            return state;
        }
    }
}