using EventReports;
using NetStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDataInteraction
{
    public IntPtr IntPtr;

    private bool isWriting;

    public SetDataInteraction()
    {
        isWriting = true;
        IntPtr = NetSimAgent.Instance.CreateSetDataInteraction();
    }

    public SetDataInteraction(IntPtr intPtr)
    {
        isWriting = false;
        IntPtr = intPtr;
        SenderId = NetSimAgent.Instance.SDISenderId(intPtr);
        ReceiverId = NetSimAgent.Instance.SDIReceiverId(intPtr);
        RequestId = NetSimAgent.Instance.SDIRequestId(intPtr);
    }

    ~SetDataInteraction()
    {
        if (isWriting)
            NetSimAgent.Instance.DeleteSetDataInteraction(IntPtr);
    }

    public EntityId SenderId
    {
        get
        {
            return NetSimAgent.Instance.SDISenderId(IntPtr);
        }
        set
        {
            NetSimAgent.Instance.SetSDISenderId(IntPtr, value);
        }
    }

    public EntityId ReceiverId
    {
        get
        {
            return NetSimAgent.Instance.SDIReceiverId(IntPtr);
        }
        set
        {
            NetSimAgent.Instance.SetSDIReceiverId(IntPtr, value);
        }
    }

    public ulong RequestId
    {
        get
        {
            return NetSimAgent.Instance.SDIRequestId(IntPtr);
        }
        set
        {
            NetSimAgent.Instance.SetSDIRequestId(IntPtr, value);
        }
    }

    public void AddSetDataInteractionFixedInt(int data, uint datumParam)
    {
        NetSimAgent.Instance.AddSetDataInteractionFixedInt(IntPtr, data, datumParam);
    }

    public int ReadSetDataInteractionFixedInt(int index)
    {
        return NetSimAgent.Instance.ReadSetDataInteractionFixedInt(IntPtr, index);
    }

    public void AddSetDataInteractionFixedUInt(uint data, uint datumParam)
    {
        NetSimAgent.Instance.AddSetDataInteractionFixedUInt(IntPtr, data, datumParam);
    }

    public uint ReadSetDataInteractionFixedUInt(int index)
    {
        return NetSimAgent.Instance.ReadSetDataInteractionFixedUInt(IntPtr, index);
    }

    public void AddSetDataInteractionFixedFloat(float data, uint datumParam)
    {
        NetSimAgent.Instance.AddSetDataInteractionFixedFloat(IntPtr, data, datumParam);
    }

    public float ReadSetDataInteractionFixedFloat(int index)
    {
        return NetSimAgent.Instance.ReadSetDataInteractionFixedFloat(IntPtr, index);
    }

    public void AddSetDataInteractionVarString(string data, int dataLength, uint datumParam)
    {
        NetSimAgent.Instance.AddSetDataInteractionVarString(IntPtr, data, dataLength, datumParam);
    }

    public IntPtr ReadSetDataInteractionVarString(int index)
    {
        return NetSimAgent.Instance.ReadSetDataInteractionVarString(IntPtr, index);
    }

    public int GetFixedDatumId(int index)
    {
        return NetSimAgent.Instance.SetDataInteractionFixedDatumId(IntPtr, index);
    }

    public int GetVarDatumId(int index)
    {
        return NetSimAgent.Instance.SetDataInteractionVarDatumId(IntPtr, index);
    }

    public int NumFixedFields
    {
        get
        {
            return NetSimAgent.Instance.SetDataInteractionNumFixedFields(IntPtr);
        }
    }

    public int NumVarFields
    {
        get
        {
            return NetSimAgent.Instance.SetDataInteractionNumVarFields(IntPtr);
        }
    }
}

public class SetDataInteractionManager : MonoBehaviour
{
    public ExerciseConnection ExerciseConnection;

    private List<Action<SetDataInteraction>> SetDataInteractionSubscribers;

    // Storing this to prevent garbage collection
    private NetSimAgent.SDIReceivedCallback sDIReceivedCallback;

    static SetDataInteractionManager()
    {
    }

    void Awake()
    {
        SetDataInteractionSubscribers = new List<Action<SetDataInteraction>>();
    }

    void Start()
    {
        sDIReceivedCallback = new NetSimAgent.SDIReceivedCallback(DataInteractionReceived);
        NetSimAgent.Instance.SetSDIReceivedCallback(ExerciseConnection.ExerciseConnectionPtr, sDIReceivedCallback);
    }

    public void SubscribeDataInteraction(Action<SetDataInteraction> handler)
    {
        SetDataInteractionSubscribers.Add(handler);
    }

    public void UnSubscribeDataInteraction(Action<SetDataInteraction> handler)
    {
        SetDataInteractionSubscribers.Remove(handler);
    }

    private void DataInteractionReceived(IntPtr setDataInteractionIntPtr)
    {
        SetDataInteraction setDataInteraction = new SetDataInteraction(setDataInteractionIntPtr);
        foreach (var subscriber in SetDataInteractionSubscribers)
        {
            //setDataInteraction.IntPtr = setDataInteractionIntPtr;
            //setDataInteraction.SenderId = NetSimAgent.Instance.SDISenderId(setDataInteractionIntPtr);
            //setDataInteraction.ReceiverId = NetSimAgent.Instance.SDIReceiverId(setDataInteractionIntPtr);
            //setDataInteraction.RequestId = NetSimAgent.Instance.SDIRequestId(setDataInteractionIntPtr);
            subscriber(setDataInteraction);
        }
    }

    public void Send(SetDataInteraction setDataInteraction)
    {
        //NetSimAgent.Instance.SetSDISenderId(setDataInteraction.IntPtr, setDataInteraction.SenderId);
        //NetSimAgent.Instance.SetSDIReceiverId(setDataInteraction.IntPtr, setDataInteraction.ReceiverId);
        //NetSimAgent.Instance.SetSDIRequestId(setDataInteraction.IntPtr, setDataInteraction.RequestId);
        NetSimAgent.Instance.SendSDI(ExerciseConnection.ExerciseConnectionPtr, setDataInteraction.IntPtr);
    }
}
