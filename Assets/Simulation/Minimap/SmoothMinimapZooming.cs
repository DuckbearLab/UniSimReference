using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * SmoothMinimapZooming -
 * DESCRIPTION - Zooms the minimap smoothly in and out.
 * =================================================================================== */
namespace Simulation.Minimap
{
    [RequireComponent(typeof(Minimap))]
    public class SmoothMinimapZooming : MonoBehaviour
    {
        [Range(1.01f, 2f)]
        public float ZoomInMultiplier = 1.2f;

        private Minimap minimap;

        private float ZoomOutMultiplier;

        void Awake()
        {
            minimap = GetComponent<Minimap>();
            ZoomOutMultiplier = 1 / ZoomInMultiplier;
        }

        void Update()
        {
            float y = Input.mouseScrollDelta.y;
            if (y > 0)
            {
                minimap.Zoom *= ZoomInMultiplier;
            }
            if (y < 0)
            {
                minimap.Zoom *= ZoomOutMultiplier; 
            }
        }

        void OnValidate()
        {
            ZoomOutMultiplier = 1 / ZoomInMultiplier;
        }

        void Reset()
        {
            GetComponent<Minimap>().ClampZoom = true;
        }
    }
}