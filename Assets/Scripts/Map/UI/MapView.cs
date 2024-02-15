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
        [SerializeField] private PathHandler pathHandler;
        public int numBranches = 2;
        public int maxNodesInLevel=3;
        
        [Header("Prefabs")] 
        [SerializeField] private UILine linePrefab;
        [SerializeField] private MapLevel mapLevelPrefab;
        
        // Internal
        private List<MapLevel> mapLevels=new List<MapLevel>();
        
        // Orientation info
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
                bool onlyOneNode = i == 0 || i == levels - 1;
                level.Init(i,roomsPerLevel,levelWidth,levelHeight,position,orientation,onlyOneNode);
                mapLevels.Add(level);
            }
            
            for (int i = 0; i < levels; i++)
            {
                if (i + 1 != levels)
                {
                    mapLevels[i].nextLevel = mapLevels[i + 1];
                }
            }

            pathHandler.Init(canvas,numBranches,mapLevels[0].nodes[0]);
            DrawMap();
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
            List<List<MapNode>> mapPaths = pathHandler.GetMapNodes();
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
            SetMapNode(nodeList);
            UILine path = Instantiate(linePrefab, lineParent);
            path.name = nodeList[^1].name + " line";
            path.SetLine(pathHandler.GetPositionList(nodeList));
        }

        private void SetMapNode(List<MapNode> nodeList)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].AddPath(nodeList);
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