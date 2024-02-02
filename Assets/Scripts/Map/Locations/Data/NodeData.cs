using UnityEngine;

namespace Assets.Scripts.Map.Locations.Data
{
    [CreateAssetMenu(fileName = "Node", menuName = "Game/Map/Node")]
    public class NodeData:ScriptableObject
    {
        public Sprite nodeSprite;
    }
}