using UnityEngine;

namespace AILab.BehaviourTree
{
    public class RootNode : Node
    {
        [HideInInspector]
        public Node child;

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        protected override NodeState OnTick()
        {
            return child.Tick();
        }

        public override Node Clone()
        {
            var node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}