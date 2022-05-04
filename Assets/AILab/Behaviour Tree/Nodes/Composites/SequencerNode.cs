namespace AILab.BehaviourTree.Nodes
{
    public class SequencerNode : CompositeNode
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
                case NodeState.Failure:
                    state = NodeState.Failure;
                    break;
                case NodeState.Success:
                    index++;
                    state = index == children.Count ?
                        NodeState.Success : NodeState.Running;
                    break;
            }

            return state;
        }
    }
}