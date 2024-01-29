using UnityEditor;
using UnityEngine;

namespace Editor.Inspector
{
    [CustomEditor(typeof(SkillProgressionGroup))]
    public class SkillProgressionGroupInspector:UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            SkillProgressionGroup element = target as SkillProgressionGroup;
            
            if (GUILayout.Button("Init"))
            {
                element.SetReferences();
                
                // Save the asset
                EditorUtility.SetDirty(element);
                AssetDatabase.SaveAssets();
            }
        }
    }
}