using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

/* ===================================================================================
* TerrainLoaderParent - 
* Holds all the basic data for the terrain loading, like  TerrainBundlePath and
 * all the events.
* =================================================================================== */

public abstract class TerrainLoader : MonoBehaviour
{

    #region Singleton
    public static TerrainLoader Instance { get; set; }

    protected virtual void Awake()
    {
        if (null == Instance)
            Instance = this;
    }
    #endregion

    public enum TerrainLoadState { Before, Loading, TerrainOnly, BeforeCombine, Fully }

    public TerrainLoadingCanvas TerrainLoadingCanvasPrefab;
    [Header("Asset Bundles")]
    public string TerrainBundlePath;
    public string NavMeshBundlePath = "";

    [Header("Imroved Terrain")]
    public string ImprovedTerrainBundlePath;
    public bool ImprovedTerrain;

    public TerrainLoadState LoadState { get; protected set; }

    /// <summary>
    /// Returns the current PutCopy of either the dynamic or full loading, whichever one exists, as a base class that has their common members. 
    /// </summary>
    public PutCopy_Base PutCopy
    {
        get
        {
            return
                (IsDynamic) ?
                    (PutCopy_Base)(this as TerrainLoader_Dynamic).PutCopyDynamic :
                    (PutCopy_Base)(this as TerrainLoader_Full).PutCopyFull;
        }
    }

    /// <summary>
    /// Gets whether the selected loading mode is dynamic. 
    /// </summary>
    public bool IsDynamic { get { return this is TerrainLoader_Dynamic; } }

    public Scene LoadedScene { get; protected set; }

    public event Action<float> Loading;
    public event Action LoadingImrovedTerrain;
    public event Action TerrainOnlyLoaded;
    public event Action TerrainFullyLoadedBeforeCombine;
    public event Action TerrainFullyLoaded;

    #region Events Invoke
    protected void Invoke_Loading(float progress)
    {
        if (Loading != null)
            Loading(progress);
    }

    protected void Invoke_TerrainOnlyLoaded()
    {
        if (TerrainOnlyLoaded != null)
            TerrainOnlyLoaded();
    }

    protected void Invoke_TerrainFullyLoadedBeforeCombine()
    {
        LoadState = TerrainLoadState.BeforeCombine;
     
        if (TerrainFullyLoadedBeforeCombine != null)
            TerrainFullyLoadedBeforeCombine();
    }

    protected void Invoke_TerrainFullyLoaded()
    {
        LoadState = TerrainLoadState.Fully;

        if (TerrainFullyLoaded != null)
            TerrainFullyLoaded();
    }

    protected void Invoke_LoadingImrovedTerrain()
    {
        if (LoadingImrovedTerrain != null)
            LoadingImrovedTerrain();
    }
    #endregion

}
