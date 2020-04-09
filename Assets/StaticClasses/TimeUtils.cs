using System.Collections;
using System.Collections.Generic;
using EventReports;

/* ===================================================================================
 * TimeUtils - utils for sim time struct.
 * NOTES - StringBuilder is a class for building strings with performance conservation in mind.
 * =================================================================================== */

public static class TimeUtils
{
    /// <summary>
    /// returns a string of the time as day:hour:minute:second. 
    /// <para>
    /// days are now optional.
    /// </para>
    /// </summary>
    /// <returns></returns>
    public static string TimeInSecondsFormatToString(float time, bool addDays = false)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if(addDays)
        {
            sb.Append(((int)(time / 86400))).Append(':'); //days
        }
        int hours = ((int)(time % 86400 / 3600));
        if (hours < 10)
            sb.Append(0);
        sb.Append(hours).Append(':'); //hours
        int minutes = (int)(time % 3600 / 60);
        if (minutes < 10)
            sb.Append(0);
        sb.Append(minutes).Append(':'); //minutes
        int seconds = ((int)(time % 60));
        if (seconds < 10)
            sb.Append(0);
        sb.Append(seconds); //seconds
        return sb.ToString();
    }

    /// <summary>
    /// returns a string of the real time as hour:minute:second
    /// </summary>
    /// <returns></returns>
    public static string GetRealTimeToString(char column = ':')
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        System.DateTime now = System.DateTime.Now;
        if (now.Hour < 10)
            sb.Append(0);
        sb.Append(now.Hour).Append(column); //hours
        if (now.Minute < 10)
            sb.Append(0);
        sb.Append(now.Minute).Append(column); //minutes
        if (now.Second < 10)
            sb.Append(0);
        sb.Append(now.Second); //seconds
        return sb.ToString();
    }
}
