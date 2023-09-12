using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [SerializeField] public GameObject highlightObject, selectedObject;
    public TextMeshProUGUI turnStartText;
    public UnitActionMenu unitActionMenu;
    public bool inUnitMenu = false;
    private int textFrames = 0;
    //public int textFramesBeginFadeout = 30;
    public int textFramesMax = 120;
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
   private void FixedUpdate() {
        if (textFrames <= 0){
            turnStartText.alpha -= 0.05f;
            return;
        }
        textFrames--;
    }
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

    public void ShowStartText(string text){
        turnStartText.text = text;
        turnStartText.alpha = 1.0f;
        textFrames = textFramesMax;
        turnStartText.transform.SetAsLastSibling();
    }

    public void ToggleUnitMenu(){
        if (inUnitMenu){
            unitActionMenu.gameObject.SetActive(false);
            inUnitMenu = false;
            return;
        }
        if (UnitManager.instance.selectedUnit == null){
            return;
        }
        unitActionMenu.Reset();
        unitActionMenu.gameObject.SetActive(true);
        inUnitMenu = true;
    }

    public void Move(Vector2 direction){
        if (!inUnitMenu){
            return;
        }
        unitActionMenu.Move(direction);
    }

    internal void SelectUnitMenuButton()
    {
        unitActionMenu.Select();
    }
}
