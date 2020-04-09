using UnityEngine;
using System;

[Serializable]
public class CycleParameters
{
    public float TimeOfDay  = 12f;  // [  0,   24  ] Hour of the day
    public float JulianDate = 180f; // [  1,   365 ] Day of the year, going from Jan 1st [=1] to Dec 31st [=365]
    public float Latitude   = 0f;   // [ -90,  90  ] Horizontal earth lines, 0 is located at the equator
    public float Longitude  = 0f;   // [ -180, 180 ] Vertical earth lines, 0 is located at Greenwich, England
}

[Serializable]
public class DayParameters
{
    public float RayleighMultiplier = 1.0f;
    public float MieMultiplier      = 0.1f;
    public float Brightness         = 10.0f;
    public float Haziness           = 0.5f;
    public Color Color              = Color.black;
}

[Serializable]
public class NightParameters
{
    public float Haziness   = 0.5f;
    public Color Color      = Color.black;
    public Color HazeColor  = Color.black;
    public Color CloudColor = Color.clear;
}

[Serializable]
public class SunParameters
{
    public float LightIntensity = 0.75f;
    public float Falloff        = 1.25f;
    public float Coloring       = 0.25f;
}

[Serializable]
public class MoonParameters
{
    public float LightIntensity = 0.05f;
    public float Phase          = 0.0f;
    public Color HaloColor      = Color.black;
}

[Serializable]
public class CloudParameters
{
    public float Tone      = 1.5f;
    public float Shading   = 0.5f;
    public float Density   = 1.0f;
    public float Sharpness = 1.0f;
    public float Speed1    = 0.5f;
    public float Speed2    = 1.0f;
    public float Scale1    = 3.0f;
    public float Scale2    = 7.0f;
}

[ExecuteInEditMode]
public class Sky : MonoBehaviour
{
    // Component references
    // These are already setup in the prefab
    public GameObject SunInstance        = null;
    public GameObject MoonInstance       = null;
    public GameObject AtmosphereInstance = null;
    public GameObject CloudInstance      = null;

    // Color space (can be linear or gamma)
    // This has to be set by the user
    public bool LinearLighting = false;

    // Dome parameters
    public CycleParameters Cycle  = null;
    public DayParameters   Day    = null;
    public NightParameters Night  = null;
    public SunParameters   Sun    = null;
    public MoonParameters  Moon   = null;
    public CloudParameters Clouds = null;

    // Shader parameters
    private Vector3 betaRayleigh;
    private Vector3 betaRayleighTheta;
    private Vector3 betaMie;
    private Vector3 betaMieTheta;
    private Vector3 henyeyGreenstein;

    // Component pointers for faster access
    internal Transform DomeTransform;
    internal Material  AtmosphereShader;
    internal Material  CloudShader;
    internal Transform SunTransform;
    internal Renderer  SunRenderer;
    internal Material  SunShader;
    internal Material  SunHalo;
    internal Light     SunLight;
    internal Transform MoonTransform;
    internal Renderer  MoonRenderer;
    internal Material  MoonShader;
    internal Material  MoonHalo;
    internal Light     MoonLight;

    // Property and state accessors
    internal bool IsDay
    {
        get { return SunLight.enabled; }
    }
    internal bool IsNight
    {
        get { return MoonLight.enabled; }
    }
    internal float OrbitRadius
    {
        get { return DomeTransform.localScale.x; }
    }
    internal float Gamma
    {
        get { return LinearLighting ? 1.0f : 2.2f; }
    }
    internal float OneOverGamma
    {
        get { return LinearLighting ? 1.0f/1.0f : 1.0f/2.2f; }
    }

    // Red wavelength
    const float lambda_r1 = 700.0e-9f;
    const float lambda_r2 = lambda_r1 * lambda_r1;
    const float lambda_r4 = lambda_r2 * lambda_r2;

    // Green wavelength
    const float lambda_g1 = 575.0e-9f;
    const float lambda_g2 = lambda_g1 * lambda_g1;
    const float lambda_g4 = lambda_g2 * lambda_g2;

    // Blue wavelength
    const float lambda_b1 = 450.0e-9f;
    const float lambda_b2 = lambda_b1 * lambda_b1;
    const float lambda_b4 = lambda_b2 * lambda_b2;

    // Powers of PI
    const float pi = Mathf.PI;
    const float pi2 = pi*pi;
    const float pi3 = pi*pi2;
    const float pi4 = pi2*pi2;

    // Unity: OnEnable
    protected void OnEnable()
    {
        DomeTransform = transform;

        if (AtmosphereInstance)
        {
            AtmosphereShader = AtmosphereInstance.GetComponent<Renderer>().sharedMaterial;
        }

        if (CloudInstance)
        {
            CloudShader = CloudInstance.GetComponent<Renderer>().sharedMaterial;
        }

        if (SunInstance)
        {
            SunTransform = SunInstance.transform;
            SunRenderer = SunInstance.GetComponent<Renderer>();
            SunShader = SunRenderer.sharedMaterial;
            SunLight = SunInstance.GetComponent<Light>();

            if (SunRenderer.sharedMaterials.Length > 1)
                SunHalo = SunRenderer.sharedMaterials[1];
        }
        else
        {
            Debug.LogError("Sun instance reference not set. Disabling script.");
            this.enabled = false;
        }

        if (MoonInstance)
        {
            MoonTransform = MoonInstance.transform;
            MoonRenderer = MoonInstance.GetComponent<Renderer>();
            MoonShader = MoonRenderer.sharedMaterial;
            MoonLight = MoonInstance.GetComponent<Light>();

            if (MoonRenderer.sharedMaterials.Length > 1)
                MoonHalo = MoonRenderer.sharedMaterials[1];
        }
        else
        {
            Debug.LogError("Moon instance reference not set. Disabling script.");
            this.enabled = false;
        }
    }

    // Unity: Update
    protected void Update()
    {
        ValidateScriptParameters();
        SetupRayleighScattering();
        SetupMieScattering();
        SetupHenyeyGreensteinPhaseFunction();
        SetupSunAndMoon();
        SetupShaderParameters();
    }

    private void SetupShaderParameters()
    {
        Vector3 objSpaceSunDir = DomeTransform.InverseTransformDirection(SunTransform.forward);

        Color sunColorGamma        = AdjustColorGamma(SunLight.color);
        Color dayColorGamma        = AdjustColorGamma(Day.Color * Day.Color.a);
        Color nightColorGamma      = AdjustColorGamma(Night.Color * Night.Color.a);
        Color nightHazeColorGamma  = AdjustColorGamma(Night.HazeColor * Night.HazeColor.a);
        Color nightCloudColorGamma = AdjustColorGamma(Night.CloudColor);

        Color sunHaloColor = SunLight.color;
        sunHaloColor.a *= SunLight.intensity / Sun.LightIntensity;

        Color moonHaloColor = Moon.HaloColor;
        moonHaloColor.a *= MoonLight.intensity / Moon.LightIntensity;

        if (AtmosphereShader != null)
        {
            AtmosphereShader.SetColor("_SunColor", sunColorGamma);
            AtmosphereShader.SetVector("_SunDirection", objSpaceSunDir);

            AtmosphereShader.SetFloat("_OneOverGamma", OneOverGamma);
            AtmosphereShader.SetFloat("_DayBrightness", Day.Brightness);
            AtmosphereShader.SetFloat("_DayHaziness", Day.Haziness);
            AtmosphereShader.SetFloat("_NightHaziness", Night.Haziness);
            AtmosphereShader.SetColor("_DayColor", dayColorGamma);
            AtmosphereShader.SetColor("_NightColor", nightColorGamma);
            AtmosphereShader.SetColor("_NightHazeColor", nightHazeColorGamma);
            AtmosphereShader.SetVector("_BetaRayleigh", betaRayleigh);
            AtmosphereShader.SetVector("_BetaRayleighTheta", betaRayleighTheta);
            AtmosphereShader.SetVector("_BetaMie", betaMie);
            AtmosphereShader.SetVector("_BetaMieTheta", betaMieTheta);
            AtmosphereShader.SetVector("_HenyeyGreenstein", henyeyGreenstein);
        }

        if (CloudShader != null)
        {
            CloudShader.SetColor("_LightColor", Color.Lerp(nightCloudColorGamma, sunColorGamma, sunColorGamma.a));
            CloudShader.SetVector("_LightDirection", objSpaceSunDir);

            CloudShader.SetFloat("_OneOverGamma", OneOverGamma);
            CloudShader.SetFloat("_CloudTone", Clouds.Tone);
            CloudShader.SetFloat("_CloudDensity", Clouds.Density);
            CloudShader.SetFloat("_CloudSharpness", Clouds.Sharpness);
            CloudShader.SetFloat("_CloudShading", Clouds.Shading);
            CloudShader.SetFloat("_CloudSpeed1", Clouds.Speed1);
            CloudShader.SetFloat("_CloudSpeed2", Clouds.Speed2);
            CloudShader.SetFloat("_CloudScale1", Clouds.Scale1);
            CloudShader.SetFloat("_CloudScale2", Clouds.Scale2);
        }

        if (MoonShader != null)
        {
            MoonShader.SetFloat("_Phase", Moon.Phase);
        }

        if (MoonHalo != null)
        {
            MoonHalo.SetColor("_Color", moonHaloColor);
        }

        if (SunShader != null)
        {
            SunShader.SetColor("_Color", sunHaloColor);
        }

        if (SunHalo != null)
        {
            SunHalo.SetColor("_Color", sunHaloColor);
        }
    }

    // Henyey Greenstein phase function - may vary with viewer height at some point in the future
    private void SetupHenyeyGreensteinPhaseFunction()
    {
        const float g = 0.55f; // Directionality factor

        // See [2] page 27
        henyeyGreenstein.x = 1-g*g;
        henyeyGreenstein.y = 1+g*g;
        henyeyGreenstein.z = 2*g;
    }

    // Rayleigh scattering
    private void SetupRayleighScattering()
    {
        const float n = 1.0003000f; // Refractive index of air in the visible spectrum
        const float N = 2.5450e25f; // Number of molecules per unit volume

        // See [1] page 23
        // See [2] page 24 equation (2.2)
        betaRayleigh.x = Day.RayleighMultiplier * 8*pi3 * (n*n-1)*(n*n-1) / (3*N*lambda_r4);
        betaRayleigh.y = Day.RayleighMultiplier * 8*pi3 * (n*n-1)*(n*n-1) / (3*N*lambda_g4);
        betaRayleigh.z = Day.RayleighMultiplier * 8*pi3 * (n*n-1)*(n*n-1) / (3*N*lambda_b4);

        // See [1] page 23
        // See [2] page 24 equation (2.1)
        betaRayleighTheta.x = Day.RayleighMultiplier * pi2 * (n*n-1)*(n*n-1) / (2*N*lambda_r4);
        betaRayleighTheta.y = Day.RayleighMultiplier * pi2 * (n*n-1)*(n*n-1) / (2*N*lambda_g4);
        betaRayleighTheta.z = Day.RayleighMultiplier * pi2 * (n*n-1)*(n*n-1) / (2*N*lambda_b4);
    }

    // Mie scattering
    private void SetupMieScattering()
    {
        const float t = 1.0f;                             // Turbidity
        const float c = (0.6544f * t - 0.6510f) * 1e-16f; // Concentration factor

        // Wavelength-dependent constants
        const float k_r = 0.687112f;
        const float k_g = 0.679802f;
        const float k_b = 0.665996f;

        // See [1] page 23
        // See [2] page 26 equation (2.5)
        betaMie.x = Day.MieMultiplier * 0.434f*c * 4f*pi2 / lambda_r2 * k_r * pi;
        betaMie.y = Day.MieMultiplier * 0.434f*c * 4f*pi2 / lambda_g2 * k_g * pi;
        betaMie.z = Day.MieMultiplier * 0.434f*c * 4f*pi2 / lambda_b2 * k_b * pi;

        // See [1] page 23
        // See [2] page 25 equation (2.3)
        betaMieTheta.x = Day.MieMultiplier * 0.434f*c * 4f*pi2 / lambda_r2 * 0.5f;
        betaMieTheta.y = Day.MieMultiplier * 0.434f*c * 4f*pi2 / lambda_g2 * 0.5f;
        betaMieTheta.z = Day.MieMultiplier * 0.434f*c * 4f*pi2 / lambda_b2 * 0.5f;
    }

    // Calculate sun and moon position
    // See [1] page 26
    private void SetupSunAndMoon()
    {
        // Solar latitude
        float latitudeRadians = Mathf.Deg2Rad * Cycle.Latitude;
        float latitudeRadiansSin = Mathf.Sin(latitudeRadians);
        float latitudeRadiansCos = Mathf.Cos(latitudeRadians);

        // Solar longitude
        float longitudeRadians = Mathf.Deg2Rad * Cycle.Longitude;

        // Solar declination - constant for the whole globe at any given day
        float solarDeclination = 0.4093f * Mathf.Sin(2f*pi/368f * (Cycle.JulianDate-81f));
        float solarDeclinationSin = Mathf.Sin(solarDeclination);
        float solarDeclinationCos = Mathf.Cos(solarDeclination);

        // Solar time
        float timeZone = (int)(Cycle.Longitude/15f);
        float meridian = Mathf.Deg2Rad * 15f * timeZone;
        float solarTime = Cycle.TimeOfDay
                        + 0.170f * Mathf.Sin(4f*pi/373f * (Cycle.JulianDate-80f))
                        - 0.129f * Mathf.Sin(2f*pi/355f * (Cycle.JulianDate-8f))
                        + 12f/pi * (meridian - longitudeRadians);
        float solarTimeRadians = pi/12f * solarTime;
        float solarTimeSin = Mathf.Sin(solarTimeRadians);
        float solarTimeCos = Mathf.Cos(solarTimeRadians);

        // Solar altitude angle - angle between the sun and the horizon
        float solarAltitudeSin = latitudeRadiansSin * solarDeclinationSin
                               - latitudeRadiansCos * solarDeclinationCos * solarTimeCos;
        float solarAltitude = Mathf.Asin(solarAltitudeSin);

        // Solar azimuth angle - angle of the sun around the horizon
        float solarAzimuthY = - solarDeclinationCos * solarTimeSin;
        float solarAzimuthX = latitudeRadiansCos * solarDeclinationSin
                            - latitudeRadiansSin * solarDeclinationCos * solarTimeCos;
        float solarAzimuth = Mathf.Atan2(solarAzimuthY, solarAzimuthX);

        // Convert solar angles to spherical coordinates
        float theta = pi/2 - solarAltitude;
        float phi   = solarAzimuth;

        // Update sun position
        SunTransform.position = DomeTransform.position
                              + DomeTransform.rotation * SphericalToCartesian(OrbitRadius, theta, phi);
        SunTransform.LookAt(DomeTransform.position);

        // Update moon position
        // The following calculation is not physically correct and might be changed at some point in the future
        // However, as most people don't pay attention to the moon position this should be a decent workaround for now
        MoonTransform.position = DomeTransform.position
                               + DomeTransform.rotation * SphericalToCartesian(OrbitRadius, theta + pi, phi);
        MoonTransform.LookAt(DomeTransform.position);

        // Update sun and fog color according to the new position of the sun
        SetupSunAndMoonColor(theta);
    }

    // Calculate sun and moon color
    // See [1] page 21
    private void SetupSunAndMoonColor(float theta)
    {
        // Relative optical mass (air mass coefficient approximated by a spherical shell)
        // See http://en.wikipedia.org/wiki/Air_mass_(solar_energy)
        float cosTheta = Mathf.Cos(theta * Mathf.Pow(Mathf.Abs(theta/pi), 1/Sun.Falloff));
        float m = Mathf.Sqrt(708f*708f * cosTheta*cosTheta + 2*708f + 1) - 708f*cosTheta;

        // Wavelengths in micrometers
        const float lambda_r = lambda_r1*1e6f; // [um]
        const float lambda_g = lambda_g1*1e6f; // [um]
        const float lambda_b = lambda_b1*1e6f; // [um]

        // Transmitted sun color
        float r = 1, g = 1, b = 1, a = 1;

        // 1. Transmittance due to Rayleigh scattering of air molecules
        const float rayleigh_beta  = 0.008735f;
        const float rayleigh_alpha = 4.08f;
        r *= Mathf.Exp(-rayleigh_beta * Mathf.Pow(lambda_r, -rayleigh_alpha * m));
        g *= Mathf.Exp(-rayleigh_beta * Mathf.Pow(lambda_g, -rayleigh_alpha * m));
        b *= Mathf.Exp(-rayleigh_beta * Mathf.Pow(lambda_b, -rayleigh_alpha * m));

        // 2. Angstrom's turbididty formula for aerosal (does not improve anything visually)
        //const float aerosol_turbidity = 1.0f;
        //const float aerosal_beta = 0.04608f * aerosol_turbidity - 0.04586f;
        //const float aerosal_alpha = 1.3f;
        //r *= Mathf.Exp(-aerosal_beta * Mathf.Pow(lambda_r, -aerosal_alpha * m));
        //g *= Mathf.Exp(-aerosal_beta * Mathf.Pow(lambda_g, -aerosal_alpha * m));
        //b *= Mathf.Exp(-aerosal_beta * Mathf.Pow(lambda_b, -aerosal_alpha * m));

        // 3. Transmittance due to ozone absorption (does not improve anything visually)
        //const float ozone_l  = 0.350f; // [cm]
        //const float ozone_kr = 0.067f; // [1/cm]
        //const float ozone_kg = 0.040f; // [1/cm]
        //const float ozone_kb = 0.009f; // [1/cm]
        //r *= Mathf.Exp(-ozone_kr * Mathf.Pow(lambda_r, -ozone_l * m));
        //g *= Mathf.Exp(-ozone_kg * Mathf.Pow(lambda_g, -ozone_l * m));
        //b *= Mathf.Exp(-ozone_kb * Mathf.Pow(lambda_b, -ozone_l * m));

        // Make sure rgb values are valid and calculate sun alpha value
        // These checks are required as it appears that there is in fact such thing as negative (or NaN) colors!
        r = r > 0 ? r : 0;
        g = g > 0 ? g : 0;
        b = b > 0 ? b : 0;
        a = Mathf.Max(Mathf.Max(r, g), b);

        // Set sun and fog color
        Color sun_aaaa = new Color(a, a, a, a);
        Color sun_rgba = new Color(r, g, b, a);
        SunLight.color = RenderSettings.fogColor = Color.Lerp(sun_aaaa, sun_rgba, Sun.Coloring);

        // Sun altitude and intensity dropoff angle
        float altitude = pi/2 - theta;
        float altitude_abs = Mathf.Abs(altitude);
        float dropoff_rad = 10 * Mathf.Deg2Rad;

        // Set sun and moon intensity
        if (altitude > 0)
        {
            // Disable moon
            MoonLight.enabled = MoonRenderer.enabled = false;
            MoonLight.intensity = 0;

            // Enable sun
            SunLight.enabled = SunRenderer.enabled = true;
            float sunIntensityMax = Sun.LightIntensity;
            if (altitude_abs < dropoff_rad)
            {
                SunLight.intensity = Mathf.Lerp(0, sunIntensityMax, altitude_abs / dropoff_rad);
            }
            else SunLight.intensity = sunIntensityMax;
        }
        else
        {
            // Disable sun
            SunLight.enabled = SunRenderer.enabled = false;
            SunLight.intensity = 0;

            // Enable moon
            MoonLight.enabled = MoonRenderer.enabled = true;
            float moonIntensityMax = Moon.LightIntensity * Mathf.Clamp01(1 - Mathf.Abs(Moon.Phase));
            if (altitude_abs < dropoff_rad)
            {
                MoonLight.intensity = Mathf.Lerp(0, moonIntensityMax, altitude_abs / dropoff_rad);
            }
            else MoonLight.intensity = moonIntensityMax;
        }
    }

    // Make the parameters stay within some sort of reasonable range
    private void ValidateScriptParameters()
    {
        // Cycle
        Cycle.TimeOfDay = Mathf.Repeat(Cycle.TimeOfDay, 24);
        Cycle.JulianDate = Mathf.Repeat(Cycle.JulianDate - 1, 365) + 1;
        Cycle.Longitude = Mathf.Clamp(Cycle.Longitude, -180, 180);
        Cycle.Latitude = Mathf.Clamp(Cycle.Latitude, -90, 90);

        // Day
        #if UNITY_EDITOR
        Day.MieMultiplier = Mathf.Max(0, Day.MieMultiplier);
        Day.RayleighMultiplier = Mathf.Max(0, Day.RayleighMultiplier);
        Day.Brightness = Mathf.Max(0, Day.Brightness);
        Day.Haziness = Mathf.Max(0, Day.Haziness);
        #endif

        // Night
        #if UNITY_EDITOR
        Night.Haziness = Mathf.Max(0, Night.Haziness);
        #endif

        // Sun
        #if UNITY_EDITOR
        Sun.LightIntensity = Mathf.Max(0, Sun.LightIntensity);
        Sun.Falloff = Mathf.Max(0, Sun.Falloff);
        Sun.Coloring = Mathf.Max(0, Sun.Coloring);
        #endif

        // Moon
        #if UNITY_EDITOR
        Moon.LightIntensity = Mathf.Max(0, Moon.LightIntensity);
        Moon.Phase = Mathf.Clamp(Moon.Phase, -1, +1);
        #endif

        // Clouds
        #if UNITY_EDITOR
        Clouds.Tone = Mathf.Max(0, Clouds.Tone);
        Clouds.Shading = Mathf.Max(0, Clouds.Shading);
        Clouds.Density = Mathf.Max(0, Clouds.Density);
        Clouds.Sharpness = Mathf.Max(0, Clouds.Sharpness);
        #endif
    }

    // Convert spherical coordinates to cartesian coordinates
    private Vector3 SphericalToCartesian(float radius, float theta, float phi)
    {
        Vector3 res;

        float sinTheta = Mathf.Sin(theta);
        float cosTheta = Mathf.Cos(theta);
        float sinPhi   = Mathf.Sin(phi);
        float cosPhi   = Mathf.Cos(phi);

        res.x = radius * sinTheta * cosPhi;
        res.y = radius * cosTheta;
        res.z = radius * sinTheta * sinPhi;

        return res;
    }

    // Adjust a color according to the current color space
    private Color AdjustColorGamma(Color c)
    {
        if (LinearLighting) return c;
        return new Color(Mathf.Pow(c.r, Gamma), Mathf.Pow(c.g, Gamma), Mathf.Pow(c.b, Gamma), Mathf.Pow(c.a, Gamma));
    }
}
