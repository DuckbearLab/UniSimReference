using UnityEngine;

public class SkyWeather : MonoBehaviour
{
    public enum WeatherType
    {
        Custom,
        Default,
        Clear,
        Cloudy,
        Stormy
    }

    public float FadeTime = 10f;
    public WeatherType Type = WeatherType.Custom;
    private WeatherType type = WeatherType.Custom;

    private Sky sky;

    private float dayBrightnessDefault;
    private float cloudToneDefault;
    private float cloudDensityDefault;
    private float cloudSharpnessDefault;

    private float dayBrightness;
    private float cloudTone;
    private float cloudDensity;
    private float cloudSharpness;

    protected void OnEnable()
    {
        sky = GetComponent<Sky>();

        dayBrightness  = dayBrightnessDefault  = sky.Day.Brightness;
        cloudTone      = cloudToneDefault      = sky.Clouds.Tone;
        cloudDensity   = cloudDensityDefault   = sky.Clouds.Density;
        cloudSharpness = cloudSharpnessDefault = sky.Clouds.Sharpness;
    }

    protected void Update()
    {
        if (Type != type)
        {
            switch (Type)
            {
                case WeatherType.Default:
                    dayBrightness  = dayBrightnessDefault;
                    cloudTone      = cloudToneDefault;
                    cloudDensity   = cloudDensityDefault;
                    cloudSharpness = cloudSharpnessDefault;
                    break;

                case WeatherType.Custom:
                    dayBrightness  = sky.Day.Brightness;
                    cloudTone      = sky.Clouds.Tone;
                    cloudDensity   = sky.Clouds.Density;
                    cloudSharpness = sky.Clouds.Sharpness;
                    break;

                case WeatherType.Clear:
                    dayBrightness  = 10f;
                    cloudTone      = 1.5f;
                    cloudDensity   = 0.0f;
                    cloudSharpness = 1.0f;
                    break;

                case WeatherType.Cloudy:
                    dayBrightness  = 10f;
                    cloudTone      = Random.Range(1.5f, 2.0f);
                    cloudDensity   = Random.Range(1.0f, 2.0f);
                    cloudSharpness = Random.Range(0.5f, 2.0f);
                    break;

                case WeatherType.Stormy:
                    dayBrightness  = 0.5f;
                    cloudTone      = Random.Range(0.1f, 0.2f);
                    cloudDensity   = Random.Range(4.0f, 5.0f);
                    cloudSharpness = Random.Range(0.1f, 0.2f);
                    break;
            }
            type = Type;
        }
        else
        {
            // FadeTime is not exact as the fade smoothens a little towards the end
            float t = Time.deltaTime / FadeTime;
            sky.Day.Brightness   = Mathf.Lerp(sky.Day.Brightness,   dayBrightness,  t);
            sky.Clouds.Tone      = Mathf.Lerp(sky.Clouds.Tone,      cloudTone,      t);
            sky.Clouds.Density   = Mathf.Lerp(sky.Clouds.Density,   cloudDensity,   t);
            sky.Clouds.Sharpness = Mathf.Lerp(sky.Clouds.Sharpness, cloudSharpness, t);
        }
    }
}
