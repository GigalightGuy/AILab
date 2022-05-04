using System.Collections.Generic;
using UnityEngine;

namespace AILab.BehaviourTree
{
    public abstract class CompositeNode : Node
    {
        [HideInInspector]
        public List<Node> children = new List<Node>();

        public override Node Clone()
        {
            var node = Instantiate(this);
            node.children = new List<Node>();
            node.children = children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}