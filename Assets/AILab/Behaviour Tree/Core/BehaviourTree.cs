using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AILab.BehaviourTree
{
    [CreateAssetMenu(menuName = "AILab/Behaviour Tree", fileName = "NewBehaviourTree")]
    public class BehaviourTree : ScriptableObject
    {
        public Node root;
        public NodeState treeState = NodeState.Running;
        public List<Node> nodes = new List<Node>();
        public Blackboard blackboard = new Blackboard();

        public NodeState Tick()
        {
            if(root.state == NodeState.Running)
            {
                treeState = root.Tick();
            }
            return treeState;
        }

        public List<Node> GetChildren(Node node)
        {
            var children = new List<Node>();

            var root = node as RootNode;
            if (root && root.child)
            {
                children.Add(root.child);
            }
            var decorator = node as DecoratorNode;
            if (decorator && decorator.child)
            {
                children.Add(decorator.child);
            }
            var composite = node as CompositeNode;
            if (composite && composite.children != null)
            {
                return composite.children;
            }

            return children;
        }

        public void Traverse(Node node, Action<Node> visiter)
        {
            if (node)
            {
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach(c => Traverse(c, visiter));
            }
        }

        public BehaviourTree Clone()
        {
            var tree = Instantiate(this);
            tree.root = tree.root.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.root, n => tree.nodes.Add(n));
            return tree;
        }

        public void Bind(Context context)
        {
            Traverse(root, n =>
            {
                n.context = context;
                n.blackboard = blackboard;
            });
        }

        #region Editor Tools
#if UNITY_EDITOR
        public Node CreateNode(Type type)
        {
            var node = CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour Tree (Create Node)");
            nodes.Add(node);

            // Can't parent node to the tree asset in play mode
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            
            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (Create Node)");

            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            if (node == root) root = null;

            Undo.RecordObject(this, "Behaviour Tree (Delete Node)");
            nodes.Remove(node);

            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            var root = parent as RootNode;
            if (root)
            {
                Undo.RecordObject(root, "Behaviour Tree (Add Child)");
                root.child = child;
                EditorUtility.SetDirty(root);
            }
            var decorator = parent as DecoratorNode;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (Add Child)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }
            var composite = parent as CompositeNode;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (Add Child)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            var root = parent as RootNode;
            if (root)
            {
                Undo.RecordObject(root, "Behaviour Tree (Remove Child)");
                root.child = null;
                EditorUtility.SetDirty(root);
            }
            var decorator = parent as DecoratorNode;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (Remove Child)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }
            var composite = parent as CompositeNode;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (Remove Child)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
            }
        }
#endif
        #endregion
    }
}