using Game.Dialogue;
using UnityEditor;
using UnityEngine;

namespace Editor.Tools.Inspector
{
    [CustomEditor(typeof(SpeakerData))]
    public class SpeakerDataInspector:UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            SpeakerData speakerData=target as SpeakerData;
            
            if (GUILayout.Button("Parse Yarn Scripts"))
            {
                speakerData.ParseYarnScripts();
                
                // Save the asset
                EditorUtility.SetDirty(speakerData);
                AssetDatabase.SaveAssets();
            }
        }
    }
}