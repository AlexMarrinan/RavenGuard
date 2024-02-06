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
        public List<List<MapNode>> paths;
        public bool hasPath;
        public MapLevel mapLevel;
        
        [Header("References")] 
        [SerializeField] private Image image;
        [SerializeField] private UILine linePrefab;
        
        //Internal
        private Canvas canvas;
        private NodeData data;
        private bool isSelected;
        
        public void Init(MapLevel level, Canvas canvas, NodeData nodeData)
        {
            mapLevel = level;
            data = nodeData;
            this.canvas = canvas;
            if (data == null) return;
            image.sprite = data.nodeSprite;
        }

        public List<MapNode> GetNextClosestNodes(int nodeCount=2)
        {
            List<MapNode> nextLevelNodes=mapLevel.GetNextLevelNodes();
            if (nextLevelNodes == null) return null;
            List<MapNode> closestNodes = nextLevelNodes
                .OrderBy((node) => (node.transform.position - transform.position).sqrMagnitude).ToList();
            return closestNodes.GetRange(0,nodeCount);
        }

        public List<List<MapNode>> GetPaths()
        {
            List<MapNode> closestNodes = GetNextClosestNodes();
            if (paths != null) return paths;
            paths = new List<List<MapNode>>();
            foreach (MapNode node in closestNodes)
            {
                foreach (List<MapNode> list in node.GetPaths())
                {
                    paths.Add(list);
                }
            }
            return paths;
        }

        public void DrawPath()
        {
            image.color=Color.red;
            List<MapNode> closestNodes = GetNextClosestNodes();
            if(closestNodes==null) return;
            foreach (MapNode node in closestNodes)
            {
                Vector2 thisPos = new Vector2(transform.position.x -canvas.pixelRect.width/2, transform.position.y-canvas.pixelRect.height/2);
                Vector2 nextPos = new Vector2(node.transform.position.x -canvas.pixelRect.width/2, node.transform.position.y-canvas.pixelRect.height/2);
                UILine path = Instantiate(linePrefab, transform.parent.parent);
                path.SetLine(new List<Vector2>(){thisPos,nextPos});
                node.DrawPath();
                node.hasPath = true;
            }
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