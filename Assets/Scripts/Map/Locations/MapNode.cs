using System;
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

        [Header("References")] 
        [SerializeField] public Button button;
        [SerializeField] public Image image;
        [SerializeField] private UILine linePrefab;
        
        //Internal
        private List<List<MapNode>> paths = new List<List<MapNode>>();
        public List<MapNode> nextNodes;//Nodes that this node is connected to
        private MapLevel mapLevel;
        private MapNodeStatus status=MapNodeStatus.Locked;
        private bool loadedNodes;
        private NodeData data;
        private bool isSelected;
        
        public void Init(MapLevel level, int num, NodeData nodeData)
        {
            name = level.name + " Map Node "+num;
            SetStatus();
            mapLevel = level;
            data = nodeData;
            if (data == null) return;
            image.sprite = data.nodeSprite;
        }
        

        public List<MapNode> GetNextClosestNodes(int nodeCount=2)
        {
            print("Count: "+nodeCount);
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
            int amount= num;
            if (nodes.Count < num) amount = nodes.Count;
            if(nodes.Count>0) return nodes.GetRange(0, amount);
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

        public void AddPath(List<MapNode> path)
        {
            hasPath = true;
            paths.Add(path);
            int index = path.IndexOf(this);
            if (path.Count > index + 1)
            {
                nextNodes.Add(path[index+1]);
            }
        }

        public void SetStatus(MapNodeStatus nodeStatus=MapNodeStatus.Locked)
        {
            button.interactable = nodeStatus == MapNodeStatus.Unlocked;
            status = nodeStatus;
            switch (nodeStatus)
            {
                case MapNodeStatus.Locked:
                    image.color = Color.blue;
                    break;
                case MapNodeStatus.Unlocked:
                    image.color = Color.green;
                    break;
                case MapNodeStatus.Completed:
                    image.color = Color.black;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeStatus), nodeStatus, null);
            }
        }

        public void Select()
        {
            if (status != MapNodeStatus.Unlocked) return;
            mapLevel.LockNodes();
            SetStatus(MapNodeStatus.Completed);
            foreach (MapNode node in nextNodes)
            {
                node.SetStatus(MapNodeStatus.Unlocked);
            }
        }
    }

    [Serializable]
    public enum MapNodeStatus
    {
        Locked,
        Unlocked,
        Completed
    }
}