using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero1 : BaseHero
{

    public int damage = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack(BaseUnit otherUnit){
        otherUnit.ReceiveDamage(damage);
        if (otherUnit.health <= 0){
            UnitManager.instance.DeleteUnit(otherUnit);
            MoveToSelectedTile(otherUnit.occupiedTile);
        }else{
            MoveToClosestTile(otherUnit.occupiedTile);
        }
        UnitManager.instance.SetSeclectedUnit(null);
    }
}
