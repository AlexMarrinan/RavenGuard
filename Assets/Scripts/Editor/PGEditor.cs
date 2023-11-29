using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(PGBase))]
public class PGEditor : Editor {
    //public DropdownField tileTypeField;
    private int tileTypeIndex = 0;
    public VisualTreeAsset inspectorXML; 
    public VisualElement myInspector;
    public Texture grass, water, forest, mountain, bridge, none;
    // public override VisualElement CreateInspectorGUI() {
    //     // Create a new VisualElement to be the root of our inspector UI
    //     myInspector = new VisualElement();

    //     // Load from default reference
    //     inspectorXML.CloneTree(myInspector);
    //     tileTypeField = myInspector.Q<DropdownField>(); //or VisualElement result = root.Q("OK");
    //     var tileTypes = Enum.GetValues(typeof(TileEditorType)).Cast<TileEditorType>().ToList();
        
    //     tileTypeField.choices = tileTypes.ConvertAll(t => t.ToString());
    //     tileTypeField.index = 0;

    //     return myInspector;
    // }

    public override void OnInspectorGUI() {

        PGBase b = (PGBase)target;
        if (GUILayout.Button("Resize")){
            b.grid = new TileEditorType[b.height * b.width];
        }
        // // if (oldX == 0 && oldY == 0){
        // //     oldX = b.width; oldY = b.height;
        // // }
        // // if (oldX != b.width || oldY != b.height){
        // //     oldX = b.width;
        // //     oldY = b.height;
        // // }
        // if (GUIBu)
        //     b.grid = new TileEditorType[b.height, b.width];

        DrawDefaultInspector();
        tileTypeIndex = EditorGUILayout.Popup(tileTypeIndex, Enum.GetNames(typeof(TileEditorType)));
        for (int i = 0, x = 0; x < b.height; x++){
            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            for (int y = 0; y < b.width; y++)
            { 
                TileEditorType tileType = b.grid[i];
                Texture texture = grass;
                switch (tileType){
                    case TileEditorType.Grass:
                        texture = grass;
                        break;
                    case TileEditorType.Water:
                        texture = water;
                        break;
                    case TileEditorType.Mountain:
                        texture = mountain;
                        break;         
                    case TileEditorType.Forest:
                        texture = forest;
                        break;              
                    case TileEditorType.Bridge:
                        texture = bridge;
                        break;      
                    case TileEditorType.None:
                        texture = none;
                        break;           
                }
                GUIStyle style = new GUIStyle
                {
                    stretchHeight = true,
                    stretchWidth = true
                };
                if (GUILayout.Button(texture, GUILayout.Width(50), GUILayout.Height(50))){
                    var t = (TileEditorType)tileTypeIndex;
                    b.grid[i] = t;
                }
                i++;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();

        }
    }
}
