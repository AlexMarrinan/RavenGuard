using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Map.Locations;
using Assets.Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;

// Slay The Spire Map Logic: https://steamcommunity.com/sharedfiles/filedetails/?id=2830078257

namespace Assets.Scripts.Map.UI
{
    public class MapView : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private RectTransform lineParent;
        [SerializeField] private RectTransform levelParent;
        [SerializeField] private Canvas canvas;
        public int numBranches = 2;
        
        [Header("Prefabs")] 
        [SerializeField] private UILine linePrefab;
        [SerializeField] private MapLevel mapLevelPrefab;
        
        // Internal
        private List<List<Vector2>> mapLines=new List<List<Vector2>>();
        private List<List<MapNode>> mapPaths=new List<List<MapNode>>();
        private List<MapLevel> mapLevels=new List<MapLevel>();
        private Bounds levelParentBounds;
        private float canvasHeight;
        private float canvasWidth;
        private float levelHeight;
        private float levelWidth;
        private float halfLevelHeight;
        private float halfLevelWidth;

        public void Init(int levels, int roomsPerLevel, Orientation orientation=Orientation.LEFT_TO_RIGHT)
        {
            Setup(levels);

            for (int i = 0; i < levels; i++)
            {
                MapLevel level = Instantiate(mapLevelPrefab, levelParent);
                Vector2 position = GetLevelPosition(i, orientation);
                level.Init(i,roomsPerLevel,levelWidth,levelHeight,position,orientation);
                mapLevels.Add(level);
            }
            
            for (int i = 0; i < levels; i++)
            {
                if (i + 1 != levels)
                {
                    mapLevels[i].nextLevel = mapLevels[i + 1];
                }
            }

            GetPath();
        }

        private void Setup(int levels)
        {
            canvasHeight = levelParent.rect.height;
            canvasWidth = levelParent.rect.width;
            levelWidth = canvasWidth / levels;
            levelHeight = canvasHeight / levels;
            halfLevelWidth = levelWidth / 2;
            halfLevelHeight = levelHeight / 2;
        }

        private void GetPath()
        {
            MapNode firstNode = mapLevels[0].nodes[0];
            List<MapNode> startingNodes=firstNode.GetNextClosestNodes(numBranches);
            for (int i = 0; i < numBranches; i++)
            {
                MapNode nextNode = startingNodes[i];
                List<MapNode> newPath = new List<MapNode>() { firstNode,nextNode };
                firstNode.hasPath = true;
                GetPath(1,nextNode,newPath);
                mapPaths.Add(newPath);
            }
            
            DrawAllPaths();
        }

        private void GetPath(int num, MapNode node, List<MapNode> currentPath)
        {
            List<MapNode> newNodes = node.GetNextClosestNodes(num);
            List<MapNode> pathNodes=currentPath;
            if (newNodes != null)
            {
                MapNode nextNode = newNodes[0];
                pathNodes.Add(nextNode);
                GetPath(num, nextNode, pathNodes);
            }
        }

        private void HideUnusedAreas()
        {
            foreach (MapLevel level in mapLevels)
            {
                foreach (MapNode node in level.nodes)
                {
                    if (!node.hasPath)
                    {
                        node.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void DrawAllPaths()
        {
            foreach (List<MapNode> paths in mapPaths)
            {
                DrawPath(paths);
            }

            HideUnusedAreas();
        }

        private void DrawPath(List<MapNode> nodeList)
        {
            List<Vector2> line = new List<Vector2>();
            foreach (MapNode node in nodeList)
            {
                float x=node.transform.position.x - canvas.pixelRect.width/2;
                float y=node.transform.position.y - canvas.pixelRect.height/2;
                line.Add(new Vector2(x,y));
            }

            if (!HasIntersection(line))
            {
                foreach (MapNode node in nodeList)
                {
                    node.hasPath = true;
                }
                UILine path = Instantiate(linePrefab, lineParent);
                path.name = nodeList[^1].name + " line";
                path.SetLine(line);
            }
            
        }

        private bool HasIntersection(List<Vector2> positions)
        {
            foreach (List<Vector2> line in mapLines)
            {
                for (int i = 1; i < positions.Count; i++)
                {
                    bool intersection = lineSegmentsIntersect(positions[i-1], positions[i], line[i-1], line[i]);    
                    if (!intersection)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool lineSegmentsIntersect(Vector2 lineOneA, Vector2 lineOneB, Vector2 lineTwoA, Vector2 lineTwoB)
        {
            return (((lineTwoB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x) > (lineTwoA.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)) != ((lineTwoB.y - lineOneB.y) * (lineTwoA.x - lineOneB.x) > (lineTwoA.y - lineOneB.y) * (lineTwoB.x - lineOneB.x)) && ((lineTwoA.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x)) != ((lineTwoB.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)));
        }
        
        /// <summary>
        /// Gets the position for a level with the given index in and a map in the given orientation
        /// </summary>
        /// <param name="index">Index of the level in the map</param>
        /// <param name="orientation">Orientation of the map</param>
        /// <returns>The level position</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private Vector2 GetLevelPosition(int index, Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.BOTTOM_TO_TOP or Orientation.TOP_TO_BOTTOM:
                    return GetVerticalPosition(index,orientation);
                case Orientation.LEFT_TO_RIGHT or Orientation.RIGHT_TO_LEFT:
                    return GetHorizontalPosition(index, orientation);
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }
        
        /// <summary>
        /// Gets the position for a level with the given index in and a map in the given vertical orientation
        /// </summary>
        /// <param name="index">Index of the level in the map</param>
        /// <param name="orientation">Orientation of the map</param>
        /// <returns>The level position</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private Vector2 GetVerticalPosition(int index, Orientation orientation)
        {
            levelWidth = canvasWidth;
            float leftOffset = levelParent.offsetMin.x;
            float bottomOffset = levelParent.offsetMin.y;
            float x=(levelWidth*.5f)+leftOffset;
            float y =(index * levelHeight)+halfLevelHeight;
            switch (orientation)
            {
                case Orientation.BOTTOM_TO_TOP:
                    y = canvasHeight - y + bottomOffset;
                    return new Vector2(x, y);
                case Orientation.TOP_TO_BOTTOM:
                    y += bottomOffset;
                    return new Vector2(x, y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        /// <summary>
        /// Gets the position for a level with the given index in and a map in the given horizontal orientation
        /// </summary>
        /// <param name="index">Index of the level in the map</param>
        /// <param name="orientation">Orientation of the map</param>
        /// <returns>The level position</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private Vector2 GetHorizontalPosition(int index, Orientation orientation)
        {
            levelHeight = canvasHeight;
            float bottomOffset = levelParent.offsetMin.y;
            
            float x=(index*levelWidth)+halfLevelWidth;
            float y = (levelHeight * .5f) + bottomOffset;
            switch (orientation)
            {
                case Orientation.LEFT_TO_RIGHT:
                    x += levelParent.offsetMin.x;
                    return new Vector2(x, y);
                case Orientation.RIGHT_TO_LEFT:
                    x = canvasWidth - x -levelParent.offsetMax.x;
                    return new Vector2(x, y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }
    }
    
    [Serializable]
    public enum Orientation
    {
        BOTTOM_TO_TOP,
        TOP_TO_BOTTOM,
        LEFT_TO_RIGHT,
        RIGHT_TO_LEFT
    }
}