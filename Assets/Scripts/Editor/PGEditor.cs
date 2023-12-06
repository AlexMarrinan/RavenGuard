using System;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(PGBase))]
public class PGEditor : Editor {
    //public DropdownField tileTypeField;
    private int drawLayerIndex = 0, tileTypeIndex = 0, layerSizeIndex = 0;
    public VisualTreeAsset inspectorXML;
    public VisualElement myInspector;
    public Texture grass, water, forest, mountain, bridge, none, small, medium, large;


    //TODO: FIX X/Y DIMMENSIONS SWAPPED
    public override void OnInspectorGUI() {
        
        PGBase b = (PGBase)target;
        b.SetHeight(b.height);
        b.SetWidth(b.width);
        EditorUtility.SetDirty(b);
        if (GUILayout.Button("Resize")){
            b.Resize();
        }

        DrawDefaultInspector();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Draw Layer:");
        drawLayerIndex = EditorGUILayout.Popup(drawLayerIndex, Enum.GetNames(typeof(PGDrawLayer)));
        GUILayout.EndHorizontal();

        switch(drawLayerIndex){
            case 0:
                DrawStandard(b);
                break;
            case 1:
                DrawLayer(b, b.riverArray);
                break;
            case 2:
                DrawLayer(b, b.pondArray);
                break;
            default:
                break;
        }
    }

    private void DrawLayer(PGBase b, Array2D<LayerSize> array)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Layer Size:");
        layerSizeIndex = EditorGUILayout.Popup(layerSizeIndex, Enum.GetNames(typeof(LayerSize)));
        GUILayout.EndHorizontal();

        for (int y = 0; y < array.Height; y++){
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            for (int x = 0; x < array.Width; x++){
                LayerSize size = array.Get(x,y);
                Texture texture = grass;

                switch (size){
                    case LayerSize.None:
                        texture = none;
                        break;
                    case LayerSize.Small:
                        texture = small;
                        break;
                    case LayerSize.Medium:
                        texture = medium;
                        break;
                    case LayerSize.Large:
                        texture = large;
                        break;
                }
                Texture2D texture2D = OverlapTexture(texture as Texture2D, none as Texture2D);

                // GUIContent content = new GUIContent()
                // GUILayout.BeginArea(new Rect(i*50, y*50, 50, 50));
                // GUILayout.Box(none, GUILayout.Width(100));
                if (GUILayout.Button(texture2D, GUILayout.Width(50), GUILayout.Height(50))){
                    var l = (LayerSize)layerSizeIndex;
                    array.Set(x,y,l);
                    Debug.Log((x,y));
                    Debug.Log(l);
                    // GUILayout.BeginArea()
                }
                // GUILayout.EndArea();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
        }
    }

    private void DrawStandard(PGBase b)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Brush Tile Type:");
        tileTypeIndex = EditorGUILayout.Popup(tileTypeIndex, Enum.GetNames(typeof(TileEditorType)));
        GUILayout.EndHorizontal();
        // Debug.Log((b.width, b.height));
        for (int y = 0; y < b.height; y++){
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            for (int x = 0; x < b.width; x++)
            { 
                TileEditorType tileType = b.GetTileType(x,y);
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
                Texture2D texture2D = OverlapTexture(texture as Texture2D, none as Texture2D);

                GUIStyle style = new GUIStyle
                {
                    stretchHeight = true,
                    stretchWidth = true
                };
                // GUIContent content = new GUIContent()
                // GUILayout.BeginArea(new Rect(i*50, y*50, 50, 50));
                // GUILayout.Box(none, GUILayout.Width(100));
                if (GUILayout.Button(texture2D, GUILayout.Width(50), GUILayout.Height(50))){
                    var t = (TileEditorType)tileTypeIndex;
                    b.array.Set(x,y,t);
                    Debug.Log((x,y));
                    // GUILayout.BeginArea()
                }
                // GUILayout.EndArea();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
        }
    }

    public Texture2D OverlapTexture(Texture2D background, Texture2D watermark)
{      
    return background;
    // int startX = 0;
    // int startY = background.height - watermark.height;

    // for (int x = startX; x < background.width; x++)
    // {

    //     for (int y = startY; y < background.height; y++)
    //     {
    //         Color bgColor = background.GetPixel(x, y);
    //         Color wmColor = watermark.GetPixel(x - startX, y - startY);

    //         Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

    //         //background.SetPixel(x, y, final_color);
    //     }
    // }

    // // background.Apply();
    // return background;
}
}
