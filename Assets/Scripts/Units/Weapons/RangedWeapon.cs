using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ranged", menuName = "Weapon/Ranged", order = 0)]
public class RangedWeapon : BaseWeapon
{
    public int damage;
    public int minRange;
    public int maxRange;
}
