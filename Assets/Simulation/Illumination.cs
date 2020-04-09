using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
    public class Illumination : MonoBehaviour
    {
        [System.Serializable]
        public struct IntensityRange
        {
            public float Min;
            public float Max;
        }

        public SimulationTime SimulationTime;
        public Light Light;
        public AnimationCurve IlluminationCurve;

        public IntensityRange LightIntensity;
        public IntensityRange EnvironmentIntensity;

        private const float OneOverMinutesInDay = 1f/1440f;

        private void Update()
        {
            float curveValue = IlluminationCurve.Evaluate(SimulationTime.TimeInMinutes * OneOverMinutesInDay);
            float lightIntensity = LightIntensity.Min + (curveValue * (LightIntensity.Max - LightIntensity.Min));
            Light.intensity = lightIntensity;

            float ambientIntensity = EnvironmentIntensity.Min + (curveValue * (EnvironmentIntensity.Max - EnvironmentIntensity.Min));
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.reflectionIntensity = ambientIntensity;
        }
    }
}