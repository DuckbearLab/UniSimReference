using Simulation.Minimap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ===================================================================================
 * NoTerrainMinimap -
 * DESCRIPTION - Loads the minimap witout terrian
 * =================================================================================== */

public class NoTerrainMinimap : MonoBehaviour
{

    [System.Serializable]
    public struct TerrainMinMaxPoints
    {
        public string TerrainName;
        public Vector2 MapMin;
        public Vector2 MapMax;
        public Vector2 LatLon;
        public Texture2D MinimapTexture;
        public int MapTexturePartsColumns;
        public int MapTexturePartsRows;
    }

    public List<TerrainMinMaxPoints> MinimapsMinMax;

    public string WantedTerrainName;


    public TerrainMinMaxPoints GetTerrainMinimapInfo()
    {
        foreach (TerrainMinMaxPoints terrainInfo in MinimapsMinMax)
        {
            if (terrainInfo.TerrainName == WantedTerrainName || terrainInfo.TerrainName + "_night" == WantedTerrainName)
            {
                return terrainInfo;
            }                          
        }

        TerrainMinMaxPoints NullTerrain = new TerrainMinMaxPoints();

        NullTerrain.TerrainName = "NullTerrain";

        return NullTerrain; 
    }
}