using Assets.Scripts.Map.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class MapController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private MapView mapView;
        
        [SerializeField] private int levelsNum;
        [SerializeField] private int roomsPerLevel;
        [SerializeField] private Orientation mapOrientation;

        void Awake()
        {
            mapView.Init(levelsNum,roomsPerLevel,mapOrientation);
        }
    }
}
