using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ===================================================================================
 * SetScenarioData - this class takes the information from the scenario menu and inserts it into the LinePrefab
 * DESCRIPTION - all the above; that it sends the line to Scenario info to place it on the grid. 
 * =================================================================================== */
namespace EnvironmentServer
{
    public class SetScenarioData : MonoBehaviour
    {
        public Button SetScenarioButton;
        public InputField StartHouresInputField;
        public InputField StartMinutesInputField;
        public InputField StartSecondesInputField;
        public InputField DurationHouresInputField;
        public InputField DurationMinutesInputField;
        public PercipitationManger PercipitationManger;
        public Slider IntansitySlider;
        public Dropdown CloudCovregeDropdown;
        public GameObject LinePrefab;
        public ScenarioInfo ScenarioInfo;
        public GameObject ScenarioInformationGrid;
        void Start()
        {
            Button btn = SetScenarioButton.GetComponent<Button>();
            btn.onClick.AddListener(UpdateSenerioInfo);
        }

        void UpdateSenerioInfo()
        {
            SetZeroes();
            int tempHours;
            int tempMinutes;
            int tempSeconds;

            GameObject line = Instantiate(LinePrefab, ScenarioInformationGrid.transform, false);
            ((RectTransform)ScenarioInformationGrid.transform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 1, 30 * ScenarioInformationGrid.transform.childCount);
            ScenarioValues senValues = line.GetComponent<ScenarioValues>();

            int.TryParse(StartHouresInputField.text, out tempHours);
            int.TryParse(StartMinutesInputField.text, out tempMinutes);
            int.TryParse(StartSecondesInputField.text, out tempSeconds);

            senValues.StartTime.text = tempHours.ToString("00") + ":" + tempMinutes.ToString("00") + ":" + tempSeconds.ToString("00");

            int.TryParse(DurationHouresInputField.text, out tempHours);
            int.TryParse(DurationMinutesInputField.text, out tempMinutes);

            senValues.Duration.text = tempHours.ToString("00") + ":" + tempMinutes.ToString("00");

            senValues.Weather.text = PercipitationManger.weatherState.ToString();
            senValues.Fog.isOn = PercipitationManger.Fog;
            senValues.Lightning.isOn = PercipitationManger.Lightning;
            senValues.Intensity.text = IntansitySlider.value.ToString();
            senValues.CloudCovrege.text = CloudCovregeDropdown.options[CloudCovregeDropdown.value].text;
            senValues.ScenarioID = -5;
            ScenarioInfo.AddScenario(line);
        }
        void SetZeroes()
        {
            StartHouresInputField.text = StartHouresInputField.text == "" ? "00" : StartHouresInputField.text;
            StartMinutesInputField.text = StartMinutesInputField.text == "" ? "00" : StartMinutesInputField.text;
            StartSecondesInputField.text = StartSecondesInputField.text == "" ? "00" : StartSecondesInputField.text;
            DurationHouresInputField.text = DurationHouresInputField.text == "" ? "00" : DurationHouresInputField.text;
            DurationMinutesInputField.text = DurationMinutesInputField.text == "" ? "00" : DurationMinutesInputField.text;
        }
    }
}