using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelupMenu : BaseMenu
{
    public TMP_Text atkT, defT, aglT, atuT, frsT, lckT, hpT;
    public TMP_Text atk2T, def2T, agl2T, atu2T, frs2T, lck2T, hp2T;
    public int atk, def, agl, atu, frs, lck, hp;
    public int atk2, def2, agl2, atu2, frs2, lck2, hp2;
    public TMP_Text levelText;
    private int addAmount = 3;
    private int remainingAdds = 2;
    private BaseUnit unit;
    private int hpDiff = 0;
    public override void Move(Vector2 direction)
    {
        base.Move(direction);
    }
    public void SetUnit(BaseUnit unit){
        this.unit = unit;
        atk = unit.GetBaseATK();
        def = unit.GetBaseDEF();
        agl = unit.GetBaseAGL();
        frs = unit.GetBaseFOR();
        atu = unit.GetBaseATU();
        lck = unit.GetBaseLCK();
        hp = unit.maxHealth;

        atkT.text = atk.ToString();
        defT.text = def.ToString();
        aglT.text = agl.ToString();
        atuT.text = atu.ToString();
        frsT.text = frs.ToString();
        lckT.text = lck.ToString();
        hpT.text = hp.ToString();

        atk2 = 0;
        def2 = 0;
        agl2 = 0;
        atu2 = 0;
        frs2 = 0;
        lck2 = 0;
        hp2 = 0;

        atk2T.text = "+0";
        def2T.text = "+0";
        agl2T.text = "+0";
        atu2T.text = "+0";
        frs2T.text = "+0";
        lck2T.text = "+0";
        hp2T.text  = "+0";
        levelText.text = "Lv. " + unit.level;
    }
    public override void Reset()
    {
        remainingAdds = 2;
        buttonIndex = 0;
        buttons.ForEach(b => b.SetOn(true));
        highlighImage.gameObject.SetActive(true);
        SetHighlight();
    }


    public override void Select(){
        if (remainingAdds <= 0){
            return;
        }
        addAmount = UnityEngine.Random.Range(1, 5);
        switch(buttonIndex){
            case 0: AddATK();
                break;
            case 1: AddDEF();
                break;
            case 2: AddAGL();
                break;
            case 3: AddATU();
                break;
            case 4: AddFRS();
                break;
            case 5: AddLCK();
                break;
            case 6: AddHP();
                break;
        }
        buttons[buttonIndex].SetOn(false);
        Move(new Vector2(0, -1));
        remainingAdds--;
        if (remainingAdds <= 0){
            StartCoroutine(FinishLevelup());
        }
    }

    private IEnumerator FinishLevelup()
    {
        buttons.ForEach(b => b.SetOn(false));
        highlighImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        atk += atk2;
        def += def2; 
        agl += agl2;
        frs += frs2; 
        atu += atu2; 
        lck += lck2;
        hp += hp2;

        atkT.text = atk.ToString();
        defT.text = def.ToString();
        aglT.text = agl.ToString();
        atuT.text = atu.ToString();
        frsT.text = frs.ToString();
        lckT.text = lck.ToString();
        hpT.text = hp.ToString();

        atk2T.text = "";
        def2T.text = "";
        agl2T.text = "";
        atu2T.text = "";
        frs2T.text = "";
        lck2T.text = "";
        hp2T.text = "";

        unit.SetBaseATK(atk);
        unit.SetBaseDEF(def);
        unit.SetBaseAGL(agl);
        unit.SetBaseATU(atu);
        unit.SetBaseFOR(frs);
        unit.SetBaseLCK(lck);
        unit.maxHealth = hp;
        unit.health += hpDiff;

        yield return new WaitForSeconds(1.5f);
        BattleSceneManager.instance.CloseBattleScene();
        MenuManager.instance.CloseMenus();
        if (UnitManager.instance.GetAllEnemies().Count <= 0){
            MenuManager.instance.ToggleLevelEndMenu();
        }
    }

    private void AddATK()
    {
        atk2 += addAmount;
        atk2T.text = "+" + atk2;
    }
    private void AddDEF()
    {
        def2 += addAmount;
        def2T.text = "+" + def2;
    }
    private void AddAGL()
    {
        agl2 += addAmount;
        agl2T.text = "+" + agl2;
    }
    private void AddATU()
    {
        atu2 += addAmount;
        atu2T.text = "+" + atu2;
    }
    private void AddFRS()
    {
        frs2 += addAmount;
        frs2T.text = "+" + frs2;
    }

    private void AddLCK()
    {
        lck2 += addAmount;
        lck2T.text = "+" + lck2;
    }
    private void AddHP()
    {
        addAmount = UnityEngine.Random.Range(3, 7);
        hp2 += addAmount;
        hpDiff = addAmount;
        hp2T.text = "+" + hp2;
    }
}
