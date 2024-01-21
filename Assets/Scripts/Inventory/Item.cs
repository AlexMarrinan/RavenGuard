using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Item", order = 0)]
    public class Item:ScriptableObject
    {
        public string itemName;
        public Sprite itemIcon;
        public List<Sprite> skillIcons;
        public string desc;
    }
}