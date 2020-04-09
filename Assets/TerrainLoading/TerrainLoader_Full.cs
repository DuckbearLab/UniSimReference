using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainLoader_Full : TerrainLoader
{
    public PutCopy PutCopyFull { get; private set; }

    private AsyncOperation assetLoadRequest = null;


    // Use this for initialization
    void Start()
    {
        LoadState = TerrainLoadState.Before;
        if (SceneManager.sceneCount == 1)
            StartCoroutine(LoadTerrain());
    }

    private IEnumerator LoadTerrain()
    {
        Instantiate(TerrainLoadingCanvasPrefab).TerrainLoader = this;

        yield return null;
        //Load Navmesh's asset bundle
        if (!string.IsNullOrEmpty(NavMeshBundlePath))
        {
            AssetBundleCreateRequest navMeshBundleRequest = AssetBundle.LoadFromFileAsync(NavMeshBundlePath);

            AssetBundle loadedNavMeshBundle = navMeshBundleRequest.assetBundle;

            var navMesh = loadedNavMeshBundle.LoadAssetAsync("navmesh");
            yield return navMesh;
        }
        //Load the terrain's assetbundle
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(AssetBundlePath);
        yield return bundleLoadRequest;

        LoadState = TerrainLoadState.Loading;
        var loadedAssetBundle = bundleLoadRequest.assetBundle;
        var terrainSceneName = Path.GetFileNameWithoutExtension(loadedAssetBundle.GetAllScenePaths()[0]);

        assetLoadRequest = SceneManager.LoadSceneAsync(loadedAssetBundle.GetAllScenePaths()[0], LoadSceneMode.Additive);
        yield return assetLoadRequest;

        loadedAssetBundle.Unload(false);

        LoadedScene = SceneManager.GetSceneByName(terrainSceneName);

        LoadState = TerrainLoadState.TerrainOnly;

        if (ImprovedTerrain)
            StartCoroutine(LoadImprovedTerrainCoroutine());

        Invoke_TerrainOnlyLoaded();

        PutCopyFull = LoadedScene.GetRootGameObjects()[0].GetComponentInChildren<PutCopy>();
        PutCopyFull.Done += OptimizeTerrain;

        if (Application.isEditor)
        {
            LoadState = TerrainLoadState.BeforeCombine;

            Invoke_TerrainFullyLoadedBeforeCombine();

            LoadState = TerrainLoadState.Fully;

            Invoke_TerrainFullyLoaded();
        }
    }

    private void OptimizeTerrain()
    {
        if (!Application.isEditor)
        {
            LoadState = TerrainLoadState.BeforeCombine;

            Invoke_TerrainFullyLoadedBeforeCombine();

            StaticBatchingUtility.Combine(LoadedScene.GetRootGameObjects()[0]);
            LoadState = TerrainLoadState.Fully;

            Invoke_TerrainFullyLoaded();
        }

    }

    private IEnumerator LoadImprovedTerrainCoroutine()
    {
        Invoke_LoadingImrovedTerrain();

        Debug.Log("Start loading the imroved terrain");

        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(ImprovedTerrainBundlePath);
        yield return bundleLoadRequest;

        var loadedAssetBundle = bundleLoadRequest.assetBundle;
        /*var terrainSceneName = */
        Path.GetFileNameWithoutExtension(loadedAssetBundle.GetAllScenePaths()[0]); //assigned but never used

        assetLoadRequest = SceneManager.LoadSceneAsync(loadedAssetBundle.GetAllScenePaths()[0], LoadSceneMode.Additive);
        yield return assetLoadRequest;

        loadedAssetBundle.Unload(false);

        Debug.Log("Finished loading the imroved terrain");

        Invoke_TerrainFullyLoadedBeforeCombine();

        Invoke_TerrainFullyLoaded();

    }

    private void Update()
    {
        if (assetLoadRequest != null && !assetLoadRequest.isDone)
            Invoke_Loading(assetLoadRequest.progress);
    }

    private string AssetBundlePath
    {
        get
        {
            if (!string.IsNullOrEmpty(TerrainBundlePath))
                return TerrainBundlePath;

            throw new Exception("No terrain bundle provided!");
        }
    }

}

#region Old
/*
 * using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainLoader : MonoBehaviour {

    public enum TerrainLoadState { Before, Loading, TerrainOnly, BeforeCombine, Fully}

    public string TerrainBundlePath;

    public event Action<float> Loading;
    public event Action TerrainOnlyLoaded;
    public event Action TerrainFullyLoadedBeforeCombine;
    public event Action TerrainFullyLoaded;

    public TerrainLoadState LoadState { get; private set; }

    public PutCopy PutCopy;

    public Scene LoadedScene { get; private set; }

    public TerrainLoadingCanvas TerrainLoadingCanvasPrefab;

    private AsyncOperation assetLoadRequest = null;

    private void Awake()
    {

#if DYNAMIC_LOADING
        enabled = false;
#else
        enabled = true;
#endif

    }


	// Use this for initialization
	void Start () {
        LoadState = TerrainLoadState.Before;
        if (SceneManager.sceneCount == 1)
            StartCoroutine(LoadTerrain());
	}

    private IEnumerator LoadTerrain()
    {
        #if !DYNAMIC_LOADING
        Instantiate<TerrainLoadingCanvas>(TerrainLoadingCanvasPrefab).TerrainLoader = this;
#endif

        yield return null;

        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(AssetBundlePath);
        yield return bundleLoadRequest;

        LoadState = TerrainLoadState.Loading;
        var loadedAssetBundle = bundleLoadRequest.assetBundle;
        var terrainSceneName = Path.GetFileNameWithoutExtension(loadedAssetBundle.GetAllScenePaths()[0]);

        assetLoadRequest = SceneManager.LoadSceneAsync(loadedAssetBundle.GetAllScenePaths()[0], LoadSceneMode.Additive);
        yield return assetLoadRequest;

        loadedAssetBundle.Unload(false);

        LoadedScene = SceneManager.GetSceneByName(terrainSceneName);

        LoadState = TerrainLoadState.TerrainOnly;
        if (TerrainOnlyLoaded != null)
            TerrainOnlyLoaded();

        PutCopy = LoadedScene.GetRootGameObjects()[0].GetComponentInChildren<PutCopy>();
        PutCopy.Done += OptimizeTerrain;

        if (Application.isEditor)
        {
            LoadState = TerrainLoadState.BeforeCombine;
            if (TerrainFullyLoadedBeforeCombine != null)
                TerrainFullyLoadedBeforeCombine();

            LoadState = TerrainLoadState.Fully;
            if (TerrainFullyLoaded != null)
                TerrainFullyLoaded();
        }
    }

    private void OptimizeTerrain()
    {
        if (!Application.isEditor)
        {
            LoadState = TerrainLoadState.BeforeCombine;
            if (TerrainFullyLoadedBeforeCombine != null)
                TerrainFullyLoadedBeforeCombine();

            StaticBatchingUtility.Combine(LoadedScene.GetRootGameObjects()[0]);
            LoadState = TerrainLoadState.Fully;
            if (TerrainFullyLoaded != null)
                TerrainFullyLoaded();
        }

    }

    private void Update()
    {
        if (assetLoadRequest != null && !assetLoadRequest.isDone)
            if(Loading != null)
                Loading(assetLoadRequest.progress);
    }

    private string AssetBundlePath
    {
        get
        {
            if (!string.IsNullOrEmpty(TerrainBundlePath))
                return TerrainBundlePath;

            throw new Exception("No terrain bundle provided!");
        }
    }
	
}

 * */
#endregion
