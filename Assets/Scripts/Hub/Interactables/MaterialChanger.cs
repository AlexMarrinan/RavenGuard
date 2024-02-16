using UnityEngine;

namespace Hub.Interactables
{
    /// <summary>
    /// Changes the material of a sprite renderer.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class MaterialChanger : MonoBehaviour
    {
        // Inspector
        [Tooltip("The materials which can be cycled through.")]
        [SerializeField] private Material[] materials;
        
        // References
        private new Renderer renderer;


        private void Awake()
        {
            renderer = GetComponent<Renderer>();
        }

        /// <summary>
        /// Sets the renderer material.
        /// </summary>
        /// <param name="index">An index corresponding to which material to set.</param>
        public void SetMaterial(int index)
        {
            renderer.material = materials[index];
        }
    }
}
