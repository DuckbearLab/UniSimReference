using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Simulation.Minimap
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class MinimapLinePositioner : MonoBehaviour
    {
        public Minimap Minimap;

        public Vector2 PointA;
        public Vector2 PointB;

        public float LineWidth = 10;
        public bool WorldMinimapLineWidth;
        //private int lastZoomIndex; //never used


        private RectTransform rectTransform;



        void Awake()
        {
            this.rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0, 0.5f);
        }

        void Start()
        {
            if (Minimap == null)
                Minimap = GetComponentInParent<Minimap>();


        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!Minimap.Initialized)
                return;

            var minimapA = Minimap.WorldPointToMinimap(PointA);
            var minimapB = Minimap.WorldPointToMinimap(PointB);

            rectTransform.anchoredPosition = minimapA;
            rectTransform.localRotation = Quaternion.FromToRotation(Vector3.right, minimapB - minimapA);
            if(WorldMinimapLineWidth)
                rectTransform.sizeDelta = new Vector2(Vector3.Distance(minimapA, minimapB), WorldToMinimapWidth());
            else
                rectTransform.sizeDelta = new Vector2(Vector3.Distance(minimapA, minimapB), LineWidth);
        }

        public void SetWorldPoints(Vector3 worldPointA, Vector3 worldPointB)
        {
            this.PointA = new Vector2(worldPointA.x, worldPointA.z);
            this.PointB = new Vector2(worldPointB.x, worldPointB.z);
        }
        public void SetWorldPoints(Vector2 worldPointA, Vector2 worldPointB)
        {
            PointA = worldPointA;
            PointB = worldPointB;
        }

        public float WorldToMinimapWidth()
        {
            Vector2 MinimapLineWidthA = Minimap.WorldPointToMinimap(PointA + new Vector2(0, LineWidth / 2));
            Vector2 MinimapLineWidthB = Minimap.WorldPointToMinimap(PointA + new Vector2(0, -LineWidth / 2));

            return Vector2.Distance(MinimapLineWidthA, MinimapLineWidthB);
        }
    }
}