using UnityEngine;

namespace AILab.BehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField]
        private BehaviourTree tree;

        public BehaviourTree Tree => tree;

        private void Awake()
        {
            tree = tree.Clone();
            tree.Bind(GetComponent<Context>());
        }

        private void Update()
        {
            tree.Tick();
        }
    }
}