using System;
using Game.Dialogue;
using UnityEngine;

namespace Game.Hub.Interactables
{
    [CreateAssetMenu(fileName = "ParagonInfo", menuName = "ParagonInfo", order = 0)]

    [Serializable]
    public class ParagonInfo : ScriptableObject
    {
        public Sprite sprite;
        public SpeakerData speakerData;
        public UnitClass unitClass;
        public int cost;
    }
}