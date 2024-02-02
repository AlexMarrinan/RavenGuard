using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map.Locations
{
    public class MapLevel : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private RectTransform rectTransform;
        [Header("Prefabs")] 
        [SerializeField] private MapNode mapNodePrefab;
        //Internal
        private List<MapNode> nodes = new List<MapNode>();

        public void Init(int roomsPerLevel, float width, float height, Vector2 position)
        {
            for (int i = 0; i < roomsPerLevel; i++)
            {
                MapNode node=Instantiate(mapNodePrefab, transform);
                transform.position = position;
                rectTransform.sizeDelta = new Vector2(width,height);
                node.Init(null);
                nodes.Add(node);
            }
        }
    }
}

