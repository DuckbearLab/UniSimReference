using NetStructs;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDB;

namespace EventReports
{

    #region Event Report Attribute
    public class EventReportAttribute : Attribute
    {
        public uint EventReportType { get; private set; }

        public EventReportAttribute(uint eventReportType)
        {
            EventReportType = eventReportType;
        }
    }

    public class EventReportArrayAttribute : Attribute
    {
        public string ArrayLengthField { get; private set; }

        public EventReportArrayAttribute(string arrayLengthField)
        {
            ArrayLengthField = arrayLengthField;
        }
    }

    public class EventReportCalculatedAttribute : Attribute
    {
        public EventReportCalculatedAttribute()
        {
        }
    }
    #endregion

    #region BlDataStruct

    public abstract class BlDataStruct
    {
        public EntityId SenderId { get; set; }
        public EntityId ReceiverId { get; set; }
    }

    public abstract class WorldPointsSquareMessage : BlDataStruct
    {
        public Vector3 TopLeft;
        public Vector3 TopRight;
        public Vector3 BottomRight;
        public Vector3 BottomLeft;

        public void SetPoints(Vector3 TopLeft, Vector3 TopRight, Vector3 BottomRight, Vector3 BottomLeft)
        {
            this.TopLeft = TopLeft;
            this.TopRight = TopRight;
            this.BottomRight = BottomRight;
            this.BottomLeft = BottomLeft;
        }
    }

    #endregion

    #region Event Reports
    /// <summary>
    /// contains the system time and scenerio time in seconds.
    /// <seealso cref="WatchUtils.TimeInSecondsFormatToString(float)"/> returns the variables as a string of days:hours:minutes:seconds
    /// </summary>
    [EventReport(1)]
    public class SimTimeStruct : BlDataStruct
    {
        //the time that passed from when the scenario started in seconds.
        public float time;
        //world time in seconds.
        public int realTime;
    }
    #endregion

}