using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ===================================================================================
 * ScenarioValues -saves Values To LinePrefab
 * DESCRIPTION -
 * =================================================================================== */
namespace EnvironmentServer
{
    public class ScenarioValues : MonoBehaviour
    {
        public Text StartTime;
        public Text Duration;
        public Text Weather;
        public Toggle Fog;
        public Toggle Lightning;
        public Text Intensity;
        public Text CloudCovrege;

        public int ScenarioID { get; set; }
        public Vector3 Position { get; set; }


        public void GetConfigInfo(EnvironmentServerConfiguration.ConfigScenarioLine line)
        {
            StartTime.text = line.StartTime;
            Duration.text = line.Duration;
            Weather.text = line.Weather;
            Fog.isOn = line.Fog;
            Lightning.isOn = line.Lightning;
            Intensity.text = line.Intensity;
            CloudCovrege.text = line.CloudCovrege;
            ScenarioID = -5;
        }
    }
}