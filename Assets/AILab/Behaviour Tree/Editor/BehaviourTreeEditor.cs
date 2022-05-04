using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;

namespace AILab.BehaviourTree.EditorTools
{
    public class BehaviourTreeEditor : EditorWindow
    {
        private BehaviourTreeView treeView;
        private InspectorView inspectorView;
        private IMGUIContainer blackboardView;
        private ToolbarMenu toolbarMenu;

        private SerializedObject treeObject;
        private SerializedProperty blackboardProperty;

        [MenuItem("AILab/Behaviour Tree Editor")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("Behaviour Tree Editor");
            wnd.minSize = new Vector2(800, 600);
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if(Selection.activeObject is BehaviourTree)
            {
                OpenWindow();
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/AILab/Behaviour Tree/UI Builder/BehaviourTreeEditor.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/AILab/Behaviour Tree/UI Builder/BehaviourTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            // Tree view
            treeView = root.Q<BehaviourTreeView>();
            treeView.OnNodeSelected = OnNodeSelectionChanged;

            // Inspector view
            inspectorView = root.Q<InspectorView>();

            // Blackboard view
            blackboardView = root.Q<IMGUIContainer>();
            blackboardView.onGUIHandler = () =>
            {
                if (treeObject != null && treeObject.targetObject != null)
                {
                    treeObject.Update();
                    EditorGUILayout.PropertyField(blackboardProperty);
                    treeObject.ApplyModifiedProperties();
                }
            };

            // Toolbar assets menu
            toolbarMenu = root.Q<ToolbarMenu>();
            var behaviourTrees = LoadAssets<BehaviourTree>();
            behaviourTrees.ForEach(t =>
            {
                toolbarMenu.menu.AppendAction($"{t.name}", a => SelectTree(t));
            });

            if (treeView.tree == null)
            {
                OnSelectionChange();
            }
            else
            {
                SelectTree(treeView.tree);
            }
        }

        private List<T> LoadAssets<T>() where T : UnityEngine.Object
        {
            string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var assets = new List<T>();
            foreach(var id in assetIds)
            {
                string path = AssetDatabase.GUIDToAssetPath(id);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                assets.Add(asset);
            }

            return assets;
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void OnSelectionChange()
        {
            EditorApplication.delayCall += () =>
            {
                var tree = Selection.activeObject as BehaviourTree;
                if (!tree)
                {
                    if (Selection.activeGameObject)
                    {
                        var runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                        if (runner)
                        {
                            tree = runner.Tree;
                        }
                    }
                }

                SelectTree(tree);
            };
        }

        private void SelectTree(BehaviourTree newTree)
        {
            if (treeView == null) return;

            if (!newTree) return;

            if (Application.isPlaying)
            {
                treeView.PopulateView(newTree);
            }
            else
            {
                treeView.PopulateView(newTree);
            }

            treeObject = new SerializedObject(newTree);
            blackboardProperty = treeObject.FindProperty("blackboard");

            EditorApplication.delayCall += () => treeView.FrameAll();
        }

        private void OnNodeSelectionChanged(NodeView nodeView)
        {
            inspectorView.UpdateSelection(nodeView);
        }

        private void OnInspectorUpdate()
        {
            treeView?.UpdateNodeStates();
        }
    }
}