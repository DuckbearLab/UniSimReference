using CppStructs;
using NetStructs;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/* ======================================================================================
 * ExerciseConnection : a singleton which handles various events in the network.
 * ...needs to be filled by someone else as I don't fully understand the script.
====================================================================================== */
public class ExerciseConnection : SingletonMonoBehaviour<ExerciseConnection>
{
    public int port;
    public int exerciseId;
    public int siteId;
    public int applicationNumber;
    //Filtered Entities List By The Vrlink DLL
    public TextAsset FilteredEntitiesConfigFile;
    public List<PublishedEntity> LocalPublishedEntities
    {
        get
        {
            return new List<PublishedEntity>(localPublishedEntities);
        }
    }
    private List<PublishedEntity> localPublishedEntities = new List<PublishedEntity>();

    public IntPtr ExerciseConnectionPtr { get; private set; }

    private List<Action<FireInteraction>> FireInteractionSubscribers;
    private List<Action<DetonationInteraction>> DetonationInteractionSubscribers;

    private List<Action<CreateEntityInteraction>> CreateEntityInteractionSubscribers;
    private List<Action<AcknowledgeInteraction>> AcknowledgeInteractionSubscribers;
    private List<Action<RemoveEntityInteraction>> RemoveEntityInteractionSubscribers;

    // Storing this to prevent garbage collection
    private NetSimAgent.FireCallback fireCallback;

    // Storing this to prevent garbage collection
    private NetSimAgent.DetonationCallback detonationCallback;

    // Storing this to prevent garbage collection
    private NetSimAgent.CreateEntityCallback createEntityCallback;

    // Storing this to prevent garbage collection
    private NetSimAgent.AcknowledgeCallback acknowledgeCallback;

    // Storing this to prevent garbage collection
    private NetSimAgent.RemoveEntityCallback removeEntityCallback;

    protected override void Awake()
    {
        base.Awake();
        //Convert the filtered entities list to a string with , between lines from the csv and send it to the vrlink dll
        List<string> filteredEntities = ReadFilteredEntities();
        string filteredEntitiesListString;
        if (filteredEntities == null)
            filteredEntitiesListString = "";
        else
            filteredEntitiesListString = String.Join(",", filteredEntities.ToArray());
        ExerciseConnectionPtr = NetSimAgent.Instance.CreateExerciseConnection(port, exerciseId, siteId, applicationNumber, filteredEntitiesListString);

        fireCallback = new NetSimAgent.FireCallback(fireInteraction);
        NetSimAgent.Instance.SetFireCallback(ExerciseConnectionPtr, fireCallback);

        detonationCallback = new NetSimAgent.DetonationCallback(detonationInteraction);
        NetSimAgent.Instance.SetDetonationCallback(ExerciseConnectionPtr, detonationCallback);

        createEntityCallback = new NetSimAgent.CreateEntityCallback(createEntityInteraction);
        NetSimAgent.Instance.SetCreateEntityCallback(ExerciseConnectionPtr, createEntityCallback);

        acknowledgeCallback = new NetSimAgent.AcknowledgeCallback(acknowledgeInteraction);
        NetSimAgent.Instance.SetAcknowledgeCallback(ExerciseConnectionPtr, acknowledgeCallback);

        removeEntityCallback = new NetSimAgent.RemoveEntityCallback(removeEntityInteraction);
        NetSimAgent.Instance.SetRemoveEntityCallback(ExerciseConnectionPtr, removeEntityCallback);

        FireInteractionSubscribers = new List<Action<FireInteraction>>();
        DetonationInteractionSubscribers = new List<Action<DetonationInteraction>>();

        CreateEntityInteractionSubscribers = new List<Action<CreateEntityInteraction>>();
        AcknowledgeInteractionSubscribers = new List<Action<AcknowledgeInteraction>>();
        RemoveEntityInteractionSubscribers = new List<Action<RemoveEntityInteraction>>();
    }

    private bool shouldUpdateRemoteEntities = true;

    void Update()
    {
        localPublishedEntities.RemoveAll((x) => { return null == x; });
        DrainInput();
        shouldUpdateRemoteEntities = true;
    }

    void FixedUpdate()
    {
        if (shouldUpdateRemoteEntities)
        {
            NetSimAgent.Instance.UpdateRemoteEntities(ExerciseConnectionPtr);
            shouldUpdateRemoteEntities = false;
        }
    }

    public void DrainInput()
    {
        NetSimAgent.Instance.DrainInput(ExerciseConnectionPtr);
    }

    #region Fire
    private void fireInteraction(FireInteraction fireInteraction)
    {
        //FireInteractionSubscribers
        foreach (var subscriber in FireInteractionSubscribers)
            subscriber(fireInteraction);
    }

    public void SubscribeFireInteraction(Action<FireInteraction> handler)
    {
        FireInteractionSubscribers.Add(handler);
    }

    public void UnSubscribeFireInteraction(Action<FireInteraction> handler)
    {
        FireInteractionSubscribers.Remove(handler);
    }
    #endregion

    #region Detonation
    private void detonationInteraction(DetonationInteraction detInteraction)
    {
        //FireInteractionSubscribers
        foreach (var subscriber in DetonationInteractionSubscribers)
            subscriber(detInteraction);
    }

    public void SubscribeDetonationInteraction(Action<DetonationInteraction> handler)
    {
        DetonationInteractionSubscribers.Add(handler);
    }

    public void UnSubscribeDetonationInteraction(Action<DetonationInteraction> handler)
    {
        DetonationInteractionSubscribers.Remove(handler);
    }
    #endregion

    #region CreateEntity
    private void createEntityInteraction(CreateEntityInteraction createEntityInteraction)
    {
        //CreateEntityInteractionSubscribers
        foreach (var subscriber in CreateEntityInteractionSubscribers)
            subscriber(createEntityInteraction);
    }

    public void SubscribeCreateEntityInteraction(Action<CreateEntityInteraction> handler)
    {
        CreateEntityInteractionSubscribers.Add(handler);
    }

    public void UnSubscribeCreateEntityInteraction(Action<CreateEntityInteraction> handler)
    {
        CreateEntityInteractionSubscribers.Remove(handler);
    }
    #endregion

    #region Acknowledge
    private void acknowledgeInteraction(AcknowledgeInteraction acknowledgeInteraction)
    {
        //CreateEntityInteractionSubscribers
        foreach (var subscriber in AcknowledgeInteractionSubscribers)
            subscriber(acknowledgeInteraction);
    }

    public void SubscribeAcknowledgeInteraction(Action<AcknowledgeInteraction> handler)
    {
        AcknowledgeInteractionSubscribers.Add(handler);
    }

    public void UnSubscribeAcknowledgeInteraction(Action<AcknowledgeInteraction> handler)
    {
        AcknowledgeInteractionSubscribers.Remove(handler);
    }
    #endregion

    #region RemoveEntity
    private void removeEntityInteraction(RemoveEntityInteraction removeEntityInteraction)
    {
        //RemoveEntityInteractionSubscribers
        foreach (var subscriber in RemoveEntityInteractionSubscribers)
            subscriber(removeEntityInteraction);
    }

    public void SubscribeRemoveEntityInteraction(Action<RemoveEntityInteraction> handler)
    {
        RemoveEntityInteractionSubscribers.Add(handler);
    }

    public void UnSubscribeRemoveEntityInteraction(Action<RemoveEntityInteraction> handler)
    {
        RemoveEntityInteractionSubscribers.Remove(handler);
    }
    #endregion

    public EntityPublisher CreateEntityPublisher(PublishedEntity Entity, string playerEntityType,
        bool publishLocation, bool publishHeading, bool publishPitch, bool publishRoll, bool publishVelocity)
    {
        localPublishedEntities.Add(Entity);
        return new EntityPublisher(
            ExerciseConnectionPtr, NetSimAgent.Instance.CreateEntityPublisher(ExerciseConnectionPtr, playerEntityType),
            publishLocation, publishHeading, publishPitch, publishRoll, publishVelocity
            );
    }

    /// <summary>
    /// This is called on PublishedEntity.OnDestroy(), to signal that this entity has been destroyed and to remove it from the list.
    /// </summary>
    /// <param name="RemovedEntity"></param>
    public void RemovePublishedEntity(PublishedEntity RemovedEntity)
    {
        localPublishedEntities.Remove(RemovedEntity);
    }

    public void SendImageShare(string img, string senderID, string recieverID, int frequency, int requestCounter)
    {
        NetSimAgent.Instance.SendImgShare(ExerciseConnectionPtr, img, senderID, recieverID, frequency, requestCounter);
    }

    public void SendCreateEntitySetData(EntityId senderId, EntityId recieverId, int requestId, EntityType entityType, XYZ location, ForceType ForceType, double psi, double theta, double phi)
    {
        NetSimAgent.Instance.SendCreateEntitySetData(ExerciseConnectionPtr, senderId, recieverId, requestId, entityType, location, ForceType, psi, theta, phi);
    }

    public EventID SendFireInteraction(FireInteraction fireInteraction)
    {
        return NetSimAgent.Instance.SendFireInteraction(ExerciseConnectionPtr, fireInteraction);
    }

    public void SendDetonationInteraction(DetonationInteraction detInteraction)
    {
        NetSimAgent.Instance.SendDetonationInteraction(ExerciseConnectionPtr, detInteraction);
    }

    public void SendCreateEntityInteraction(CreateEntityInteraction createEntityInteraction)
    {
        NetSimAgent.Instance.SendCreateEntityInteraction(ExerciseConnectionPtr, createEntityInteraction);
    }

    public void SendAcknowledgeInteraction(AcknowledgeInteraction acknowledgeInteraction)
    {
        NetSimAgent.Instance.SendAcknowledgeInteraction(ExerciseConnectionPtr, acknowledgeInteraction);
    }

    public void SetRefLatLon(double refLat, double refLon)
    {
        NetSimAgent.Instance.SetRefLatLon(refLat, refLon);
    }

    public void SendRemoveEntityInteraction(RemoveEntityInteraction removeEntityInteraction)
    {
        NetSimAgent.Instance.SendRemoveEntityInteraction(ExerciseConnectionPtr, removeEntityInteraction);
    }

    public void SendComment(EntityId senderId, EntityId recieverId, string comment)
    {
        NetSimAgent.Instance.SendComment(ExerciseConnectionPtr, senderId, recieverId, comment);
    }

    #region FilteredVrlinkEntities
    //Filtered Entities List By The Vrlink DLL
    private List<string> ReadFilteredEntities()
    {

        if (FilteredEntitiesConfigFile == null)
            return null;

        List<string> filteredEntities = new List<string>();
        StringReader reader = new StringReader(FilteredEntitiesConfigFile.text);
        string line;
        if (reader != null)
            while ((line = reader.ReadLine()) != null)
            {
                filteredEntities.Add(line);
            }

        return filteredEntities;
    }

    #endregion
}