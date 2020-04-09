using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CppStructs;
using NetStructs;
using System.Runtime.InteropServices;
using System;
using System.Linq;

public class ReflectedEntities : SingletonMonoBehaviour<ReflectedEntities>, IEnumerable
{

    [System.Serializable]
    public struct EntityTypeReflectedModelPair
    {
        public string Name;
        public string EntityType;
        public GameObject ReflectedPrefab;
    }

    public event Action<ReflectedEntity> ReflectedEntityJoined;
    public event Action<ReflectedEntity> ReflectedEntityAboutToLeave;

    public ExerciseConnection ExerciseConnection;
    public PublishedEntity PublishedEntity;
    public EventReportsManager EventReportsManager;
    public EntityTypeReflectedModelPair[] ReflectedModels;

    private HashSet<EntityId> entitiesIdsToIgnore;
    private HashSet<EntityId> checkedEntityIds;

    private Dictionary<EntityId, ReflectedEntityCache> reflectedEntitiesCache;
    private class ReflectedEntityCache
    {
        public ReflectedEntity ReflectedEntity;
        public Rigidbody Rigidbody;
        public Transform Transform;
        public GameObject GameObject;
        public Infantry.ReflectedInfantryModelOptions SelectedDevice; //only for reflected infantries. 
        public Animator Animator;
        public AnimatorControllerParameter[] AnimatorParameters;
        public int AnimatorSpeedParameterHash = -1;
        public Dictionary<uint, ReflectedEntityArtPart> ArtParts;
        public LifeformState PrevLifeformState = (LifeformState)(-1);
        public DamageState PrevDamageStateName = (DamageState)(-1);
    }

    protected override void Awake()
    {
        base.Awake();
        entitiesIdsToIgnore = new HashSet<EntityId>();
        checkedEntityIds = new HashSet<EntityId>();
        reflectedEntitiesCache = new Dictionary<EntityId, ReflectedEntityCache>();
        if (null == EventReportsManager)
            EventReportsManager = GetComponent<EventReportsManager>();
        EventReportsManager.Subscribe<EventReports.InfantryModelOptions>(ApplyDeviceToInfantry);
    }

    void Start()
    {
        var exerciseConnectionPtr = ExerciseConnection.ExerciseConnectionPtr;

        entityAddedCallback = new NetSimAgent.EntityAddedCallback(EntityAdded);
        NetSimAgent.Instance.SetEntityAddedCallback(exerciseConnectionPtr, entityAddedCallback);

        entityRemovedCallback = new NetSimAgent.EntityRemovedCallback(EntityRemoved);
        NetSimAgent.Instance.SetEntityRemovedCallback(exerciseConnectionPtr, entityRemovedCallback);

        entityStateCallback = new NetSimAgent.EntityStateCallback(EntityStateChanged);
        NetSimAgent.Instance.SetEntityStateCallback(exerciseConnectionPtr, entityStateCallback);

        entityStateArtPartCallback = new NetSimAgent.EntityStateArtPartCallback(EntityStateArtPartChanged);
        NetSimAgent.Instance.SetEntityStateArtPartCallback(exerciseConnectionPtr, entityStateArtPartCallback);
    }

    public void AddToIgnore(EntityId entityId)
    {
        entitiesIdsToIgnore.Add(entityId);
    }

    private void EntityAdded(EntityId entityId)
    {

    }

    float doubleToFloat(double num)
    {
        return Mathf.Round((float)num * 1000f) * 0.001f;
    }

    private void EntityStateChanged(EntityState entityState)
    {
        if (!checkedEntityIds.Contains(entityState.entityId))
        {
            PublishedEntity e = ExerciseConnection.LocalPublishedEntities.Find(x => x.MyEntityId == entityState.entityId);

            //Reflected entities' marking text is capped at 11 characters. This is why StartsWith is used and not ==. 
            //if same entity id but different marking texts, change
            if (null != e && !e.MarkingText.StartsWith(NetSimAgent.Instance.GetReflectedEntityMarkingText(entityState.reflectedEntityPtr)))
            {
                EventReports.EntityIdChange idChange = new EventReports.EntityIdChange();
                idChange.SenderId = e.MyEntityId;
                EntityId id = new EntityId(1, UnityEngine.Random.Range(0, 9999), (ushort)UnityEngine.Random.Range(0, 255));
                idChange.NewEntityId = id;
                EventReportsManager.Send(idChange);
                e.MyEntityId = id;
                e.entityPublisher.SetEntityId(id.ToString());
                if (entitiesIdsToIgnore.Remove(idChange.SenderId))
                    entitiesIdsToIgnore.Add(id);
            }
            else checkedEntityIds.Add(entityState.entityId);
        }

        if (entitiesIdsToIgnore.Contains(entityState.entityId))
            return;

        ReflectedEntityCache reflectedEntityCache;

        bool joined = false;

        if (!reflectedEntitiesCache.TryGetValue(entityState.entityId, out reflectedEntityCache))
        {
            reflectedEntityCache = CreateReflectedEntity(entityState);
            joined = true;
        }

        if (reflectedEntityCache == null) return;

        if (reflectedEntityCache.Rigidbody != null)
        {
            reflectedEntityCache.Rigidbody.MovePosition(new Vector3(doubleToFloat(entityState.posX), doubleToFloat(entityState.posY), doubleToFloat(entityState.posZ)));
            reflectedEntityCache.Rigidbody.velocity = new Vector3(doubleToFloat(entityState.velX), doubleToFloat(entityState.velZ), doubleToFloat(entityState.velY));
        }
        else
        {
            reflectedEntityCache.Transform.position = new Vector3(doubleToFloat(entityState.posX), doubleToFloat(entityState.posY), doubleToFloat(entityState.posZ));
        }
        reflectedEntityCache.Transform.eulerAngles = new Vector3(doubleToFloat(-entityState.rotY), doubleToFloat(entityState.rotX), doubleToFloat(-entityState.rotZ));

        // TODO: Use this for Pitch.
        //reflectedEntityCache.Transform.eulerAngles = new Vector3(doubleToFloat(entityState.rotX), doubleToFloat(entityState.rotY), doubleToFloat(entityState.rotZ));

        if (reflectedEntityCache.Animator != null)
        {
            SetAnimatorParams("Posture", entityState.lifeformState, ref reflectedEntityCache.PrevLifeformState, reflectedEntityCache);
            SetAnimatorParams("Damage", entityState.DamageState, ref reflectedEntityCache.PrevDamageStateName, reflectedEntityCache);
            if (reflectedEntityCache.AnimatorSpeedParameterHash != -1)
                reflectedEntityCache.Animator.SetFloat(reflectedEntityCache.AnimatorSpeedParameterHash, new Vector3(doubleToFloat(entityState.velX), doubleToFloat(entityState.velZ), doubleToFloat(entityState.velY)).magnitude);
        }


        reflectedEntityCache.ReflectedEntity.LifeformState = entityState.lifeformState;
        reflectedEntityCache.ReflectedEntity.DamageState = entityState.DamageState;
        reflectedEntityCache.ReflectedEntity.ForceType = entityState.forceType;
        reflectedEntityCache.ReflectedEntity.PrimaryWeaponState = entityState.primaryWeaponState;
        if (null != reflectedEntityCache.SelectedDevice)
            reflectedEntityCache.SelectedDevice.SetCurrentWeaponState(entityState.primaryWeaponState);

        if (joined && ReflectedEntityJoined != null)
            ReflectedEntityJoined(reflectedEntityCache.ReflectedEntity);
    }

    private void SetAnimatorParams<T>(string paramName, T currentState, ref T prevState, ReflectedEntityCache reflectedEntityCache) where T : struct, IComparable
    {
        if (!prevState.Equals(currentState))
        {
            for (int i = 0; i < reflectedEntityCache.AnimatorParameters.Length; i++)
            {
                if (reflectedEntityCache.AnimatorParameters[i].name.StartsWith(paramName))
                {
                    bool isActive = reflectedEntityCache.AnimatorParameters[i].name == paramName + currentState.ToString();
                    reflectedEntityCache.Animator.SetBool(reflectedEntityCache.AnimatorParameters[i].nameHash, isActive);
                }
            }
            prevState = currentState;
        }
    }

    private ReflectedEntityCache CreateReflectedEntity(EntityState entityState)
    {
        string entityType = entityState.entityType.entityKind + ":" + entityState.entityType.domain + ":" + entityState.entityType.country + ":" + entityState.entityType.category + ":" + entityState.entityType.subCategory + ":" + entityState.entityType.specific + ":" + entityState.entityType.extra;

        GameObject reflectedPrefab = null;

        foreach (var reflectedModelPair in ReflectedModels)
        {
            if (reflectedModelPair.EntityType == entityType)
            {
                reflectedPrefab = reflectedModelPair.ReflectedPrefab;
                break;
            }
        }


        if (reflectedPrefab == null)
            return null;

        var reflectedGameObj = Instantiate(reflectedPrefab);
        var reflectedEntity = reflectedGameObj.GetComponent<ReflectedEntity>();
        if (reflectedEntity == null)
            reflectedEntity = reflectedGameObj.AddComponent<ReflectedEntity>();

        reflectedEntity.EntityId = entityState.entityId;
        reflectedEntity.MarkingText = NetSimAgent.Instance.GetReflectedEntityMarkingText(entityState.reflectedEntityPtr);
        reflectedEntity.EntityType = entityState.entityType;

        reflectedGameObj.name = reflectedEntity.MarkingText;

        var reflectedEntityCache = new ReflectedEntityCache();

        reflectedEntityCache.GameObject = reflectedGameObj;
        reflectedEntityCache.ReflectedEntity = reflectedEntity;
        reflectedEntityCache.Rigidbody = reflectedEntity.GetComponent<Rigidbody>();
        reflectedEntityCache.Transform = reflectedEntity.transform;
        reflectedEntityCache.Animator = reflectedEntity.GetComponent<Animator>();
        reflectedEntityCache.SelectedDevice = reflectedEntity.GetComponent<Infantry.ReflectedInfantryModelOptions>();
        if (reflectedEntityCache.Animator)
        {
            reflectedEntityCache.AnimatorParameters = reflectedEntityCache.Animator.parameters;
            foreach (var anim in reflectedEntityCache.AnimatorParameters)
                if (anim.name == "Speed")
                    reflectedEntityCache.AnimatorSpeedParameterHash = anim.nameHash;
        }
        reflectedEntityCache.ArtParts = new Dictionary<uint, ReflectedEntityArtPart>();
        foreach (var artPart in reflectedGameObj.GetComponentsInChildren<ReflectedEntityArtPart>())
            reflectedEntityCache.ArtParts[artPart.PartID] = artPart;

        reflectedEntityCache.ReflectedEntity.LifeformState = entityState.lifeformState;
        reflectedEntityCache.ReflectedEntity.DamageState = entityState.DamageState;

        reflectedEntitiesCache[entityState.entityId] = reflectedEntityCache;

        return reflectedEntityCache;
    }

    private void EntityStateArtPartChanged(EntityStateArtPart entityStateArtPart)
    {
        ReflectedEntityCache reflectedEntityCache;
        if (reflectedEntitiesCache.TryGetValue(entityStateArtPart.entityId, out reflectedEntityCache))
        {
            ReflectedEntityArtPart reflectedEntityArtPart;
            if (reflectedEntityCache.ArtParts.TryGetValue(entityStateArtPart.partId, out reflectedEntityArtPart))
            {
                reflectedEntityArtPart.transform.localPosition = new Vector3(doubleToFloat(entityStateArtPart.posX), doubleToFloat(entityStateArtPart.posY), doubleToFloat(entityStateArtPart.posZ));
                reflectedEntityArtPart.transform.localEulerAngles = new Vector3(doubleToFloat(entityStateArtPart.rotY), doubleToFloat(entityStateArtPart.rotX), doubleToFloat(entityStateArtPart.rotZ));
            }
        }

        /*Debug.Log(entityStateArtPart.partId + " POS: " + entityStateArtPart.posX + " " + entityStateArtPart.posY + " " + entityStateArtPart.posZ);
        Debug.Log(entityStateArtPart.partId + " ROT: " + entityStateArtPart.rotX + " " + entityStateArtPart.rotY + " " + entityStateArtPart.rotZ);*/
    }

    private void EntityRemoved(EntityId entityId)
    {
        if (reflectedEntitiesCache.ContainsKey(entityId))
        {
            if (ReflectedEntityAboutToLeave != null)
                ReflectedEntityAboutToLeave(reflectedEntitiesCache[entityId].ReflectedEntity);

            Destroy(reflectedEntitiesCache[entityId].GameObject);

            reflectedEntitiesCache.Remove(entityId);
        }

    }

    public ReflectedEntity[] ReflectedEntitiesList
    {
        get
        {
            if (null == reflectedEntitiesCache)
                return new ReflectedEntity[0];
            ReflectedEntity[] reflectedEntities = new ReflectedEntity[reflectedEntitiesCache.Count];
            int i = 0;
            foreach (var reflectedEntityCache in reflectedEntitiesCache.Values)
                reflectedEntities[i++] = reflectedEntityCache.ReflectedEntity;
            return reflectedEntities;
        }
    }

    public ReflectedEntity LookUpByMarkingText(string markingText, bool CaseSensitive = true)
    {
        if (markingText.Length > 11)
            markingText = markingText.Substring(0, 11);
        foreach (var reflectedEntityCache in reflectedEntitiesCache.Values)
        {
            if (CaseSensitive)
            {
                if (reflectedEntityCache.ReflectedEntity.MarkingText == markingText)
                    return reflectedEntityCache.ReflectedEntity;
            }
            else
            {
                if (reflectedEntityCache.ReflectedEntity.MarkingText.Equals(markingText, StringComparison.CurrentCultureIgnoreCase))
                    return reflectedEntityCache.ReflectedEntity;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns a reflected entity whose marking text starts with a given string. 
    /// </summary>
    /// <param name="markingText">The string to check. </param>
    /// <param name="CaseSensitive">Whether the check should be case sensitive or not. </param>
    /// <returns>Reflected Entity if one is found, null otherwise. </returns>
    public ReflectedEntity StartsWithMarkingText(string markingText, bool CaseSensitive = true)
    {
        foreach (var reflectedEntityCache in reflectedEntitiesCache.Values)
        {
            if (CaseSensitive)
            {
                if (reflectedEntityCache.ReflectedEntity.MarkingText.StartsWith(markingText))
                    return reflectedEntityCache.ReflectedEntity;
            }
            else
            {
                if (reflectedEntityCache.ReflectedEntity.MarkingText.ToLower().StartsWith(markingText.ToLower()))
                    return reflectedEntityCache.ReflectedEntity;
            }
        }
        return null;
    }

    public ReflectedEntity GetEntity(EntityId entityId)
    {
        if (reflectedEntitiesCache.ContainsKey(entityId))
            return reflectedEntitiesCache[entityId].ReflectedEntity;
        return null;
    }

    private void ApplyDeviceToInfantry(EventReports.InfantryModelOptions message)
    {
        ReflectedEntityCache cache;
        if (reflectedEntitiesCache.TryGetValue(message.SenderId, out cache) && null != cache.SelectedDevice)
        {
            cache.SelectedDevice.SetCurrentDevice(message.DeviceIndex);
            cache.SelectedDevice.SetShowModel(message.ShowModel);
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ReflectedEntitiesList.GetEnumerator();
    }

    // Storing this to prevent garbage collection
    private NetSimAgent.EntityAddedCallback entityAddedCallback;

    // Storing this to prevent garbage collection
    private NetSimAgent.EntityRemovedCallback entityRemovedCallback;

    // Storing this to prevent garbage collection
    private NetSimAgent.EntityStateCallback entityStateCallback;

    // Storing this to prevent garbage collection
    private NetSimAgent.EntityStateArtPartCallback entityStateArtPartCallback;


    #region SingletonMonoBehaviour 
    protected override void OnNewInstanceCreated()
    {
        ThrowExceptionOnNewInstance();
    }
    #endregion
}
