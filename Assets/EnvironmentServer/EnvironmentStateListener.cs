using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetStructs;
using EventReports;
using Tenkoku.Core;
/* ===================================================================================
 * EnvironmentStateListener -
 * DESCRIPTION - this script apply the EnvironmentState event report to the tenkoku module.
 * =================================================================================== */

public class EnvironmentStateListener : MonoBehaviour
{
    public EventReportsManager EventReportsManager;

    public enum WeatherMakerCloudType
    {
        None = 0,
        Light = 1,
        Medium = 2,
        HeavyBright = 3,
        Heavy = 4,
        Storm = 5,
        Custom = 126
    }
    public TenkokuModule tenkokuObject;
    public int Day = 15;
    public int Month = 5;
    public int Year = 2019;
    public int Hour =12;
    public int Minute = 0;
    public int Second = 0;
    public float NightBrightness = 0.3f;

    public bool EnviromnentServerExists { get; private set; }

    void Start()
    {
        tenkokuObject = (TenkokuModule)FindObjectOfType(typeof(TenkokuModule));
        tenkokuObject.yearValue = Year;
        tenkokuObject.monthValue = Month;
        tenkokuObject.dayValue = Day;
        EventReportsManager.Subscribe<EnvironmentState>(EnvironmentStateCallback);
    }

    public void SetTime(int Hour, int Minute)
    {
        this.Hour = Hour;
        this.Minute = Minute;
        Second = 0;

        tenkokuObject.currentHour = Hour;
        tenkokuObject.currentMinute = Minute;
        tenkokuObject.currentSecond = Second;
    }

    public void SetTime(int Hour, int Minute, int Second)
    {
        this.Hour = Hour;
        this.Minute = Minute;
        this.Second = Second;

        tenkokuObject.currentHour = Hour;
        tenkokuObject.currentMinute = Minute;
        tenkokuObject.currentSecond = Second;
    }

    private void EnvironmentStateCallback(EnvironmentState s)
    {
        EnviromnentServerExists = true;
        Hour = (int)(s.TimeOfDay / 3600);
        Minute = (int)((s.TimeOfDay % 3600) / 60);
        Second = (int)(s.TimeOfDay % 60);

        tenkokuObject.currentHour = Hour;
        tenkokuObject.currentMinute = Minute;
        tenkokuObject.currentSecond = Second;

        tenkokuObject.weather_RainAmt = (s.weatherState == EnvironmentState.WeatherState.Rainy ? 1 : 0) * s.Intensity;
        tenkokuObject.weather_SnowAmt = (s.weatherState == EnvironmentState.WeatherState.Snowy ? 1 : 0) * s.Intensity;
        tenkokuObject.weather_lightning = (s.Lightning ? 0.5f : 0f) * s.Intensity;

        tenkokuObject.nightBrightness = NightBrightness;
        CloudsManager(CloudsCoverageConverter(s.CloudsCoverageScale));
    }
    private WeatherMakerCloudType CloudsCoverageConverter(float CloudsCoverageScale)
    {
        CloudsCoverageScale = Mathf.Pow(CloudsCoverageScale, -1f);
        CloudsCoverageScale -= 1;
        return (WeatherMakerCloudType)(int)CloudsCoverageScale;
    }
    private void CloudsManager(WeatherMakerCloudType cloudType)
    {
        switch (cloudType)
        {
            case WeatherMakerCloudType.None:
                {
                    tenkokuObject.weather_cloudAltoStratusAmt = 0;
                    tenkokuObject.weather_cloudCirrusAmt = 0;
                    tenkokuObject.weather_cloudCumulusAmt = 0;
                    tenkokuObject.weather_OvercastAmt = 0;
                    break;
                }
            case WeatherMakerCloudType.Light:
                {

                    tenkokuObject.weather_cloudAltoStratusAmt = 0.4f;
                    tenkokuObject.weather_cloudCirrusAmt = 0.35f;
                    tenkokuObject.weather_cloudCumulusAmt = 0.2f;
                    tenkokuObject.weather_OvercastAmt = 0;
                    break;
                }
            case WeatherMakerCloudType.Medium:
                {
                    tenkokuObject.weather_cloudAltoStratusAmt = 0.5f;
                    tenkokuObject.weather_cloudCirrusAmt = 0.4f;
                    tenkokuObject.weather_cloudCumulusAmt = 0.45f;
                    tenkokuObject.weather_OvercastAmt = 0.15f;
                    break;
                }
            case WeatherMakerCloudType.HeavyBright:
                {
                    tenkokuObject.weather_cloudAltoStratusAmt = 0.5f;
                    tenkokuObject.weather_cloudCirrusAmt = 0.4f;
                    tenkokuObject.weather_cloudCumulusAmt = 1f;
                    tenkokuObject.weather_OvercastAmt = 0.2f;
                    break;
                }
            case WeatherMakerCloudType.Heavy:
                {
                    tenkokuObject.weather_cloudAltoStratusAmt = 0.5f;
                    tenkokuObject.weather_cloudCirrusAmt = 0.4f;
                    tenkokuObject.weather_cloudCumulusAmt = 1f;
                    tenkokuObject.weather_OvercastAmt = 0.35f;
                    break;
                }
            case WeatherMakerCloudType.Storm:
                {
                    tenkokuObject.weather_cloudAltoStratusAmt = 0.5f;
                    tenkokuObject.weather_cloudCirrusAmt = 0.4f;
                    tenkokuObject.weather_cloudCumulusAmt = 1;
                    tenkokuObject.weather_OvercastAmt = 1f;
                    break;
                }
        }
    }
}