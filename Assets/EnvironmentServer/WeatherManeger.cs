using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventReports;

public class WeatherManeger : MonoBehaviour {

    public EventReportsManager EventReportsManager;
    public DigitalRuby.WeatherMaker.WeatherMakerScript WeatherMakerScript;
    public DigitalRuby.WeatherMaker.WeatherMakerConfigurationScript WeatherMakerConfigurationScript;
    public Simulation.Illumination NonWMSun;

    private static bool Fog;
    private bool Lightning;
    

    // Use this for initialization
    void Start ()
    {
        EventReportsManager.Subscribe<EnvironmentState>(EnvironmentStateCallback);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    private void EnvironmentStateCallback(EnvironmentState s)
    {
        WeatherMakerScript.Precipitation = WeatherConverter(s.weatherState);
        WeatherMakerScript.Clouds = CloudsCoverageConverter(s.CloudsCoverageScale);
        WeatherMakerScript.PrecipitationIntensity= s.Intensity;
        if (Fog != s.Fog)
        {
            WeatherMakerConfigurationScript.FogChanged(s.Fog);
            Fog = s.Fog;
        }
        if (Lightning != s.Lightning)
        {
            DigitalRuby.WeatherMaker.WeatherMakerScript.Instance.LightningScript.EnableLightning = s.Lightning;
            Lightning = s.Lightning;
        }
        if (WeatherMakerScript.isActiveAndEnabled)
            NonWMSun.LightIntensity.Min = NonWMSun.LightIntensity.Max = 0;
    }

    private DigitalRuby.WeatherMaker.WeatherMakerPrecipitationType WeatherConverter(EnvironmentState.WeatherState WeatherState)
    {
        if (WeatherState == EnvironmentState.WeatherState.Clear)
            return DigitalRuby.WeatherMaker.WeatherMakerPrecipitationType.None;
        if (WeatherState == EnvironmentState.WeatherState.Rainy)
            return DigitalRuby.WeatherMaker.WeatherMakerPrecipitationType.Rain;
        if (WeatherState == EnvironmentState.WeatherState.Snowy)
            return DigitalRuby.WeatherMaker.WeatherMakerPrecipitationType.Snow;
        return DigitalRuby.WeatherMaker.WeatherMakerPrecipitationType.None;
    }
    private DigitalRuby.WeatherMaker.WeatherMakerCloudType CloudsCoverageConverter(float CloudsCoverageScale)
    {
        CloudsCoverageScale = Mathf.Pow(CloudsCoverageScale, -1f);
        CloudsCoverageScale -= 1;
        return (DigitalRuby.WeatherMaker.WeatherMakerCloudType)(int)CloudsCoverageScale;
    }
}
