using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.UI
{
    public class StatUI:MonoBehaviour
    {
        [SerializeField] private Image statImage;

        public void Init(Sprite sprite)
        {
            statImage.sprite = sprite;
        }
    }
}