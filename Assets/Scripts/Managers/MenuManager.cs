using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [SerializeField] private GameObject highlightObject, selectedObject;
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
    public void EnableHighlight(Tile tile){
        if (!tile.isTerrainWalkable()){
            DisableHighlight();
            return;
        }
        highlightObject.transform.position = tile.transform.position;        
        highlightObject.SetActive(true);
    }
    public void DisableHighlight(){
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
