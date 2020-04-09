using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetStructs;
using CppStructs;

public class SimManagerRequests : MonoBehaviour
{
    public ExerciseConnection ExerciseConnection;
    public PublishedEntity InfantryPublishedEntity;
    public float TimeBetweenSendingActiveRequests;
    public float RetryRequestRate;

    public event System.Action OnSendCreateEntityMsg;
    public event System.Action<EntityType> OnCreateEntity;
    public event System.Action OnSendRemoveEntityMsg;
    public event System.Action<EntityId> OnRemoveEntity;

    private struct CreateEntityInfo
    {
        public Vector3 Location;
        public Vector3 Orientation;
        public EntityType EntityType;
        public ForceType ForceType;
    }

    private Dictionary<int, CreateEntityInfo> CreateEntityRequests;
    private Dictionary<int, EntityId> RemoveEntityRequests;

    void Awake()
    {
        CreateEntityRequests = new Dictionary<int, CreateEntityInfo>();
        RemoveEntityRequests = new Dictionary<int, EntityId>();

        //StartCoroutine(SendAllActiveRemoveRequests());
    }

    void Start()
    {
        ExerciseConnection.SubscribeAcknowledgeInteraction(AcknowledgeCallback);
    }

    public int SendCreateEntityMsg(Vector3 location, Vector3 orientation, EntityType entityType, ForceType forceType = ForceType.DtForceOther)
    {
        int requestId = Random.Range(0, 1000000000);
        while (CreateEntityRequests.ContainsKey(requestId))
            requestId = Random.Range(0, 1000000000);

        CreateEntityInfo info = new CreateEntityInfo()
        {
            Location = location,
            Orientation = orientation,
            EntityType = entityType,
            ForceType = forceType
        };

        CreateEntityRequests[requestId] = info;

        CreateEntityInteraction createEntityInteraction;
        createEntityInteraction.senderId = InfantryPublishedEntity.MyEntityId;
        createEntityInteraction.requestId = requestId;
        createEntityInteraction.receiverId = EntityId.NullId;

        ExerciseConnection.SendCreateEntityInteraction(createEntityInteraction);

        if (OnSendCreateEntityMsg != null)
            OnSendCreateEntityMsg();

        return requestId;
    }

    public int SendRemoveEntityMsg(EntityId entityId)
    {
        int requestId = Random.Range(0, 1000000000);
        while (RemoveEntityRequests.ContainsKey(requestId))
            requestId = Random.Range(0, 1000000000);

        RemoveEntityRequests[requestId] = entityId;

        RemoveEntityInteraction removeEntityInteraction;
        removeEntityInteraction.senderId = InfantryPublishedEntity.MyEntityId;
        removeEntityInteraction.requestId = requestId;
        removeEntityInteraction.receiverId = entityId;

        ExerciseConnection.SendRemoveEntityInteraction(removeEntityInteraction);

        if (OnSendRemoveEntityMsg != null)
            OnSendRemoveEntityMsg();

        return requestId;
    }

    private void AcknowledgeCallback(AcknowledgeInteraction acInteraction)
    {
        if (null == InfantryPublishedEntity || acInteraction.receiverId != InfantryPublishedEntity.MyEntityId)
            return;

        if (acInteraction.acknowledgeFlag == NetStructs.AcknowledgeFlag.DtAckCreate)
        {
            if (!CreateEntityRequests.ContainsKey(acInteraction.requestId))
                return;

            CreateEntityInfo info = CreateEntityRequests[acInteraction.requestId];

            Vector3 newEntityLocation = info.Location;
            Vector3 orientation = info.Orientation;
            EntityType entityType = info.EntityType;
            ForceType ForceType;

            if (info.ForceType == ForceType.DtForceOther)
                ForceType = InfantryPublishedEntity.ForceType;
            else
                ForceType = info.ForceType;

            EntityId senderId = InfantryPublishedEntity.MyEntityId;
            EntityId recieverId = acInteraction.senderId;
            int requestId = acInteraction.requestId;
            XYZ location;
            location.X = newEntityLocation.x;
            location.Y = newEntityLocation.y;
            location.Z = newEntityLocation.z;

            // x and y values of the orientation are switched in order to preserve the original rotation, don't know the reason why
            ExerciseConnection.SendCreateEntitySetData(senderId, recieverId, requestId, entityType, location, ForceType, orientation.y, orientation.x, orientation.z);

            CreateEntityRequests.Remove(requestId);

            if (OnCreateEntity != null)
                OnCreateEntity(entityType);
        }
        else if (acInteraction.acknowledgeFlag == NetStructs.AcknowledgeFlag.DtAckRemove)
        {
            if (!RemoveEntityRequests.ContainsKey(acInteraction.requestId))
                return;

            if (OnRemoveEntity != null)
                OnRemoveEntity(RemoveEntityRequests[acInteraction.requestId]);

            RemoveEntityRequests.Remove(acInteraction.requestId);
        }
    }

    public void SendCreateEntityMsgPersistent(Vector3 location, Vector3 orientation, EntityType entityType, ForceType forceType = ForceType.DtForceOther)
    {
        int requestId = SendCreateEntityMsg(location, orientation, entityType, forceType);
        StartCoroutine(SendCreatePersistent(requestId));
    }

    private IEnumerator SendCreatePersistent(int requestId)
    {
        yield return new WaitForSeconds(RetryRequestRate);

        while (CreateEntityRequests.ContainsKey(requestId))
        {
            int id = requestId;
            CreateEntityInfo info = CreateEntityRequests[requestId];
            requestId = SendCreateEntityMsg(info.Location, info.Orientation, info.EntityType, info.ForceType);

            CreateEntityRequests.Remove(id);
            yield return new WaitForSeconds(RetryRequestRate);
        }
    }

    public void SendRemoveEntityMsgPersistent(EntityId entityId)
    {
        int requestId = SendRemoveEntityMsg(entityId);
        StartCoroutine(SendRemovePersistent(requestId));
    }

    private IEnumerator SendRemovePersistent(int requestId)
    {
        yield return new WaitForSeconds(RetryRequestRate);

        while (RemoveEntityRequests.ContainsKey(requestId))
        {
            int id = requestId;
            requestId = SendRemoveEntityMsg(RemoveEntityRequests[requestId]);

            RemoveEntityRequests.Remove(id);
            yield return new WaitForSeconds(RetryRequestRate);
        }
    }

    private IEnumerator SendAllActiveRemoveRequests()
    {
        foreach (int key in RemoveEntityRequests.Keys)
        {
            RemoveEntityInteraction removeEntityInteraction;
            removeEntityInteraction.senderId = InfantryPublishedEntity.MyEntityId;
            removeEntityInteraction.requestId = key;
            removeEntityInteraction.receiverId = RemoveEntityRequests[key];

            ExerciseConnection.SendRemoveEntityInteraction(removeEntityInteraction);
        }

        yield return new WaitForSeconds(TimeBetweenSendingActiveRequests);
        StartCoroutine(SendAllActiveRemoveRequests());
    }
}
