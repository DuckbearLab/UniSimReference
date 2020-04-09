using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using NetStructs;
using Simulation.Minimap;

namespace Simulation.Minimap
{
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(RectTransform))]
    public class EntityOnMinimap : MonoBehaviour
    {

        public GameObject EnemyDotPrefab;
        public GameObject FriendlyDotPrefab;
        public Minimap map;
        public ReflectedEntities ReflectedEntities;
        public PublishedEntity PublishedEntity;
        public GameObject Infantry;
        public float Distanse;
        private ForceType enemyForceType;
        private Dictionary<ReflectedEntity, MinimapPointPositioner> entityDot;

        // Use this for initialization
        void Start()
        {
            entityDot = new Dictionary<ReflectedEntity, MinimapPointPositioner>();

            if (PublishedEntity.ForceType == ForceType.DtForceFriendly)
                enemyForceType = ForceType.DtForceOpposing;
            else if (PublishedEntity.ForceType == ForceType.DtForceOpposing)
                enemyForceType = ForceType.DtForceFriendly;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateEntityLocations();
        }

        private void UpdateEntityLocations()
        {
            var list = entityDot.ToList();
            var missing = list.FindAll(x => x.Key == null);

            missing.ForEach(x => Destroy(x.Value.gameObject));
            list.RemoveAll(x => x.Key == null);
            entityDot = list.ToDictionary(x => x.Key, x => x.Value);

            foreach (var refEnt in ReflectedEntities.ReflectedEntitiesList)
            {
                if (refEnt.ForceType == enemyForceType || refEnt.ForceType == PublishedEntity.ForceType)
                {
                    float distanseEntity = Vector3.Distance(Infantry.transform.position, refEnt.transform.position);
                    if (Distanse < distanseEntity)
                    {
                        if (entityDot.ContainsKey(refEnt))
                        {
                            Destroy(entityDot[refEnt].gameObject);
                            entityDot.Remove(refEnt);
                        }
                        continue;
                    }
                    if (entityDot.ContainsKey(refEnt))
                        entityDot[refEnt].Position = new Vector2(refEnt.transform.position.x, refEnt.transform.position.z);
                    else
                    {
                        GameObject prefab;

                        //if (refEnt.ForceType == enemyForceType)
                        //    prefab = EnemyDotPrefab;
                        if (refEnt.ForceType == PublishedEntity.ForceType)
                        {
                            prefab = FriendlyDotPrefab;

                            var obj = Instantiate(prefab, map.gameObject.transform);
                            var point = obj.GetComponent<MinimapPointPositioner>();
                            point.Minimap = map;
                            point.Position = new Vector2(refEnt.transform.position.x, refEnt.transform.position.z);
                            entityDot.Add(refEnt, point);
                        }
                    }
                }
            }
        }
    }
}
