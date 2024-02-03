using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Map.Locations.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Map.Locations
{
    public class MapNode : MonoBehaviour
    {
        public bool isPathless;
        
        [Header("References")] 
        [SerializeField] private Image image;
        
        //Internal
        private NodeData data;
        private bool isSelected;
        
        public void Init(NodeData nodeData)
        {
            data = nodeData;
            if (data == null) return;
            image.sprite = data.nodeSprite;
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