using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Minimap
{
    [RequireComponent(typeof(RectTransform))]
    public class MinimapPointPositioner : MonoBehaviour
    {
        public Minimap Minimap;

        public bool AspectRatio;

        public float PointRadius;

        public Vector2 Position;
        public float Rotation;

        private RectTransform rectTransform;

        void Awake()
        {
            this.rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
        }

        void Start()
        {
            if (Minimap == null)
                Minimap = GetComponentInParent<Minimap>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (null == Minimap || !Minimap.Initialized)
                return;

            rectTransform.anchoredPosition = Minimap.WorldPointToMinimap(Position);
            rectTransform.localEulerAngles = new Vector3(0, 0, -Rotation);
            if(AspectRatio)
            {
                float PointSize = WorldToMinimapWidth();
                rectTransform.sizeDelta = new Vector2(PointSize,PointSize);
            }

        }

        public void SetWorldPoint(Vector3 worldPos)
        {
            //If Desired AutoFocus On Click
            //Minimap.FocusOnWorldPoint(worldPos);
            gameObject.SetActive(true);
            Position = new Vector2(worldPos.x, worldPos.z);
            LateUpdate();
        }

        public float WorldToMinimapWidth()
        {
            Vector2 MinimapLineWidthA = Minimap.WorldPointToMinimap(Position + new Vector2(0, PointRadius));
            Vector2 MinimapLineWidthB = Minimap.WorldPointToMinimap(Position + new Vector2(0, -PointRadius));

            return Vector2.Distance(MinimapLineWidthA, MinimapLineWidthB);
        }

    }
}