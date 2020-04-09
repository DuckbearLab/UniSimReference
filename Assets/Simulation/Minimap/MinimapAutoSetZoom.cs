using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * MinimapAutoSetZoom -
 * DESCRIPTION - Automatically sets the zoom of the minimap to touch/normal zoom. 
 * =================================================================================== */
namespace Simulation.Minimap
{
    public class MinimapAutoSetZoom : MonoBehaviour
    {
        public MonoBehaviour NormalZoom, TouchZoom;
        void Update()
        {
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                NormalZoom.enabled = true;
                TouchZoom.enabled = false;
            }
            else if (0 < Input.touchCount)
            {
                NormalZoom.enabled = false;
                TouchZoom.enabled = true;
            }
        }

#if UNITY_EDITOR
        void Reset()
        {
            MinimapTouchZooming tz = GetComponent<MinimapTouchZooming>();
            if (null != tz)
            {
                TouchZoom = tz;
            }
            SmoothMinimapZooming smz = GetComponent<SmoothMinimapZooming>();
            if (null != smz)
            {
                NormalZoom = smz;
            }
            else
            {
                MinimapZooming mz = GetComponent<MinimapZooming>();
                if (null != mz)
                {
                    NormalZoom = mz;
                }
            }
        }
#endif
    }
}