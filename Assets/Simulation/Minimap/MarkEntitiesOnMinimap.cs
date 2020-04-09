using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ===================================================================================
 * MarkentitiesOnMinimap -
 * Marks all entities with the matching entity type, force type, marking text, or any combination of the three on the minimap. 
 * The provided prefab must have the MinimapPointPositioner component, and a text component on it or a child.
 * =================================================================================== */
 namespace Simulation.Minimap {
    public class MarkEntitiesOnMinimap : MonoBehaviour
    {
        public GameObject ImagePrefab;
        public Transform MinimapTransform;

        public ReflectedEntities ReflectedEntities;

        public float UpdateInterval;
        public float ImageScale;
        public bool UpdateHeading, StartHidden;
        [Header("Filter Options")]
        public bool FilterByEntityTypePattern;
        public string EntityType;
        public bool FilterByForceType;
        public NetStructs.ForceType Type;
        public bool FilterByMarkingText;
        public string[] MarkingTexts;

        protected List<string> markingTextsList;
        protected Dictionary<ReflectedEntity, ImagePosition> EntityImageDictionary;

        protected struct ImagePosition
        {
            public GameObject gameObject;
            public MinimapPointPositioner MinimapPointPositioner;

            public ImagePosition(GameObject go, MinimapPointPositioner mmp)
            {
                gameObject = go;
                MinimapPointPositioner = mmp;
            }
        }

        private void Awake()
        {
            EntityImageDictionary = new Dictionary<ReflectedEntity, ImagePosition>();
            ReflectedEntities.ReflectedEntityJoined += CheckAndAddEntity;
            ReflectedEntities.ReflectedEntityAboutToLeave += CheckAndRemoveEntity;
            //some entities might join before awake/before this script is activated
            foreach (ReflectedEntity Entity in ReflectedEntities.ReflectedEntitiesList)
                CheckAndAddEntity(Entity);
            StartCoroutine(UpdateEntityPositions());
        }

        void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(UpdateEntityPositions());
        }

        protected virtual void CheckAndAddEntity(ReflectedEntity newRefEnt)
        {
            if (FilterByForceType && newRefEnt.ForceType != Type)
                return;
            if (FilterByEntityTypePattern && !newRefEnt.EntityType.MatchPattern(NetStructs.EntityType.FromString(EntityType)))
                return;
            if (FilterByMarkingText && (null == markingTextsList || markingTextsList.Count != MarkingTexts.Length)) {
                markingTextsList = new List<string>();
                foreach (string markingText in MarkingTexts)
                {
                    markingTextsList.Add(markingText.ToLower());
                }
            }
            if (FilterByMarkingText && !markingTextsList.Contains(newRefEnt.MarkingText.ToLower()))
                return;

            GameObject newImage = Instantiate(ImagePrefab, MinimapTransform);
            EntityImageDictionary.Add(newRefEnt, new ImagePosition(newImage, newImage.GetComponent<MinimapPointPositioner>()));
            newImage.transform.localScale *= ImageScale;
            Text text = newImage.GetComponentInChildren<Text>();
            if (text)
            {
                newImage.GetComponentInChildren<Text>().text = newRefEnt.MarkingText;
            }
            newImage.GetComponent<Image>().enabled = !StartHidden;
        }

        private void CheckAndRemoveEntity(ReflectedEntity leavingRefEnt)
        {
            if (EntityImageDictionary.ContainsKey(leavingRefEnt))
            {
                Destroy(EntityImageDictionary[leavingRefEnt].gameObject);
                EntityImageDictionary.Remove(leavingRefEnt);
            }
        }

        public void ShowAll()
        {
            foreach (ReflectedEntity refEnt in EntityImageDictionary.Keys)
            {
                EntityImageDictionary[refEnt].gameObject.GetComponent<Image>().enabled = true;
                StartHidden = false;
            }
        }

        public void HideAll()
        {
            foreach (ReflectedEntity refEnt in EntityImageDictionary.Keys)
            {
                EntityImageDictionary[refEnt].gameObject.GetComponent<Image>().enabled = false;
                StartHidden = true;
            }
        }

        private IEnumerator UpdateEntityPositions()
        {
            while (true)
            {
                foreach (ReflectedEntity refEnt in EntityImageDictionary.Keys)
                {
                    EntityImageDictionary[refEnt].MinimapPointPositioner.SetWorldPoint(refEnt.transform.position);
                    if (UpdateHeading)
                    {
                        EntityImageDictionary[refEnt].MinimapPointPositioner.Rotation = refEnt.transform.eulerAngles.y;
                        Text text = EntityImageDictionary[refEnt].gameObject.GetComponentInChildren<Text>();
                        if (text)
                        {
                            text.transform.localScale = refEnt.transform.eulerAngles.y > 90 && refEnt.transform.eulerAngles.y < 270 ? new Vector3(-1, -1, 1) : Vector3.one;
                        }
                    }
                }
                yield return new WaitForSeconds(UpdateInterval);
            }
        }
    }
}