using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * TerrainLightingIntensity -
 * DESCRIPTION - changes the intensity of the lighting depending on the terrain.
 * =================================================================================== */
namespace Simulation
{
    public class TerrainLightingIntensity : MonoBehaviour
    {
        public SerializableDictionary.StringFloatDictionary TerrainIntensityDict;
        public Tenkoku.Core.TenkokuModule TenkokuModule;

        private void Start()
        {
            string bundlePath = TerrainLoader.Instance.TerrainBundlePath.ToLower();
            // Get terrian name      
            foreach (var pair in TerrainIntensityDict)
            {
                if (bundlePath.Contains(pair.Key.ToLower()))
                {
                    TenkokuModule.sunBright = pair.Value;
                    break;
                }
            }
            Destroy(this);
        }
    }
}