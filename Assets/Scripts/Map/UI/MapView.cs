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

            MakeMap();
        }

        /// <summary>
        /// Set up the canvas sizing variables
        /// </summary>
        /// <param name="levels">The number of levels</param>
        private void Setup(int levels)
        {
            canvasHeight = levelParent.rect.height;
            canvasWidth = levelParent.rect.width;
            levelWidth = canvasWidth / levels;
            levelHeight = canvasHeight / levels;
            halfLevelWidth = levelWidth / 2;
            halfLevelHeight = levelHeight / 2;
        }
        
        /// <summary>
        /// Start drawing the map paths
        /// </summary>
        private void MakeMap()
        {
            MapNode firstNode = mapLevels[0].nodes[0];
            List<MapNode> startingNodes=firstNode.GetNextClosestNodes(numBranches);
            for (int i = 0; i < numBranches; i++)
            {
                MapNode nextNode = startingNodes[i];
                firstNode.hasPath = true;
                MakePath(new List<MapNode>() { firstNode},nextNode);
            }

            DrawMap();
        }

        #region Path Generating

        /// <summary>
        /// Make a path that uses the given path and node
        /// </summary>
        /// <param name="currentPath">The path</param>
        /// <param name="node">A node that you can use to get to the next nodes</param>
        private void MakePath(List<MapNode> currentPath, MapNode node)
        {
            List<MapNode> newNodes = node.GetNextClosestNodes(numBranches);
            List<MapNode> path = currentPath;
            if (newNodes != null)
            {
                MapNode nextNode = newNodes[0];
                path.Add(nextNode);
                MakePath(currentPath, nextNode);
                return;
            }
            AddPath(currentPath);
        }

        /// <summary>
        /// Adds the given path to mapPaths if it isn't in already
        /// </summary>
        /// <param name="path">The path being added</param>
        private void AddPath(List<MapNode> path)
        {
            if (ShouldAddPath(path))
            {
                mapPaths.Add(path);
            }
        }

        #endregion

        #region Path Bool Checks

        /// <summary>
        /// Checks if the given path should be added to mapPaths
        /// </summary>
        /// <param name="path">The path potentially being added</param>
        /// <returns>Whether the path should be added</returns>
        private bool ShouldAddPath(List<MapNode> path)
        {
            return NoDuplicates(path) && NotPartOfPath(path);
        }

        /// <summary>
        /// Checks to see if mapPaths contains path
        /// </summary>
        /// <param name="path">The path being checked</param>
        /// <returns>True if mapPaths doesn't contain path</returns>
        private bool NoDuplicates(List<MapNode> path)
        {
            return !mapPaths.Contains(path);
        }
        
        /// <summary>
        /// Checks to see if the given path is apart of mapPaths
        /// </summary>
        /// <param name="path">The path that could be part of a path in mapPaths</param>
        /// <returns>Whether the given path is part of mapPaths</returns>
        private bool NotPartOfPath(List<MapNode> path)
        {
            foreach (List<MapNode> acceptedPaths in mapPaths)
            {
                if (PartOfPath(acceptedPaths, path))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks to see if the given smallerPath is part of the given biggerPath
        /// </summary>
        /// <param name="biggerPath">The path that could potentially contain the smallerPath</param>
        /// <param name="smallerPath">The path that could be part of the biggerPath</param>
        /// <returns>Whether the biggerPath contains the smallerPath</returns>
        private bool PartOfPath(List<MapNode> biggerPath, List<MapNode> smallerPath)
        {
            return biggerPath.Intersect(smallerPath).Count() == smallerPath.Count();
        }
        
        /// <summary>
        /// Checks to see if path crosses over any of the accepted paths
        /// </summary>
        /// <param name="path">The list of points on a path</param>
        /// <returns>Whether or not the given path overlaps with any path in mapPaths</returns>
        private bool HasIntersection(List<Vector2> path)
        {
            foreach (List<Vector2> line in mapLines)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    bool intersection = LineSegmentsIntersect(path[i-1], path[i], line[i-1], line[i]);    
                    if (!intersection)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks to see if line intersects
        /// </summary>
        /// <param name="lineOneA">The start of line 1</param>
        /// <param name="lineOneB">The end of line 1</param>
        /// <param name="lineTwoA">The start of line 2</param>
        /// <param name="lineTwoB">The end of line 2</param>
        /// <returns>True if an intersection exists</returns>
        private static bool LineSegmentsIntersect(Vector2 lineOneA, Vector2 lineOneB, Vector2 lineTwoA, Vector2 lineTwoB)
        {
            return (((lineTwoB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x) > (lineTwoA.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)) != ((lineTwoB.y - lineOneB.y) * (lineTwoA.x - lineOneB.x) > (lineTwoA.y - lineOneB.y) * (lineTwoB.x - lineOneB.x)) && ((lineTwoA.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x)) != ((lineTwoB.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)));
        }

        #endregion

        #region Draw Map

        private void DrawMap()
        {
            DrawAllPaths();
            HideUnusedAreas();
        }

        /// <summary>
        /// Hide nodes that aren't a part of a path
        /// </summary>
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

        /// <summary>
        /// Draw all the paths
        /// </summary>
        private void DrawAllPaths()
        {
            foreach (List<MapNode> paths in mapPaths)
            {
                DrawPath(paths);
            }
        }

        /// <summary>
        /// Draw the given path
        /// </summary>
        /// <param name="nodeList">The path being drawn</param>
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

        #endregion

        #region Map Orientation

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

        #endregion
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