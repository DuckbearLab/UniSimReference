using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefLatLonSetter : MonoBehaviour {

    public ExerciseConnection ExerciseConnection;

	// Use this for initialization
	void Start () {
        TerrainLoader.Instance.TerrainOnlyLoaded += TerrainLoaded;
	}

    public void OnDestroy()
    {
        TerrainLoader.Instance.TerrainOnlyLoaded -= TerrainLoaded;
    }

    void TerrainLoaded()
    {
        var refLatLon = FindObjectOfType<TerrainRefLatLon>();
        ExerciseConnection.SetRefLatLon(refLatLon.RefLat, refLatLon.RefLon);
    }
	
}
