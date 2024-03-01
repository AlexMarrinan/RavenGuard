using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
public class UnitSelectionMenu : BaseMenu
{
    public List<Image> selectedUnitImages;
    private List<BaseUnit> possibleUnits = new();
    private List<BaseUnit> selectedUnits = new();
    public TMP_Text headerText;
    public UnitSummaryMenu unitSummary;
    public void SetUnits(){
        List<ScriptableUnit> scriptUnits = Resources.LoadAll<ScriptableUnit>("Units/C Term/Player Units").ToList();
        int index = 0;
        foreach (var unit in scriptUnits){
            BaseUnit bu = unit.unitPrefab;
            possibleUnits.Add(bu);
            var ibu = Instantiate(bu);
            ibu.InitUnit();
            buttons[index].image.sprite = ibu.spriteRenderer.sprite;
            index++;
        }
        buttonIndex = 0;
        unitSummary.SetUnit(possibleUnits[buttonIndex]);
        SetText();
        SetHighlight();
    }
    public override void Move(Vector2 direction)
    {
        base.Move(direction);
        unitSummary.SetUnit(possibleUnits[buttonIndex]);
        Debug.Log("moving!");
    }
    public override void Select()
    {
        base.Select();
        if (selectedUnits.Count >= 5){
            return;
        }
        BaseUnit unit = possibleUnits[buttonIndex];
        selectedUnitImages[selectedUnits.Count].sprite = unit.spriteRenderer.sprite;
        selectedUnits.Add(possibleUnits[buttonIndex]);
        //buttons[buttonIndex].SetOn(false);
        SetText();
    }
    private void SetText(){
        headerText.text = "Select " + (5-selectedUnits.Count) + " Units...";
    }
    public void UnselectUnit(){
        if (selectedUnits.Count <= 0){
            SceneManager.LoadScene("MainMenu");
            return;
        }
        int index = selectedUnits.Count-1;
        selectedUnitImages[index].sprite = null;
        selectedUnits.RemoveAt(selectedUnits.Count-1);
        SetText();
    }
    internal void ConfirmUnits()
    {
        foreach (BaseUnit u in selectedUnits){
            var newUnit = Instantiate(u, UnitManager.instance.transform);
            UnitManager.instance.units.Add(newUnit);
        }
        GameManager.instance.LoadNextLevel();
    }
}