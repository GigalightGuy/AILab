using UnityEngine;

namespace AILab.BehaviourTree
{
    public enum NodeState
    {
        Running,
        Failure,
        Success
    }

    public abstract class Node : ScriptableObject
    {
        [TextArea]
        public string description;

        [HideInInspector]
        public NodeState state = NodeState.Running;
        [HideInInspector]
        public bool isStarted = false;
        [HideInInspector]
        public Vector2 position;
        [HideInInspector]
        public string guid;
        [HideInInspector]
        public Blackboard blackboard;
        [HideInInspector]
        public Context context;

        public NodeState Tick()
        {
            if (!isStarted)
            {
                isStarted = true;
                OnStart();
            }

            state = OnTick();

            if(state != NodeState.Running)
            {
                OnStop();
                isStarted = false;
            }

            return state;
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract NodeState OnTick();
    }
}