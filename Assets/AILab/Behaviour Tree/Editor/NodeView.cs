using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace AILab.BehaviourTree.EditorTools
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Node node;
        public Port input;
        public Port output;

        public Action<NodeView> OnNodeSelected;

        public NodeView(Node node) : base("Assets/AILab/Behaviour Tree/UI Builder/NodeView.uxml")
        {
            this.node = node;
            title = node.name;
            viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();

            SetUpClasses();

            var descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(new SerializedObject(node));
        }

        private void SetUpClasses()
        {
            if(node is ActionNode)
            {
                AddToClassList("action");
            }else if(node is DecoratorNode)
            {
                AddToClassList("decorator");
            }else if(node is CompositeNode)
            {
                AddToClassList("composite");
            }else if(node is RootNode)
            {
                AddToClassList("root");
            }
        }

        private void CreateInputPorts()
        {
            if(node is ActionNode)
            {
                input = new NodePort(Direction.Input, Port.Capacity.Single);
            }else if(node is DecoratorNode)
            {
                input = new NodePort(Direction.Input, Port.Capacity.Single);
            }
            else if(node is CompositeNode)
            {
                input = new NodePort(Direction.Input, Port.Capacity.Single);
            }

            if(input != null)
            {
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts()
        {
            if (node is RootNode)
            {
                output = new NodePort(Direction.Output, Port.Capacity.Single);
            }
            else if (node is DecoratorNode)
            {
                output = new NodePort(Direction.Output, Port.Capacity.Single);
            }
            else if (node is CompositeNode)
            {
                output = new NodePort(Direction.Output, Port.Capacity.Multi);
            }

            if (output != null)
            {
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(output);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Behaviour Tree (Set Position)");
            node.position.x = newPos.x;
            node.position.y = newPos.y;
            EditorUtility.SetDirty(node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public void SortChildren()
        {
            var composite = node as CompositeNode;
            if (composite)
            {
                composite.children.Sort(SortByHorizontalPosition);
            }
        }

        private int SortByHorizontalPosition(Node left, Node right)
        {
            return left.position.x < right.position.x ? -1 : 1;
        }

        public void UpdateState()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            if (Application.isPlaying)
            {
                switch (node.state)
                {
                    case NodeState.Running:
                        if (node.isStarted)
                            AddToClassList("running");
                        break;
                    case NodeState.Failure:
                        AddToClassList("failure");
                        break;
                    case NodeState.Success:
                        AddToClassList("success");
                        break;
                }
            }
        }
    }
}