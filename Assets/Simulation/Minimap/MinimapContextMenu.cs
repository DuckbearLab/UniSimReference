using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/* ===================================================================================
 * MinimapContextMenu -
 * DESCRIPTION - Provides prebuilt minimap context menu behaviour. 
 * GameObject Heirarchy layout:
 * >Minimap
 *   >MinimapPointPositioner
 *     >Offset (Must contain the entire minimap content)
 *       >Minimap Content
 * =================================================================================== */
namespace Simulation.Minimap
{
    [RequireComponent(typeof(Minimap))]
    public class MinimapContextMenu : Utils.ContextMenu
    {
        public MinimapPointPositioner MenuPositioner;
        public PointerEventData.InputButton OpenButton = PointerEventData.InputButton.Right;

        protected Minimap Minimap;

        /// <summary>
        /// Sets the event. Please call base.Start() if overriding. 
        /// </summary>
        override protected void Start()
        {
            Minimap = GetComponent<Minimap>();
            Minimap.MinimapClicked += ToggleContextMenu;
            base.Start();
        }

        protected virtual void ToggleContextMenu(Vector2 minimapPosition, Vector3 worldPosition, PointerEventData.InputButton inputButton)
        {
            MenuPositioner.SetWorldPoint(worldPosition);
            ToggleContextMenu(inputButton == OpenButton);
        }
    }
}