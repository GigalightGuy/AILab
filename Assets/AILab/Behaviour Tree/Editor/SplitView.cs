using UnityEngine.UIElements;

namespace AILab.BehaviourTree.EditorTools
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
    }
}
