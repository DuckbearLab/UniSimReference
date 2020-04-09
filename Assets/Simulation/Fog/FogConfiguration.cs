using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * FogConfiguration -
 * DESCRIPTION - Configures the fog on cameras. 
 * Fields:
 * - Fog (bool): Whether to add fog or not. 
 * - FogValues (FogValues) a class containing:
 * - ~ StartDistance (float): The distance at which to start the fog.
 * - ~ MaxDistance (float): the distance at which the fog draws only white. 
 * - ~ CurveExponent (Range[0f, 1f]): The curve exponent at which to draw the fog: ( inverse lerp(min, max, distance)^curve exponent )
 * =================================================================================== */

namespace Simulation.Fog
{
    public class FogConfiguration : SingletonConfiguration<FogConfiguration.Cofiguration, FogConfiguration>
    {
        [System.Serializable]
        public class FogValues
        {
            public float StartDistance, MaxDistance = 600, CurveExponent = 0.4f;
        }

        [System.Serializable]
        public class Cofiguration
        {
            public bool Fog;
            public FogValues FogValues;
        }


        protected override void ApplyConfiguration(bool GotConfiguration)
        {
            if (null == Config.FogValues)
                return;
            PutArg("Fog", ref Config.Fog);
            Config.FogValues.CurveExponent = Mathf.Clamp01(Config.FogValues.CurveExponent);
            if (Config.Fog)
            {
                foreach (GameObject obj in gameObject.scene.GetRootGameObjects())
                {
                    foreach (Camera cam in obj.GetComponentsInChildren<Camera>(true))
                    {
                        AddFog(cam);
                    }
                }
            }
        }

        public bool HasFog()
        {
            return Config.Fog;
        }

        public void AddFog(Camera cam)
        {
            if (null == Config.FogValues)
                return;
            Fog f = cam.GetComponent<Fog>() ?? cam.gameObject.AddComponent<Fog>();
            f.FogStartDistance = Config.FogValues.StartDistance;
            f.FogMaxDistance = Config.FogValues.MaxDistance;
            f.FogCurveExponent = Config.FogValues.CurveExponent;
            f.Gradient = new Gradient();
            f.Gradient.SetKeys(
                new GradientColorKey[]
                    {
                        new GradientColorKey(new Color(0f, 0f, 0f), 0f),
                        new GradientColorKey(new Color(0.9f, 0.9f, 0.9f), 0.54f),
                        new GradientColorKey(new Color(1f, 1f, 1f), 1f)
                    }, 
                    new GradientAlphaKey[] 
                    {
                        new GradientAlphaKey(1f, 0f),
                        new GradientAlphaKey(1f, 1f)
                    }
            );
        }
    }
}