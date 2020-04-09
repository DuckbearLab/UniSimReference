using NetStructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ===================================================================================
 * ScenarioInfo -this class manage the grid, it can add rebove and clean the grid from infoLines
 * DESCRIPTION -
 * =================================================================================== */
namespace EnvironmentServer
{
    public class ScenarioInfo : MonoBehaviour
    {
        public GameObject LinePrefab;
        public RectTransform ScenarioInformationGrid;
        public Scrollbar VerticalScrollbar;
        public float LineOffset;
        public int MaxLines;
        public Color TrackTextColor;

        public event System.Action OnScenarioAdded;

        public List<RectTransform> ScenarioLines;
        private List<ScenarioValues> ScenarioValues;
        private float lineHeight;
        private int id;
        private float originalHeight;
        const string StartTimeString = "{00}:{00}:{00}";
        const string DurationString = "{00}:{00}";

        public void Awake()
        {
            ScenarioLines = new List<RectTransform>();
            ScenarioValues = new List<ScenarioValues>();

            lineHeight = LinePrefab.GetComponent<RectTransform>().rect.height;
            id = 0;

            StartCoroutine(GetScenarioInfoGridOriginalHeight());
        }

        //Adds a Scenario to the list and a symbol to the list.
        public int AddScenario(GameObject LinePrefab, int ScenarioID = -1)
        {
            // If the list is full, delete the last element.
            if (ScenarioLines.Count == MaxLines)
            {
                GameObject obj = ScenarioLines[0].gameObject;

                ScenarioValues.Remove(obj.GetComponent<ScenarioValues>());
                Destroy(obj);
                ScenarioLines.RemoveAt(0);
            }

            // Calculate the position of each line in the list.
            foreach (RectTransform senLine in ScenarioLines)
            {
                senLine.position -= new Vector3(0, lineHeight + LineOffset, 0);
            }

            int currentID = ScenarioID == -1 ? id : ScenarioID;

            ScenarioValues senValues = LinePrefab.GetComponent<ScenarioValues>();
            senValues.ScenarioID = currentID;


            ScenarioValues.Add(senValues);

            if (ScenarioInformationGrid.rect.height - (ScenarioLines.Count * (lineHeight + LineOffset)) < lineHeight + LineOffset)
                ScenarioInformationGrid.offsetMin = new Vector2(ScenarioInformationGrid.offsetMin.x, ScenarioInformationGrid.offsetMin.y - (lineHeight + LineOffset));

            ScenarioLines.Add(LinePrefab.GetComponent<RectTransform>());

            if (ScenarioID == -1)
                id++;

            if (OnScenarioAdded != null)
                OnScenarioAdded();

            return currentID;
        }

        // Remove all Scenarios by ID.
        public void RemoveScenarios(int ScenarioID)
        {
            List<RectTransform> senLines = new List<RectTransform>(ScenarioLines);

            foreach (RectTransform senLine in senLines)
            {
                ScenarioValues senVal = senLine.gameObject.GetComponent<ScenarioValues>();

                // If Scenario should be removed, delete it and position
                // the other elements in the grid accordingly.
                if (senVal.ScenarioID == ScenarioID)
                {
                    ScenarioValues.Remove(senVal);

                    Destroy(senLine.gameObject);

                    int removedIndex = ScenarioLines.IndexOf(senLine);

                    if (removedIndex > 0)
                    {
                        for (int i = removedIndex - 1; i >= 0; --i)
                        {
                            ScenarioLines[i].position += new Vector3(0, lineHeight + LineOffset, 0);
                        }
                    }

                    if (ScenarioInformationGrid.rect.height > originalHeight)
                    {
                        ScenarioInformationGrid.offsetMin = new Vector2(ScenarioInformationGrid.offsetMin.x, ScenarioInformationGrid.offsetMin.y + (lineHeight + LineOffset));
                        VerticalScrollbar.value += 1 / (lineHeight + LineOffset);
                    }

                    ScenarioLines.Remove(senLine);
                }
            }
        }

        //public void RemoveAllScenarios()
        //{
        //    foreach (var scen in ScenarioValues)
        //    {
        //        RemoveScenarios(scen.ScenarioID);
        //    }
        //}

        // In the start of the run, the height is 0 for some reason.
        private IEnumerator GetScenarioInfoGridOriginalHeight()
        {
            while (true)
            {
                if (ScenarioInformationGrid.rect.height != 0)
                {
                    originalHeight = ScenarioInformationGrid.rect.height;
                    break;
                }

                yield return null;
            }
        }

        public List<ScenarioValues> GetScenariosList()
        {
            return ScenarioValues;
        }

    }

}