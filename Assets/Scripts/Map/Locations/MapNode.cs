using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Map.Locations.Data;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Map.Locations
{
    public class MapNode : MonoBehaviour
    {
        public List<MapNode> closestNodes=null;
        public bool hasPath;
        public MapLevel mapLevel;
        
        [Header("References")] 
        [SerializeField] public Image image;
        [SerializeField] private UILine linePrefab;
        
        //Internal
        private bool loadedNodes;
        private NodeData data;
        private bool isSelected;
        
        public void Init(MapLevel level, int num, NodeData nodeData)
        {
            name = level.name + " Map Node "+num;
            mapLevel = level;
            data = nodeData;
            if (data == null) return;
            image.sprite = data.nodeSprite;
        }

        public List<MapNode> GetNextClosestNodes(int nodeCount=2)
        {
            if (!loadedNodes)
            {
                closestNodes = LoadClosestNodes(nodeCount);
            }
            return closestNodes;
        }

        private List<MapNode> LoadClosestNodes(int num)
        {
            loadedNodes = true;
            List<MapNode> nextLevelNodes=mapLevel.GetNextLevelNodes();
            List<MapNode> nodes = nextLevelNodes
                .OrderBy((node) => (node.transform.position - transform.position).sqrMagnitude).ToList();
            if(nodes.Count>0) return nodes.GetRange(0, num);
            return null;
        }

        public Vector2 GetNodeWidth()
        {
            if (image.rectTransform.rect.width == 0 || image.rectTransform.rect.height == 0)
            {
                return new Vector2(60, 60);
            }
            return new Vector2(image.rectTransform.rect.width, image.rectTransform.rect.height);
        }

        public void Select()
        {
            if (isSelected) return;
            isSelected = true;
            print("Select Node");
        }
    }
}