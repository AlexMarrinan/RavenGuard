using Hub.Weapons;
using UnityEditor;
using UnityEngine;

namespace Editor.Inspector
{
    [CustomEditor(typeof(WeaponsInventoryView))]
    public class WeaponInventoryInspector:UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            WeaponsInventoryView element = target as WeaponsInventoryView;
            
            if (GUILayout.Button("Toggle Show"))
            {
                element.ToggleUINoTween();
            }
        }
    }
}