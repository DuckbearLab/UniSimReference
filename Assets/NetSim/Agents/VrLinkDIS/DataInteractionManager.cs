using EventReports;
using NetStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInteraction
{
    public IntPtr IntPtr;

    private bool isWriting;

    public DataInteraction()
    {
        isWriting = true;
        IntPtr = NetSimAgent.Instance.CreateDataInteraction();
    }

    public DataInteraction(IntPtr intPtr)
    {
        isWriting = false;
        IntPtr = intPtr;
        SenderId = NetSimAgent.Instance.DISenderId(intPtr);
        ReceiverId = NetSimAgent.Instance.DIReceiverId(intPtr);
        RequestId = NetSimAgent.Instance.DIRequestId(intPtr);
    }

    ~DataInteraction()
    {
        if (isWriting)
            NetSimAgent.Instance.DeleteDataInteraction(IntPtr);
    }

    public EntityId SenderId
    {
        get
        {
            return NetSimAgent.Instance.DISenderId(IntPtr);
        }
        set
        {
            NetSimAgent.Instance.SetDISenderId(IntPtr, value);
        }
    }

    public EntityId ReceiverId
    {
        get
        {
            return NetSimAgent.Instance.DIReceiverId(IntPtr);
        }
        set
        {
            NetSimAgent.Instance.SetDIReceiverId(IntPtr, value);
        }
    }

    public ulong RequestId
    {
        get
        {
            return NetSimAgent.Instance.DIRequestId(IntPtr);
        }
        set
        {
            NetSimAgent.Instance.SetDIRequestId(IntPtr, value);
        }
    }

    public void AddDataInteractionFixedInt(int data, uint datumParam)
    {
        NetSimAgent.Instance.AddDataInteractionFixedInt(IntPtr, data, datumParam);
    }

    public int ReadDataInteractionFixedInt(int index)
    {
        return NetSimAgent.Instance.ReadDataInteractionFixedInt(IntPtr, index);
    }

    public void AddDataInteractionFixedUInt(uint data, uint datumParam)
    {
        NetSimAgent.Instance.AddDataInteractionFixedUInt(IntPtr, data, datumParam);
    }

    public uint ReadDataInteractionFixedUInt(int index)
    {
        return NetSimAgent.Instance.ReadDataInteractionFixedUInt(IntPtr, index);
    }

    public void AddDataInteractionFixedFloat(float data, uint datumParam)
    {
        NetSimAgent.Instance.AddDataInteractionFixedFloat(IntPtr, data, datumParam);
    }

    public float ReadDataInteractionFixedFloat(int index)
    {
        return NetSimAgent.Instance.ReadDataInteractionFixedFloat(IntPtr, index);
    }

    public void AddDataInteractionVarString(string data, int dataLength, uint datumParam)
    {
        NetSimAgent.Instance.AddDataInteractionVarString(IntPtr, data, dataLength, datumParam);
    }

    public IntPtr ReadDataInteractionVarString(int index)
    {
        return NetSimAgent.Instance.ReadDataInteractionVarString(IntPtr, index);
    }

    public int GetFixedDatumId(int index)
    {
        return NetSimAgent.Instance.DataInteractionFixedDatumId(IntPtr, index);
    }

    public int GetVarDatumId(int index)
    {
        return NetSimAgent.Instance.DataInteractionVarDatumId(IntPtr, index);
    }

    public int NumFixedFields
    {
        get
        {
            return NetSimAgent.Instance.DataInteractionNumFixedFields(IntPtr);
        }
    }

    public int NumVarFields
    {
        get
        {
            return NetSimAgent.Instance.DataInteractionNumVarFields(IntPtr);
        }
    }
}

public class DataInteractionManager : MonoBehaviour
{
    public ExerciseConnection ExerciseConnection;

    private List<Action<DataInteraction>> DataInteractionSubscribers;

    // Storing this to prevent garbage collection
    private NetSimAgent.DIReceivedCallback dataInteractionReceivedCallback;

    static DataInteractionManager()
    {
    }

    void Awake()
    {
        DataInteractionSubscribers = new List<Action<DataInteraction>>();
    }

    void Start()
    {
        dataInteractionReceivedCallback = new NetSimAgent.DIReceivedCallback(DataInteractionReceived);
        NetSimAgent.Instance.SetDIReceivedCallback(ExerciseConnection.ExerciseConnectionPtr, dataInteractionReceivedCallback);
    }

    public void SubscribeDataInteraction(Action<DataInteraction> handler)
    {
        DataInteractionSubscribers.Add(handler);
    }

    public void UnSubscribeDataInteraction(Action<DataInteraction> handler)
    {
        DataInteractionSubscribers.Remove(handler);
    }

    private void DataInteractionReceived(IntPtr dataInteractionIntPtr)
    {
        DataInteraction dataInteraction = new DataInteraction(dataInteractionIntPtr);
        foreach (var subscriber in DataInteractionSubscribers)
        {
            //dataInteraction.IntPtr = dataInteractionIntPtr;
            //dataInteraction.SenderId = NetSimAgent.Instance.DISenderId(dataInteractionIntPtr);
            //dataInteraction.ReceiverId = NetSimAgent.Instance.DIReceiverId(dataInteractionIntPtr);
            //dataInteraction.RequestId = NetSimAgent.Instance.DIRequestId(dataInteractionIntPtr);
            subscriber(dataInteraction);
        }
    }

    public void Send(DataInteraction dataInteraction)
    {
        //NetSimAgent.Instance.SetDISenderId(dataInteraction.IntPtr, dataInteraction.SenderId);
        //NetSimAgent.Instance.SetDIReceiverId(dataInteraction.IntPtr, dataInteraction.ReceiverId);
        //NetSimAgent.Instance.SetDIRequestId(dataInteraction.IntPtr, dataInteraction.RequestId);
        NetSimAgent.Instance.SendDI(ExerciseConnection.ExerciseConnectionPtr, dataInteraction.IntPtr);
    }
}
