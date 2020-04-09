using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLoadingCanvas : MonoBehaviour 
{

    public TerrainLoader TerrainLoader;

    public UnityEngine.UI.Text StatusText;

	// Use this for initialization
	void Start () 
    {
        TerrainLoader.Instance.TerrainFullyLoaded += TerrainLoader_TerrainFullyLoaded;
        TerrainLoader.Instance.LoadingImrovedTerrain += TerrainLoader_LoadingImrovedTerrain;
        TerrainLoader.Instance.TerrainOnlyLoaded += TerrainLoader_TerrainOnlyLoaded;
	}

    public void OnDestroy()
    {
        TerrainLoader.Instance.TerrainFullyLoaded -= TerrainLoader_TerrainFullyLoaded;
    }

    private void TerrainLoader_TerrainFullyLoaded()
    {
        TerrainLoader.Instance.TerrainFullyLoaded -= TerrainLoader_TerrainFullyLoaded;
        Destroy(gameObject);
    }

    private void TerrainLoader_TerrainOnlyLoaded()
    {
        if(StatusText != null)
            StatusText.text = HebrewUtils.Fix("טוען בניינים");
    }

    private void TerrainLoader_LoadingImrovedTerrain()
    {
        if (StatusText != null)
            StatusText.text = HebrewUtils.Fix("טוען שטח מועשר");
    }
	
}
