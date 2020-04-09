using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * TerrainMeasurer - contains the terrain's xy bounderies.
 * this is meant to be a class that is called, not put in before start.
\ * =================================================================================== */

public class TerrainMeasurer : SingletonMonoBehaviour<TerrainMeasurer>
{
    [HideInInspector]
    public float topX, buttomX, topZ, buttomZ;

    protected override void OnNewInstanceCreated()
    {
        TerrainMinimap minimap = TerrainLoader.Instance.LoadedScene.GetRootGameObjects()[0].GetComponent<TerrainMinimap>();
        topX = minimap.MapMax[0];
        topZ = minimap.MapMax[1];
        buttomX = minimap.MapMin[0];
        buttomZ = minimap.MapMin[1];
    }

    /// <summary>
    /// checks if the point is in the bounderies of the terrain.
    /// </summary>
    /// <param name="point">the position</param>
    /// <returns>whether the point is in the bounderies of the terrain.</returns>
    public static bool IsInTerrain(Vector3 point)
    {
        TerrainMeasurer instance = TerrainMeasurer.Instance;
        if(point.x < instance.buttomX||
            point.x > instance.topX||
            point.z < instance.buttomZ||
            point.z > instance.topZ)
        {
            return false;
        }
        return true;
    }
}
