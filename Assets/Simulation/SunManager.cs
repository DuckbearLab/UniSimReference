using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
    public class SunManager : MonoBehaviour
    {
        public SimulationTime SimulationTime;
        public float SunriseTimeInMinutes = 360;

        private void Update()
        {
            float sunPosition = GetSunPosition();
            gameObject.transform.eulerAngles = new Vector3(sunPosition, -90, 0);
        }

        private float GetSunPosition()
        {
            return (float)(SimulationTime.TimeInMinutes - SunriseTimeInMinutes) * 0.25f;
        }
    }
}