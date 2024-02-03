using System;
using System.Collections.Generic;
using Assets.Scripts.Map.UI;
using UnityEngine;
using UnityEngine.UI;
using Random=System.Random;

namespace Assets.Scripts.Map.Locations
{
    public class MapLevel : MonoBehaviour
    {
        public Image image;
        
        [Header("References")] 
        [SerializeField] private RectTransform rectTransform;
        [Header("Prefabs")] 
        [SerializeField] private MapNode mapNodePrefab;
        //Internal
        private List<MapNode> nodes = new List<MapNode>();
        private float nodeWidthChunk;
        private float nodeHeightChunk;

        public void Init(int levelIndex, int roomsPerLevel, float width, float height, Vector2 position, Orientation orientation, bool isLastLevel)
        {
            rectTransform.sizeDelta = new Vector2(width,height);
			nodeWidthChunk = width /roomsPerLevel;
            nodeHeightChunk = height/roomsPerLevel;
            
            for (int i = 0; i < roomsPerLevel; i++)
            {
                MapNode node=Instantiate(mapNodePrefab, transform);
                transform.position = position;
                node.Init(null);
                node.transform.localPosition = GetNodePosition(i, orientation, width,height,node);
                nodes.Add(node);
            }

            if (levelIndex == 0) ChooseFirstNode(roomsPerLevel);
        }

        private Vector2 GetNodePosition(int index,Orientation orientation,float width, float height, MapNode node)
        {
            //Random random = new Random();
            Vector2 spriteSize = node.GetNodeWidth();
            float spriteWidth = spriteSize.x * .5f;
            float spriteHeight = spriteSize.y * .5f;
            

			float x=(nodeWidthChunk*.5f)-spriteWidth;
			float y=(nodeHeightChunk*.5f)-spriteHeight;

			switch (orientation)
            {
                case Orientation.BOTTOM_TO_TOP or Orientation.TOP_TO_BOTTOM:
                    x=nodeWidthChunk*index-(width*.35f);
					break;
                case Orientation.LEFT_TO_RIGHT or Orientation.RIGHT_TO_LEFT:
                    y=nodeHeightChunk*index-(height*.35f);
					break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
           
            return new Vector2(x, y);
        }

        private void ChooseFirstNode(int roomNum)
        {
            Random random = new Random();
            int index = random.Next(0, roomNum);
            for(int i=0; i < nodes.Count; i++)
            {
                if (i != index)
                {
                    nodes[i].gameObject.SetActive(false);
                }
            }
            RemoveUnusedNodes();
        }

        public void RemoveUnusedNodes()
        {
            List<MapNode> temp = new List<MapNode>();
            foreach (MapNode node in nodes)
            {
                if(node.isPathless || !node.gameObject.activeSelf) temp.Add(node);
            }
            foreach (MapNode node  in temp)
            {
                node.gameObject.SetActive(false);
                nodes.Remove(node);
            }
        }
    }
}

