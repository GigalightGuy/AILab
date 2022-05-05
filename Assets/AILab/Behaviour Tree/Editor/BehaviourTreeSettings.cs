using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace AILab.BehaviourTree.EditorTools
{
    public class BehaviourTreeSettings : ScriptableObject
    {
        public VisualTreeAsset behaviourTreeXml;
        public StyleSheet behaviourTreeStyle;
        public VisualTreeAsset nodeXml;
        public TextAsset scriptTemplateActionNode;
        public TextAsset scriptTemplateCompositeNode;
        public TextAsset scriptTemplateDecoratorNode;
        public string newNodeBasePath = "Assets/";

        public static BehaviourTreeSettings FindSettings()
        {
            var guids = AssetDatabase.FindAssets("t:BehaviourTreeSettings");
            if (guids.Length == 0) return null;
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<BehaviourTreeSettings>(path);
        }

        internal static BehaviourTreeSettings GetOrCreateSettings()
        {
            var settings = FindSettings();
            if(settings == null)
            {
                settings = CreateInstance<BehaviourTreeSettings>();
                AssetDatabase.CreateAsset(settings, "Assets/AILab/Behaviour Tree/Settings");
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
    }
}