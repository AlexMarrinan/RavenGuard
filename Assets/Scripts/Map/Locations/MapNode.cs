using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Map.Locations.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Map.Locations
{
    public class MapNode : MonoBehaviour
    {
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

        public void Select()
        {
            if (isSelected) return;
            isSelected = true;
            print("Select Node");
        }
    }
}