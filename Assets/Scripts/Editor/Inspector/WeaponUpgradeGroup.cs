using UnityEditor;
using UnityEngine;
using Weapons;

namespace Editor.Inspector
{
    [CustomEditor(typeof(WeaponUpgradeGroup))]
    public class WeaponUpgradeGroupInspector:UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            WeaponUpgradeGroup element = target as WeaponUpgradeGroup;
            
            if (GUILayout.Button("Setup References"))
            {
                element.SetWeaponReferences();
            }
        }
    }
}