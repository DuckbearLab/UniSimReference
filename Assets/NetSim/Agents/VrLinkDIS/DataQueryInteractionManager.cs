using EventReports;
using NetStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataQueryInteraction
{
    public IntPtr IntPtr;

    private bool isWriting;

    public DataQueryInteraction()
    {
        isWriting = true;
        IntPtr = NetSimAgent.Instance.CreateDataQueryInteraction();
    }

    public DataQueryInteraction(IntPtr intPtr)
    {
        isWriting = false;
        IntPtr = intPtr;
        SenderId = NetSimAgent.Instance.DQISenderId(intPtr);
        ReceiverId = NetSimAgent.Instance.DQIReceiverId(intPtr);
        RequestId = NetSimAgent.Instance.DQIRequestId(intPtr);
    }

    ~DataQueryInteraction()
    {
        if (isWriting)
            NetSimAgent.Instance.DeleteDataQueryInteraction(IntPtr);
    }

    public EntityId SenderId
    {
        get
        {
            return NetSimAgent.Instance.DQISenderId(IntPtr);
        }
        set
        {
            NetSimAgent.Instance.SetDQISenderId(IntPtr, value);
        }
    }

    public EntityId ReceiverId
    {
        get
        {
            return NetSimAgent.Instance.DQIReceiverId(IntPtr);
        }
        set
        {
            NetSimAgent.Instance.SetDQIReceiverId(IntPtr, value);
        }
    }

    public ulong RequestId
    {
        get
        {
            return NetSimAgent.Instance.DQIRequestId(IntPtr);
        }
        set
        {
            NetSimAgent.Instance.SetDQIRequestId(IntPtr, value);
        }
    }

    public void initDatumIds(ulong numFixedFields, ulong numVarFields)
    {
        NetSimAgent.Instance.DQIInitDatumIds(IntPtr,numFixedFields,numVarFields);
    }

    public int NumFixedFields
    {
        get
        {
            return NetSimAgent.Instance.DataQueryInteractionNumFixedFields(IntPtr);
        }
    }

    public int FixedDatumId(int index)
    {
        return NetSimAgent.Instance.DataQueryInteractionFixedDatumId(IntPtr, index);
    }

    public void SetFixedDatumId(int index, int id)
    {
        NetSimAgent.Instance.DataQueryInteractionSetFixedDatumId(IntPtr, index, id);
    }

    public int NumVarFields
    {
        get
        {
            return NetSimAgent.Instance.DataQueryInteractionNumVarFields(IntPtr);
        }
    }

    public int VarDatumId(int index)
    {
        return NetSimAgent.Instance.DataQueryInteractionVarDatumId(IntPtr, index);
    }

    public void SetVarDatumId(int index, int id)
    {
        NetSimAgent.Instance.DataQueryInteractionSetVarDatumId(IntPtr, index, id);
    }
}

public class DataQueryInteractionManager : MonoBehaviour
{
    public ExerciseConnection ExerciseConnection;

    private List<Action<DataQueryInteraction>> DataQueryInteractionSubscribers;

    // Storing this to prevent garbage collection
    private NetSimAgent.DQIReceivedCallback dataQueryInteractionReceivedCallback;

    static DataQueryInteractionManager()
    {
    }

    void Awake()
    {
        DataQueryInteractionSubscribers = new List<Action<DataQueryInteraction>>();
    }

    void Start()
    {
        dataQueryInteractionReceivedCallback = new NetSimAgent.DQIReceivedCallback(DataQueryInteractionReceived);
        NetSimAgent.Instance.SetDQIReceivedCallback(ExerciseConnection.ExerciseConnectionPtr, dataQueryInteractionReceivedCallback);
    }

    public void SubscribeDataQueryInteraction(Action<DataQueryInteraction> handler)
    {
        DataQueryInteractionSubscribers.Add(handler);
    }

    public void UnSubscribeDataQueryInteraction(Action<DataQueryInteraction> handler)
    {
        DataQueryInteractionSubscribers.Remove(handler);
    }

    private void DataQueryInteractionReceived(IntPtr DataQueryInteractionIntPtr)
    {
        DataQueryInteraction DataQueryInteraction = new DataQueryInteraction(DataQueryInteractionIntPtr);
        foreach (var subscriber in DataQueryInteractionSubscribers)
        {
            //DataQueryInteraction.IntPtr = DataQueryInteractionIntPtr;
            //DataQueryInteraction.SenderId = NetSimAgent.Instance.DISenderId(DataQueryInteractionIntPtr);
            //DataQueryInteraction.ReceiverId = NetSimAgent.Instance.DIReceiverId(DataQueryInteractionIntPtr);
            //DataQueryInteraction.RequestId = NetSimAgent.Instance.DIRequestId(DataQueryInteractionIntPtr);
            subscriber(DataQueryInteraction);
        }
    }

    public void Send(DataQueryInteraction DataQueryInteraction)
    {
        //NetSimAgent.Instance.SetDQISenderId(DataQueryInteraction.IntPtr, DataQueryInteraction.SenderId);
        //NetSimAgent.Instance.SetDQIReceiverId(DataQueryInteraction.IntPtr, DataQueryInteraction.ReceiverId);
        //NetSimAgent.Instance.SetDQIRequestId(DataQueryInteraction.IntPtr, DataQueryInteraction.RequestId);
        NetSimAgent.Instance.SendDQI(ExerciseConnection.ExerciseConnectionPtr, DataQueryInteraction.IntPtr);
    }
}
