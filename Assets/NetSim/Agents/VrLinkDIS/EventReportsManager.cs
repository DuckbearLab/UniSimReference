using EventReports;
using NetStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReportsManager : SingletonMonoBehaviour<EventReportsManager>
{
    public ExerciseConnection ExerciseConnection;

    private Dictionary<uint, List<Action<BlDataStruct>>> subscribersByEventType;

    private static Dictionary<uint, Type> eventReportsTypes;

    // Storing this to prevent garbage collection
    private NetSimAgent.EventReportReceivedCallback eventReportReceivedCallback;

    static EventReportsManager()
    {
        eventReportsTypes = new Dictionary<uint, Type>();
        foreach(var type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
        {
            var eventReportAttr = GetEventreportAttribute(type);
            if (eventReportAttr != null)
                eventReportsTypes[eventReportAttr.EventReportType] = type;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (null == subscribersByEventType)
            subscribersByEventType = new Dictionary<uint, List<Action<BlDataStruct>>>();
    }

    void Start()
    {
        eventReportReceivedCallback = new NetSimAgent.EventReportReceivedCallback(EventReportReceived);
        NetSimAgent.Instance.SetEventReportReceivedCallback(ExerciseConnection.ExerciseConnectionPtr, eventReportReceivedCallback);
    }

    public void Subscribe<EventReportType>(Action<EventReportType> handler) where EventReportType : BlDataStruct
    {
        if(subscribersByEventType==null)
        {
            subscribersByEventType = new Dictionary<uint, List<Action<BlDataStruct>>>();
        }
        var eventReportAttr = GetEventreportAttribute(typeof(EventReportType));

        if (eventReportAttr == null)
            throw new Exception("Event report type must have EventReportAttribute");

        var eventReportType = eventReportAttr.EventReportType;

        if (!subscribersByEventType.ContainsKey(eventReportType))
            subscribersByEventType[eventReportType] = new List<Action<BlDataStruct>>();

        subscribersByEventType[eventReportType].Add((BlDataStruct obj) => { handler((EventReportType)obj); });
    }

    private void EventReportReceived(uint eventType, IntPtr pduPtr)
    {
        if(subscribersByEventType.ContainsKey(eventType) && subscribersByEventType[eventType].Count > 0)
        {
           if(eventReportsTypes.ContainsKey(eventType))
           {
               Type eventReportType = eventReportsTypes[eventType];
               BlDataStruct o = (BlDataStruct)Activator.CreateInstance(eventReportType);
               
               var eventReportData = new EventReportEncoder(pduPtr, o);

               o.SenderId = eventReportData.SenderId;
               o.ReceiverId = eventReportData.ReceiverId;

               foreach (var field in eventReportType.GetFields())
               {
                    field.SetValue(o, eventReportData.Read(field.FieldType, field));
               }

               foreach (var subscriber in subscribersByEventType[eventType])
                   subscriber(o);
           }
        }
    }
    

    public void Send(BlDataStruct eventReport)
    {
        var eventReportAttr = GetEventreportAttribute(eventReport);

        if (eventReportAttr == null)
            throw new Exception("Event report must have EventReportAttribute");

        var eventReportData = new EventReportEncoder(eventReportAttr.EventReportType, eventReport);

        eventReportData.SenderId = eventReport.SenderId;
        eventReportData.ReceiverId = eventReport.ReceiverId;

        foreach (var field in eventReport.GetType().GetFields())
        {
            if (field.Name == "SenderId" || field.Name == "ReceiverId")
            {
                continue;
            }
            var value = field.GetValue(eventReport);
            eventReportData.Write(value, field);
        }

        NetSimAgent.Instance.SendEventReportInteraction(ExerciseConnection.ExerciseConnectionPtr, eventReportData.EventReportPduPtr);
    }

    private static EventReportAttribute GetEventreportAttribute(object eventReport)
    {
        return GetEventreportAttribute(eventReport.GetType());
    }

    private static EventReportAttribute GetEventreportAttribute(Type eventReportType)
    {
        var attrs = eventReportType.GetCustomAttributes(typeof(EventReportAttribute), true);
        if (attrs.Length == 0) return null;
        return (EventReportAttribute)attrs[0];
    }

    #region SingletonMonoBehaviour 
    protected override void OnNewInstanceCreated()
    {
        ThrowExceptionOnNewInstance();
    }
    #endregion
}
