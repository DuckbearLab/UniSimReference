using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using NetStructs;
using UnityEngine.UI;

namespace EnvironmentServer
{
    public class SendStatus : MonoBehaviour
    {
        public EventReportsManager EventReportsManager;
        public ClockDisplay ClockDisplay;
        public Dropdown CloudCovregeDropdown;
        public Slider IntensitySlider;
        public PercipitationManger PercipitationManger;

        private int hour;
        private int minute;
        private int timeMultiplier = 1;
        private EventReports.EnvironmentState s;
        // Use this for initialization
        void Start()
        {
            InvokeRepeating("SendTimeStatus", 0, 1);
        }
        void Update()
        {

        }
        void SendTimeStatus()
        {
            minute = ClockDisplay.minutes;
            hour = ClockDisplay.houres;

            float time = (hour * 60 + ((float)minute)) * 60;
            float clouds = (float)1 / (CloudCovregeDropdown.value + 1);
            int windSpeed = 2;
            float windDirX = 1;
            float windDirY = 0;

            s = new EventReports.EnvironmentState()
            {
                TimeOfDay = time,
                TimeMultiplier = timeMultiplier,
                CloudsCoverageScale = clouds,
                Intensity = IntensitySlider.value,
                WindSpeed = windSpeed,
                WindDirX = windDirX,
                WindDirY = windDirY,
                Fog = PercipitationManger.Fog,
                Lightning = PercipitationManger.Lightning,
                weatherState = PercipitationManger.weatherState
            };

            EventReportsManager.Send(s);

        }
    }
}