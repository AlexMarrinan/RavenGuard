using Game.UI.Element;
using UnityEditor;
using UnityEngine;

namespace Editor.Inspector
{
    [CustomEditor(typeof(UIElement),true)]
    public class UIElementInspector:UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            UIElement element = target as UIElement;
            
            if (GUILayout.Button("Init"))
            {
                element.Init();
                
                // Save the asset
                EditorUtility.SetDirty(element);
                AssetDatabase.SaveAssets();
            }
        }
    }
}