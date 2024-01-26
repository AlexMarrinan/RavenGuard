using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[CustomEditor(typeof(LevelBase))]
public class PGEditor : Editor {
    //public DropdownField tileTypeField;
    private int drawLayerIndex = 0, tileTypeIndex = 0, layerSizeIndex = 0, teamSpanwIndex = 0;
    public VisualTreeAsset inspectorXML;
    public VisualElement myInspector;
    public Texture2D grass, water, forest, mountain, bridge, none, small, medium, large;
    public Texture2D orange, orangeRanged, orangeMelee; 
    public Texture2D blue, blueRanged, blueMelee; 
    public Dictionary<(TileEditorType, LayerSize), Texture2D> layerSizeTextures;
    public Dictionary<(TileEditorType, SpawnFaction), Texture2D> spawnFactionTextures;

    private void Awake() {
        GenerateOverlapTextures();
    }

    private void GenerateOverlapTextures()
    {
        layerSizeTextures = new();
        spawnFactionTextures = new();
        Debug.Log("Generating textures..");
        foreach (TileEditorType tileEditorType in Enum.GetValues(typeof(TileEditorType))){
            Texture2D tileTexture = TileTypeTexture(tileEditorType);
            foreach (LayerSize layerSize in Enum.GetValues(typeof(LayerSize))){
                Texture2D layerTexture = LayerSizeTexture(layerSize);
                Texture2D newTexture = OverlapTexture(tileTexture, layerTexture);
                layerSizeTextures.Add((tileEditorType, layerSize), newTexture);
            }
            foreach (SpawnFaction faction in Enum.GetValues(typeof(SpawnFaction))){
                Texture2D factionTexture = SpawnFactionTexture(faction);
                Texture2D newTexture = OverlapTexture(tileTexture, factionTexture);
                spawnFactionTextures.Add((tileEditorType, faction), newTexture);
            }
        }
    }
    private Texture2D TileTypeTexture(TileEditorType tileEditorType){
        switch (tileEditorType){
            case TileEditorType.None:
                return none;
            case TileEditorType.Grass:
                return grass;            
            case TileEditorType.Water:
                return water;            
            case TileEditorType.Forest:
                return forest;            
            case TileEditorType.Mountain:
                return mountain;            
            case TileEditorType.Bridge:
                return bridge;
        }
        return null;
    }
    //TODO: FIX X/Y DIMMENSIONS SWAPPED
    public override void OnInspectorGUI() {
        
        LevelBase b = (LevelBase)target;
        b.SetHeight(b.height);
        b.SetWidth(b.width);
        EditorUtility.SetDirty(b);
        if (GUILayout.Button("Resize")){
            b.Resize();
        }

        DrawDefaultInspector();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Draw Layer:");
        drawLayerIndex = EditorGUILayout.Popup(drawLayerIndex, Enum.GetNames(typeof(LEDrawLayer)));
        GUILayout.EndHorizontal();

        switch(drawLayerIndex){
            case 0:
                DrawStandard(b);
                break;
            case 1:
                DrawChests(b);
                break;
            case 2:
                DrawSpawn(b);
                break;
            default:
                break;
        }
    }
    private void DrawSpawn(LevelBase b)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Spawn Faction:");
        teamSpanwIndex = EditorGUILayout.Popup(teamSpanwIndex, Enum.GetNames(typeof(SpawnFaction)));
        GUILayout.EndHorizontal();
        var array =  b.spawnArray;
        for (int y = 0; y < b.array.Height; y++){
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            for (int x = 0; x < b.array.Width; x++){
                SpawnFaction faction = array.Get(x,y);
                TileEditorType tileType = b.array.Get(x,y);
                Texture2D texture2D = spawnFactionTextures[(tileType, faction)];

                if (GUILayout.Button(texture2D, GUILayout.Width(50), GUILayout.Height(50))){
                    var s = (SpawnFaction)teamSpanwIndex;
                    array.Set(x,y,s);
                    // GUILayout.BeginArea()
                }
                // GUILayout.EndArea();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
        }
    }

    private void DrawChests(LevelBase b)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Layer Size:");
        layerSizeIndex = EditorGUILayout.Popup(layerSizeIndex, Enum.GetNames(typeof(LayerSize)));
        GUILayout.EndHorizontal();
        var array = b.chestArray;
        for (int y = 0; y < array.Height; y++){
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            for (int x = 0; x < array.Width; x++)
            {
                LayerSize size = array.Get(x, y);
                TileEditorType tileType = b.array.Get(x,y);
                Texture2D texture2D = layerSizeTextures[(tileType, size)];
            
                if (GUILayout.Button(texture2D, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    var l = (LayerSize)layerSizeIndex;
                    array.Set(x, y, l);
                    Debug.Log((x, y));
                    Debug.Log(l);
                    // GUILayout.BeginArea()
                }
                // GUILayout.EndArea();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
        }
    }
    private Texture2D SpawnFactionTexture(SpawnFaction faction)
    {
        switch (faction)
        {
            case SpawnFaction.None:
                return none;
            case SpawnFaction.BlueMelee:
                return blueMelee;
            case SpawnFaction.BlueRanged:
                return blueRanged;
            case SpawnFaction.BlueEither:
                return blue;
            case SpawnFaction.OrangeMelee:
                return orangeMelee;
            case SpawnFaction.OrangeRanged:
                return orangeRanged;
            case SpawnFaction.OrangeEither:
                return orange;
        }
        return null;;
    }
    private Texture2D LayerSizeTexture(LayerSize size)
    {
        switch (size)
        {
            case LayerSize.None:
                return none;
            case LayerSize.Small:
                return small;
            case LayerSize.Medium:
                return medium;
            case LayerSize.Large:
                return large;
        }
        return null;;
    }

    private void DrawStandard(LevelBase b)
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
                Texture texture = TileTypeTexture(tileType);
                //Texture2D texture2D = OverlapTexture(texture as Texture2D, none as Texture2D);

                GUIStyle style = new GUIStyle
                {
                    stretchHeight = true,
                    stretchWidth = true
                };
                // GUIContent content = new GUIContent()
                // GUILayout.BeginArea(new Rect(i*50, y*50, 50, 50));
                // GUILayout.Box(none, GUILayout.Width(100));
                if (GUILayout.Button(texture, GUILayout.Width(50), GUILayout.Height(50))){
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

    public Texture2D OverlapTexture(Texture2D background, Texture2D overlay)
    {      
        var newbg = Instantiate(new Texture2D(background.width, background.height));

        //Deep copy to newbg, so that the old textures are not overwritten
        for (int x = 0; x < background.width; x++)
        {

            for (int y = 0; y < background.height; y++)
            {
                Color bgColor = background.GetPixel(x, y);
                bgColor.a = 0.4f;
                newbg.SetPixel(x, y, bgColor);
            }
        }

        int startX = 0;
        int startY = background.height - overlay.height;

        for (int x = startX; x < newbg.width; x++)
        {

            for (int y = startY; y < newbg.height; y++)
            {
                Color bgColor = newbg.GetPixel(x, y);
                Color wmColor = overlay.GetPixel(x - startX, y - startY);

                Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                newbg.SetPixel(x, y, final_color);
            }
        }

        newbg.Apply();
        return newbg;
    }
}
