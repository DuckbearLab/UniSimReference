using UnityEngine;

public class SkyTime : MonoBehaviour
{
    public float DayLengthInMinutes = 30;

    public static bool ProgressTime       = true;
    public bool ProgressDate              = true;
    public bool ProgressMoonPhase         = true;

    private Sky sky;

    protected void Start()
    {
        sky = GetComponent<Sky>();
    }

    protected void Update()
    {
        float oneDay = DayLengthInMinutes * 60;
        float oneHour = oneDay / 24;

        float hourIter = Time.deltaTime / oneHour;
        float moonIter = Time.deltaTime / (30*oneDay) * 2;

        if (ProgressTime)
        {
            sky.Cycle.TimeOfDay += hourIter;

            if (ProgressMoonPhase)
            {
                sky.Moon.Phase += moonIter;
                if (sky.Moon.Phase < -1) sky.Moon.Phase += 2;
                else if (sky.Moon.Phase > 1) sky.Moon.Phase -= 2;
            }

            if (sky.Cycle.TimeOfDay >= 24)
            {
                sky.Cycle.TimeOfDay = 0;

                if (ProgressDate)
                {
                    sky.Cycle.JulianDate = (sky.Cycle.JulianDate < 365) ? (sky.Cycle.JulianDate + 1) : 1;
                }
            }
        }
    }
}
