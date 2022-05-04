namespace AILab.BehaviourTree.Nodes
{
    public class SelectorNode : CompositeNode
    {
        private int index;

        protected override void OnStart()
        {
            index = 0;
        }

        protected override void OnStop()
        {
            
        }

        protected override NodeState OnTick()
        {
            switch (children[index].Tick())
            {
                case NodeState.Running:
                    state = NodeState.Running;
                    break;
                case NodeState.Success:
                    state = NodeState.Success;
                    break;
                case NodeState.Failure:
                    index++;
                    state = index == children.Count ?
                        NodeState.Failure : NodeState.Running;
                    break;
            }

            return state;
        }
    }
}