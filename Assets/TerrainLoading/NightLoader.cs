using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class NightLoader : MonoBehaviour {

    public bool NightAvailable { get; private set; }
    public bool DayAvailable { get; private set; }
    public bool IsDay { get; set; }

    public GameObject Environment;

    public enum Mode { DayOnly, NightOnly, Dual }
    public Mode NightLoadingMode = Mode.DayOnly;

    private Color initialCamColor;
    private Material skybox;
    private Color initialAmbientLight;

    private TerrainLoader dayTerrainLoader;
    private TerrainLoader nightTerrainLoader;

    private const float nightIntesnity = 1f;

    //why?
    //private bool loadNight = false;

    public void SwitchMode()
    {
        if ((IsDay && !NightAvailable) || (!IsDay && !DayAvailable)) return;
        IsDay = !IsDay;
        Refresh();
    }

    public void SwitchToDay()
    {
        if (!DayAvailable || IsDay) return;
        IsDay = true;
        Refresh();
    }

    public void SwitchToNight()
    {
        if (!NightAvailable || !IsDay) return;
        IsDay = false;
        Refresh();
    }

    private void Refresh()
    {
        if(DayAvailable)
            dayTerrainLoader.LoadedScene.GetRootGameObjects()[0].SetActive(IsDay);
        if(NightAvailable)
            nightTerrainLoader.LoadedScene.GetRootGameObjects()[0].SetActive(!IsDay);

        //Environment.SetActive(IsDay);

        foreach (Camera cam in Camera.allCameras)
        {
            cam.backgroundColor = IsDay ? initialCamColor : new Color(nightIntesnity, nightIntesnity, nightIntesnity);
        }

        RenderSettings.ambientMode = IsDay ? UnityEngine.Rendering.AmbientMode.Skybox : UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = IsDay ? initialAmbientLight : new Color(nightIntesnity, nightIntesnity, nightIntesnity);
        RenderSettings.skybox = IsDay ? skybox : null;


        foreach (var grayscale in FindAllGrayscales())
            grayscale.enabled = !IsDay;

        /*RenderSettings.fog = !day;
        RenderSettings.fogColor = Color.black;*/
    }

    private static Grayscale[] grayscales = null;

    private static Grayscale[] FindAllGrayscales()
    {
        if(grayscales == null)
        {
            List<Grayscale> foundGrayscales = new List<Grayscale>();
            foreach(var gameObj in SceneManager.GetActiveScene().GetRootGameObjects())
                foundGrayscales.AddRange(gameObj.transform.GetComponentsInChildren<Grayscale>(true));

            grayscales = foundGrayscales.ToArray();
        }
        return grayscales;
    }


    void Start()
    {
        //if there isn't a main camera in the start, wait a second and try again.
        if (Camera.main == null)
        {
           new Timer(1f, Start);
           return;
        } 
        NightAvailable = false;
        DayAvailable = false;
        IsDay = true;

        initialCamColor = Camera.main.backgroundColor;
        skybox = RenderSettings.skybox;
        initialAmbientLight = RenderSettings.ambientLight;

        LoadNeededTerrain();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            SwitchMode();
    }

    private void LoadNeededTerrain()
    {
        if(NightLoadingMode == Mode.DayOnly)
        {
            dayTerrainLoader = TerrainLoader.Instance;
            dayTerrainLoader.TerrainFullyLoaded += DayTerrainLoaded;
        }
        else if (NightLoadingMode == Mode.NightOnly)
        {
            nightTerrainLoader = TerrainLoader.Instance;
            nightTerrainLoader.TerrainFullyLoaded += NightTerrainLoaded;
        }
        else if (NightLoadingMode == Mode.Dual)
        {
            dayTerrainLoader = TerrainLoader.Instance;
            dayTerrainLoader.TerrainFullyLoaded += DayTerrainLoaded;

            var dayTerrainPath = dayTerrainLoader.TerrainBundlePath;
            var nightTerrainPath = dayTerrainPath + "_night";

            if (File.Exists(nightTerrainPath))
            {
                nightTerrainLoader = TerrainLoader.Instance;
                nightTerrainLoader.TerrainLoadingCanvasPrefab = dayTerrainLoader.TerrainLoadingCanvasPrefab;
                nightTerrainLoader.TerrainBundlePath = nightTerrainPath;
                nightTerrainLoader.TerrainFullyLoaded += NightTerrainLoaded;
            }
        }

        
    }

    private void NightTerrainLoaded()
    {
        NightAvailable = true;
        if (NightLoadingMode == Mode.NightOnly)
            SwitchToNight();
        else
            SwitchToDay();
    }

    private void DayTerrainLoaded()
    {
        DayAvailable = true;
        if (NightLoadingMode == Mode.NightOnly)
            SwitchToNight();
        else
            SwitchToDay();
    }

}
