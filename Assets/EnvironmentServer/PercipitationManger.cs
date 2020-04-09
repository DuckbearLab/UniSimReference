using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EnvironmentServer
{
    public class PercipitationManger : MonoBehaviour
    {
        public Toggle ClearToggle;
        public Toggle RainyToggle;
        public Toggle SnowyToggle;
        public Toggle FogToggle;
        public Toggle LightningToggle;

        [HideInInspector]
        public EventReports.EnvironmentState.WeatherState weatherState;
        [HideInInspector]
        public bool Fog;
        [HideInInspector]
        public bool Lightning;
        // Use this for initialization
        void Start()
        {
            ClearToggle.isOn = true;
            RainyToggle.isOn = false;
            SnowyToggle.isOn = false;
            FogToggle.isOn = false;
            LightningToggle.isOn = false;
            weatherState = EventReports.EnvironmentState.WeatherState.Clear;
        }

        // Update is called once per frame
        void Update()
        {
            if (ClearToggle.isOn)
                weatherState = EventReports.EnvironmentState.WeatherState.Clear;
            else
            if (RainyToggle.isOn)
                weatherState = EventReports.EnvironmentState.WeatherState.Rainy;
            else
            if (SnowyToggle.isOn)
                weatherState = EventReports.EnvironmentState.WeatherState.Snowy;
            if (FogToggle.isOn)
                Fog = true;
            else
                Fog = false;
            if (LightningToggle.isOn)
                Lightning = true;
            else
                Lightning = false;
        }
    }
}