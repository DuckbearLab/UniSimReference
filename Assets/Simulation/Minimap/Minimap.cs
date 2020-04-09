using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Simulation.Minimap
{
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(RectTransform))]
    public class Minimap : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
    {

        public delegate void MinimapClick(Vector2 minimapPosition, Vector3 worldPosition, PointerEventData.InputButton inputButton);
        public event MinimapClick MinimapClicked;

        public event Action MinimapGotFocus;

        public TerrainLoader TerrainLoader;
        [SerializeField, ReadOnlyWhenEditing] private Vector2 MapMin;
        [SerializeField, ReadOnlyWhenEditing] private Vector2 MapMax;
        //private NightLoader NightLoader; //never used

        public float Zoom = 1;

        public bool Initialized { get; private set; }

        private NoTerrainMinimap noTerrainMinimap;

        public RawImage MapImage { get; private set; }
        private RectTransform rectTransform;
        private Mask mask;

        /// <summary>
        /// Disables zooming out further than the edges of the minimap. 
        /// </summary>
        public bool ClampZoom = false;
        private float minZoom;


        void Start()
        {
            if (null == MapImage)
                MapImage = GetComponent<RawImage>();
            if (null == rectTransform)
                rectTransform = GetComponent<RectTransform>();
            if (null == mask)
                mask = GetComponentInParent<Mask>();

            rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            UpdateZoom();

            noTerrainMinimap = GetComponent<NoTerrainMinimap>();
            if (TerrainLoader.Instance && TerrainLoader.Instance.isActiveAndEnabled && TerrainLoader.Instance.LoadState < TerrainLoader.TerrainLoadState.TerrainOnly)
            {
                TerrainLoader.Instance.TerrainOnlyLoaded += LoadMap;
            }
            else
            {
                LoadMap();
            }

        }

        /// <summary>
        /// Loads the minimap from MinimapTerrain (terrain loader) or NoTerrainMinimap
        /// </summary>
        void LoadMap()
        {
            var terrainMinimap = FindObjectOfType<TerrainMinimap>();
            if (terrainMinimap != null)
            {
                MapImage.texture = terrainMinimap.MapTexture;
                MapMin = terrainMinimap.MapMin;
                MapMax = terrainMinimap.MapMax;
                rectTransform.sizeDelta = new Vector2(terrainMinimap.MapTexture.width, terrainMinimap.MapTexture.height);
                rectTransform.anchoredPosition = Vector2.zero;
                Initialized = true;
            }

            else
            {
                if (noTerrainMinimap != null)
                {
                    NoTerrainMinimap.TerrainMinMaxPoints TerrainInfo = noTerrainMinimap.GetTerrainMinimapInfo();
                    Setup(TerrainInfo.MapMin, TerrainInfo.MapMax, TerrainInfo.MinimapTexture);
                    rectTransform.sizeDelta = new Vector2(TerrainInfo.MinimapTexture.width, TerrainInfo.MinimapTexture.height);
                    rectTransform.anchoredPosition = Vector2.zero;
                    Initialized = true;
                }
            }

            //the minimum zoom is calculated as the size of the parent devided by the size of the texture, so that 
            //the texture is never smaller than the parent. If ClampZoom is true, the zoom can never be smaller than MinZoom. 
            Vector2 size = MinimapSize();
            minZoom = Mathf.Max(size.x / MapImage.mainTexture.width,
                  size.y / MapImage.mainTexture.height);


            //if (NightLoader != null)
            //{
            //    if (NightLoader.IsDay == false || TerrainLoader.TerrainBundlePath.Contains("night") || TerrainLoader.TerrainBundlePath.Contains("Night"))
            //    {
            //        mapImage.texture = DayTextures[0];
            //        //foreach (Texture2D DayTexture in DayTextures)
            //        //{
            //        //    if(DayTexture.name.Contains(TerrainLoader.TerrainBundlePath))
            //        //    {
            //        //        mapImage.texture = DayTexture;
            //        //    }

            //        //}
            //    }
            //}

            //Initialized = true;

        }

        private Vector2 MinimapSize()
        {
            Canvas.ForceUpdateCanvases();
            Vector2 result = new Vector2(1f, 1f);

            Vector2 sizeOffset = new Vector2();

            RectTransform current = transform.parent as RectTransform;

            while (null != current)
            {
                Vector2 currentAnchorSize = current.anchorMax - current.anchorMin;
                if (currentAnchorSize == Vector2.zero)
                {
                    if (null == current.parent)
                    {
                        break;
                    }
                    else
                    {
                        result.Scale(current.rect.size);
                    }
                    result += sizeOffset;
                    return result;
                }
                result.Scale(currentAnchorSize);
                Vector2 sizeDelta = current.sizeDelta;
                sizeDelta.Scale(current.localScale);
                sizeOffset += sizeDelta;
                current = current.parent as RectTransform;
            } 

            return new Vector2(sizeOffset.x + result.x * Screen.width, sizeOffset.y + result.y * Screen.height);
        } 

        public void Setup(Vector2 MapMin, Vector2 MapMax, Texture2D MinimapImage)
        {
            if (null == MapImage)
                MapImage = GetComponent<RawImage>();
            if (null == rectTransform)
                rectTransform = GetComponent<RectTransform>();
            if (null == mask)
                mask = GetComponentInParent<Mask>();

            MapImage.texture = MinimapImage;
            this.MapMin = MapMin;
            this.MapMax = MapMax;
            rectTransform.sizeDelta = new Vector2(MinimapImage.width, MinimapImage.height);
            rectTransform.anchoredPosition = Vector2.zero;
            Vector2 size = MinimapSize();
            minZoom = Mathf.Max(size.x / MapImage.mainTexture.width,
                  size.y / MapImage.mainTexture.height);
            enabled = true;
            Initialized = true;
        }

        void Update()
        {
            if (!Initialized)
                return;
            var focusPointBeforeZoom = MinimaptoWorldPoint(GetMinimapFocusPoint());
            UpdateZoom();
            FocusOnWorldPoint(focusPointBeforeZoom);
        }

        public void UpdateZoom()
        {
            if (ClampZoom)
                Zoom = Mathf.Clamp(Zoom, minZoom, minZoom * 64);
            rectTransform.sizeDelta = new Vector2(
                MapImage.mainTexture.width * Zoom,
                MapImage.mainTexture.height * Zoom
            );
        }

        public Vector2 WorldPointToMinimap(Vector3 point)
        {
            return WorldPointToMinimap(new Vector2(point.x, point.z));
        }

        public Vector2 WorldPointToMinimap(Vector2 point)
        {
            return new Vector2(
                (point.x - MapMin.x) / (MapMax.x - MapMin.x) * rectTransform.sizeDelta.x,
                (point.y - MapMin.y) / (MapMax.y - MapMin.y) * rectTransform.sizeDelta.y
            );
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, null, out localPoint))
            {
                if (MinimapClicked != null)
                {
                    var minimapPosition = localPoint;
                    var worldPosition = MinimaptoWorldPoint(minimapPosition);
                    MinimapClicked(minimapPosition, worldPosition, eventData.button);
                }
            }
        }

        public bool ScreenToWorldMinimapPoint(out Vector3 Position)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint))
            {
                Position = MinimaptoWorldPoint(localPoint);
                return true;
            }
            Position = Vector3.negativeInfinity;
            return false;
        }

        public bool ScreenToWorldMinimapPoint(Vector2 ScreenPoint, out Vector3 Position)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), ScreenPoint, null, out localPoint))
            {
                Position = MinimaptoWorldPoint(localPoint);
                return true;
            }
            Position = Vector3.negativeInfinity;
            return false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(MinimapGotFocus != null)
                MinimapGotFocus();
        }

        public Vector3 MinimaptoWorldPoint(Vector2 minimapPoint)
        {
            return new Vector3(
                (minimapPoint.x / rectTransform.sizeDelta.x) * (MapMax.x - MapMin.x) + MapMin.x,
                GetWorldPointHight(minimapPoint),
                (minimapPoint.y / rectTransform.sizeDelta.y) * (MapMax.y - MapMin.y) + MapMin.y
            );
        }

        public void FocusOnWorldPoint(Vector3 point)
        {
            FocusOnMinimapPoint(WorldPointToMinimap(point));
        }

        public void FocusOnWorldPoint(Vector2 point)
        {
            FocusOnMinimapPoint(WorldPointToMinimap(point));
        }

        public void FocusOnMinimapPoint(Vector2 point)
        {
            rectTransform.anchoredPosition = new Vector3(
                    -point.x + mask.rectTransform.rect.width / 2,
                    -point.y + mask.rectTransform.rect.height / 2
                );
        }

        public Vector2 GetMinimapFocusPoint()
        {
            return new Vector2(
                -rectTransform.anchoredPosition.x + mask.rectTransform.rect.width / 2,
                -rectTransform.anchoredPosition.y + mask.rectTransform.rect.height / 2
            );
        }
        public float GetWorldPointHight(Vector2 mapPosition)
        {
            RaycastHit hitPoint;
            Vector3 NewPos = new Vector3(mapPosition.x, 0, mapPosition.y);
            if (Physics.Raycast(NewPos += new Vector3(0, 1000, 0), Vector3.down, out hitPoint))
            {
                NewPos = hitPoint.point;
                return NewPos.y;
            }
            return NewPos.y;
        }

    }
}