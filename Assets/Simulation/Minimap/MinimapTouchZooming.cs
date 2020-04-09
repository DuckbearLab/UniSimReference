using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* ===================================================================================
 * MinimapTouchZooming -
 * DESCRIPTION - Minimap zooming using touch Input. 
 * =================================================================================== */
namespace Simulation.Minimap
{
    public class MinimapTouchZooming : MonoBehaviour
    {
        public Minimap Minimap;
        public ScrollRect ScrollRect;

        private float InitialTouchDistance = -1;

        void Update()
        {
            //if there are two touches
            if (2 == Input.touchCount)
            {
                //if this is the first frame in which the touch count is two
                if (-1 == InitialTouchDistance)
                {
                    //disable scrolling
                    ScrollRect.horizontal = ScrollRect.vertical = false;
                    InitialTouchDistance = (Input.touches[0].position - Input.touches[1].position).sqrMagnitude;
                }
                else
                {
                    //multiply the zoom by how much the distance has changed since the last frame. 
                    float CurrentTouchDistance = (Input.touches[0].position - Input.touches[1].position).sqrMagnitude;
                    Minimap.Zoom *= CurrentTouchDistance / InitialTouchDistance;
                    InitialTouchDistance = CurrentTouchDistance;
                }
            }
            else
            {
                if (-1 != InitialTouchDistance)
                {
                    //stop current scroll
                    ScrollRect.velocity = Vector2.zero;
                    InitialTouchDistance = -1;
                }
                //reenable scrolling
                ScrollRect.horizontal = ScrollRect.vertical = true;
            }
        }
    }
}