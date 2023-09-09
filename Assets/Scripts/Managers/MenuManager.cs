using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [SerializeField] public GameObject highlightObject, selectedObject;

    public Color moveColor, attackColor, inRangeColor, supportColor;
    public void Awake(){
        instance = this;
    }
    // public void ShowSelectedUnit(BaseUnit unit){
    //     if (unit == null){
    //         selectedHeroObject.SetActive(false);
    //         return;
    //     }
    //     selectedHeroObject.GetComponentInChildren<Text>().text = unit.unitName;
    //     selectedHeroObject.SetActive(true);
    // }

    // public void ShowTileInfo(Tile tile){
    //     selectedHeroObject.GetComponentInChildren<Text>().text = unit.unitName;
    //     selectedHeroObject.SetActive(true);
    // }
    public void HighlightTile(Tile tile){
        if (!tile.isTerrainWalkable()){
            UnhighlightTile();
            return;
        }
        highlightObject.transform.position = tile.transform.position;        
        highlightObject.SetActive(true);
        
        if (UnitManager.instance.selectedUnit == null){
            GameManager.instance.LookCameraAtHighlight();
        }
    }
    public void UnhighlightTile(){
        highlightObject.SetActive(false);
    }

    public void SelectTile(Tile tile){
        if (!tile.isTerrainWalkable()){
            UnselectTile();
            return;
        }
        selectedObject.transform.position = tile.transform.position;
        selectedObject.SetActive(true);
    }
    public void UnselectTile(){
        selectedObject.SetActive(false);
    }
}
