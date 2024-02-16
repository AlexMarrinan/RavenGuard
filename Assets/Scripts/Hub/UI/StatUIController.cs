using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Hub.UI
{
    public class StatUIController:MonoBehaviour
    {
        public SerializedDictionary<StatUI, Sprite> spiteLibrary = new SerializedDictionary<StatUI, Sprite>();
    }
}