using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainLoaderOld : MonoBehaviour
{

    public string TerrainBundlePath;

    public event Action<float> Loading;
    public event Action TerrainOnlyLoaded;
    public event Action TerrainFullyLoadedBeforeCombine;
    public event Action TerrainFullyLoaded;

    public Scene LoadedScene { get; private set; }

    public TerrainLoadingCanvas TerrainLoadingCanvasPrefab;

    private AsyncOperation assetLoadRequest = null;

    // Use this for initialization
    void Start()
    {
        if (SceneManager.sceneCount == 1)
            StartCoroutine(LoadTerrain());
    }

    private IEnumerator LoadTerrain()
    {
        //Instantiate<TerrainLoadingCanvas>(TerrainLoadingCanvasPrefab).TerrainLoader = this;

        yield return null;

        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(AssetBundlePath);
        yield return bundleLoadRequest;

        var loadedAssetBundle = bundleLoadRequest.assetBundle;
        var terrainSceneName = Path.GetFileNameWithoutExtension(loadedAssetBundle.GetAllScenePaths()[0]);

        assetLoadRequest = SceneManager.LoadSceneAsync(loadedAssetBundle.GetAllScenePaths()[0], LoadSceneMode.Additive);
        yield return assetLoadRequest;

        loadedAssetBundle.Unload(false);

        LoadedScene = SceneManager.GetSceneByName(terrainSceneName);


        if (TerrainOnlyLoaded != null)
            TerrainOnlyLoaded();

        if (Application.isEditor)
        {
            if (TerrainFullyLoadedBeforeCombine != null)
                TerrainFullyLoadedBeforeCombine();

            if (TerrainFullyLoaded != null)
                TerrainFullyLoaded();
        }

        LoadedScene.GetRootGameObjects()[0].GetComponentInChildren<PutCopy>().Done += OptimizeTerrain;
    }

    private void OptimizeTerrain()
    {
        if (!Application.isEditor)
        {
            if (TerrainFullyLoadedBeforeCombine != null)
                TerrainFullyLoadedBeforeCombine();

            StaticBatchingUtility.Combine(LoadedScene.GetRootGameObjects()[0]);

            if (TerrainFullyLoaded != null)
                TerrainFullyLoaded();
        }

    }

    private void Update()
    {
        if (assetLoadRequest != null && !assetLoadRequest.isDone)
            if (Loading != null)
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

