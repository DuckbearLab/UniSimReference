using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/* ===================================================================================
 * ActiveScenario - this class finds the active scenario, marks him and sets him on the main Canvas
 * DESCRIPTION - at update we use FindActiveScenario, and if we found one we will activate it.
 * when the senerio overs we either deleting it or unmarking it.
 * =================================================================================== */
namespace EnvironmentServer
{
    public class ActiveScenario : MonoBehaviour
    {
        public ScenarioInfo ScenarioInfo;
        public Slider TimeOfDaySlider;
        public bool DeleteAfterFinished = true;
        [Header("UI Objects")]
        public Dropdown CloudCovregeDropdown;
        public Slider IntensitySlider;
        public Toggle ClearToggle;
        public Toggle RainyToggle;
        public Toggle SnowyToggle;
        public Toggle FogToggle;
        public Toggle LightningToggle;

        private ScenarioValues ActiveScenarioValues;
        private ScenarioValues TempScenarioValues;
        //dufault values
        private string defWeather;
        private string defIntensity;
        private string defCloudCovrege;
        private bool defFog;
        private bool defightning;

        private int timeOfDay = 0;
        void Start()
        {
            //whene a Senerio overs the weather will set to be defaultive
            defWeather = "Clear";
            defIntensity = "1";
            defCloudCovrege = "Light";
            defFog = false;
            defightning = false;
        }
        void Update()
        {
            timeOfDay = (int)TimeOfDaySlider.value;
            TempScenarioValues = FindActiveScenario();
            if (TempScenarioValues != null)
            {
                if (ActiveScenarioValues != null)
                {
                    if (TempScenarioValues != ActiveScenarioValues)
                    {
                        ScenarioInfo.RemoveScenarios(ActiveScenarioValues.ScenarioID);
                    }
                }
                ActiveScenarioValues = TempScenarioValues;
                SetValuesOfActive(ActiveScenarioValues);
                if (!(ActiveScenarioValues.CloudCovrege.color == Color.red || ActiveScenarioValues.CloudCovrege.color == Color.yellow))
                    ColorActive(ActiveScenarioValues, true);
            }
            else
            {
                if (ActiveScenarioValues != null)
                {
                    SetValuesOfActive();
                    if (DeleteAfterFinished)
                        ScenarioInfo.RemoveScenarios(ActiveScenarioValues.ScenarioID);
                    else
                        ColorActive(ActiveScenarioValues, false);
                    ActiveScenarioValues = null;
                }
            }

        }
        private ScenarioValues FindActiveScenario()
        {
            int startTime;
            int endTime;
            foreach (var sen in ScenarioInfo.GetScenariosList())
            {
                startTime = GetTimeInSecconds(sen.StartTime.text);
                endTime = startTime + GetTimeInSecconds(sen.Duration.text);
                if (endTime < 86400)
                {
                    if (timeOfDay > startTime && timeOfDay < endTime)
                        return sen;
                }
                else
                {
                    if (timeOfDay > startTime || timeOfDay < endTime % 86400)
                        return sen;
                }
            }
            return null;
        }
        //the startTime and Duration strings are in HH:MM:SS or HH:MM formats so we convert them to seconds.
        private int GetTimeInSecconds(string str)
        {
            int Time = 0;
            int counter = 0;
            int tempInt = 0;
            int i = 0;
            char s = str[i];
            string temp = "";
            while (i < str.Length)
            {
                s = str[i];
                if (s != ':')
                    temp += s;
                else
                {
                    counter++;
                    int.TryParse(temp, out tempInt);
                    Time = Time * 60 + tempInt;
                    tempInt = 0;
                    temp = "";
                }
                i++;
            }
            int.TryParse(temp, out tempInt);
            Time = Time * 60 + tempInt;
            if (counter == 1)
            {
                Time *= 60;
            }
            return Time;
        }
        private void ColorActive(ScenarioValues senValues, bool isActive)
        {
            Color color;
            if (isActive)
                color = Color.red;
            else
                color = Color.black;
            senValues.StartTime.color = color;
            senValues.Duration.color = color;
            senValues.Weather.color = color;
            senValues.Intensity.color = color;
            senValues.CloudCovrege.color = color;
        }
        private void SetValuesOfActive(ScenarioValues senValues)
        {
            float temp = 0;
            foreach (var option in CloudCovregeDropdown.options)
            {
                if (option.text == senValues.CloudCovrege.text)
                {
                    CloudCovregeDropdown.value = (int)temp;
                    break;
                }
                temp++;
            }
            float.TryParse(senValues.Intensity.text, out temp);
            IntensitySlider.value = temp;
            if (senValues.Weather.text == "Clear")
            {
                ClearToggle.isOn = true;
            }
            else
            {
                if (senValues.Weather.text == "Rainy")
                {
                    RainyToggle.isOn = true;
                }
                else
                {
                    if (senValues.Weather.text == "Snowy")
                    {
                        SnowyToggle.isOn = true;
                    }
                }
            }

            FogToggle.isOn = senValues.Fog.isOn;
            LightningToggle.isOn = senValues.Lightning.isOn;
        }
        private void SetValuesOfActive()
        {
            float temp = 0;
            foreach (var option in CloudCovregeDropdown.options)
            {
                if (option.text == defCloudCovrege)
                {
                    CloudCovregeDropdown.value = (int)temp;
                    break;
                }
                temp++;
            }
            float.TryParse(defIntensity, out temp);
            IntensitySlider.value = temp;
            if (defWeather == "Clear")
            {
                ClearToggle.isOn = true;
            }
            else
            {
                if (defWeather == "Rainy")
                {
                    RainyToggle.isOn = true;
                }
                else
                {
                    if (defWeather == "Snowy")
                    {
                        SnowyToggle.isOn = true;
                    }
                }
            }

            FogToggle.isOn = defFog;
            LightningToggle.isOn = defightning;
        }
    }
}