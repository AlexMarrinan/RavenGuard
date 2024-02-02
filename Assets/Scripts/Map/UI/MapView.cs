using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Map.Locations;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Map.UI
{
    public class MapView : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private RectTransform levelParent;
        
        [Header("Prefabs")] 
        [SerializeField] private MapLevel mapLevelPrefab;
        
        // Internal
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
            
            Vector2 position = GetLevelPosition(0, orientation);
            print($"Width: {canvasWidth} Height: {canvasHeight}");
            for (int i = 0; i < levels; i++)
            {
                MapLevel level = Instantiate(mapLevelPrefab, levelParent);
                mapLevels.Add(level);
                position = GetLevelPosition(i, orientation);
                level.Init(roomsPerLevel,levelWidth,levelHeight,position);
            }
        }

        private void Setup(int levels)
        {
            float yOffset = levelParent.offsetMin.y-levelParent.offsetMax.y;
            print(yOffset);
            canvasHeight = levelParent.rect.height;
            canvasWidth = levelParent.rect.width;
            levelWidth = canvasWidth / levels;
            levelHeight = canvasHeight / levels;
            halfLevelWidth = levelWidth / 2;
            halfLevelHeight = levelHeight / 2;
        }


        private Vector2 GetLevelPosition(int index, Orientation orientation)
        {
            float horizontalX=(index*levelWidth)+halfLevelWidth;
            float verticalY = (index * levelHeight)+halfLevelHeight;
            
            float topOffset=levelParent.offsetMax.y;
            float bottomOffset = levelParent.offsetMin.y;
            float rightOffset=levelParent.offsetMax.x;
            float leftOffset = levelParent.offsetMin.x;

            float x;
            float y;
            
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

    public struct Bounds
    {
        public float left;
        public float right;
        public float up;
        public float down;
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