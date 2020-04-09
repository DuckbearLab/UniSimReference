using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* ===================================================================================
 * SelectScenario - 
 * DESCRIPTION -this class detect scenerio selections and control the selected scenario,
 * it can show selection by color and delet it using the delet key when a scenario is selected.
 * =================================================================================== */
namespace EnvironmentServer
{
    public class SelectScenario : MonoBehaviour, IPointerClickHandler
    {
        public ScenarioInfo ScenarioInfo;
        public GameObject LinePrefab;
        public Color SelectedColor;
        public GameObject LastSelection { get; private set; }

        private string prefabName;


        public void Awake()
        {
            LastSelection = null;
            prefabName = LinePrefab.name;
        }
        void Update()
        {
            CheckDeleteClick();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            GameObject obj = eventData.hovered.Find(o => o.name.Contains(prefabName));

            if (obj == null)
            {
                ColorSelected(LastSelection, false);
                return;
            }
            ColorSelected(LastSelection, false);
            ColorSelected(obj, true);
            LastSelection = obj;

        }
        private void ColorSelected(GameObject obj, bool isSelected)
        {
            if (obj == null)
                return;
            ScenarioValues senValues = obj.GetComponent<ScenarioValues>();
            Color color = Color.black;
            if (IsActive(obj))
                color = Color.red;
            if (isSelected)
            {
                if (IsActive(obj))
                    color = Color.yellow;
                else
                    color = SelectedColor;
            }
            senValues.StartTime.color = color;
            senValues.Duration.color = color;
            senValues.Weather.color = color;
            senValues.Intensity.color = color;
            senValues.CloudCovrege.color = color;
        }
        private void CheckDeleteClick()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                if (LastSelection != null)
                {
                    var senValues = LastSelection.GetComponent<ScenarioValues>();
                    ScenarioInfo.RemoveScenarios(senValues.ScenarioID);
                    LastSelection = null;
                }
            }
        }
        private bool IsActive(GameObject obj)
        {
            ScenarioValues senValues = obj.GetComponent<ScenarioValues>();
            if (senValues.CloudCovrege.color == Color.red || senValues.CloudCovrege.color == Color.yellow)
                return true;
            return false;
        }

    }
}