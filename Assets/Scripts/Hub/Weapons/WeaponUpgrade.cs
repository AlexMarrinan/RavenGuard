using System.Collections.Generic;
using Hub.UI;
using TMPro;
using UnityEngine;

namespace Hub.Weapons
{
    public class WeaponUpgrade:MonoBehaviour
    {
        [SerializeField] private List<StatUI> stats = new List<StatUI>();
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private TextMeshProUGUI desc;
        [SerializeField] private TextMeshProUGUI cost;
        
        public void Init(WeaponType weapon)
        {
            name.text = weapon.weapon.weaponName;
            desc.text = weapon.weapon.weaponDescription;
            cost.text = weapon.cost.ToString();
            LoadStats(weapon.weapon);
        }

        private void LoadStats(BaseWeapon weapon)
        {
            List<Sprite> statIcons = weapon.statIcons;
            for (int i = 0; i < stats.Count; i++)
            {
                stats[i].gameObject.SetActive(i < statIcons.Count);
                if (i < statIcons.Count)
                {
                    stats[i].Init(statIcons[i]);
                }
            }
        }
    }
}