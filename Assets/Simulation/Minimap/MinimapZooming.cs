using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Simulation.Minimap
{
    public class MinimapZooming : MonoBehaviour, IScrollHandler
    {
        public Minimap Minimap;

        public int CurrentZoomIndex;
        public float[] Zooms;
        public event System.Action<int> ZoomChanged;

        void Update()
        {
            CurrentZoomIndex = Mathf.Clamp(CurrentZoomIndex, 0, Zooms.Length - 1);
            Minimap.Zoom = Zooms[CurrentZoomIndex];
        }

        public void OnScroll(PointerEventData eventData)
        {
            CurrentZoomIndex += (int) Mathf.Sign(eventData.scrollDelta.y);

            if (ZoomChanged != null)
                ZoomChanged(CurrentZoomIndex);
        }
    }
}
