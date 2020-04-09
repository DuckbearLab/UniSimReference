using CppStructs;
using NetStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class CppExerciseConnection {

    [DllImport("VrLinkDIS")]
    public extern static IntPtr CreateExerciseConnection(int port, int exerciseId, int siteId, int applicationNumber, string filteredEntities);

    [DllImport("VrLinkDIS")]
    public extern static void UpdateRemoteEntities(IntPtr exConnPtr);

    [DllImport("VrLinkDIS")]
    public extern static IntPtr GetReflectedEntityMarkingText(IntPtr reflectedEntityPtr);

    [DllImport("VrLinkDIS")]
    public extern static void DrainInput(IntPtr exConnPtr);

    [DllImport("VrLinkDIS")]
    public extern static void SetEntityAddedCallback(IntPtr exConnPtr, NetSimAgent.EntityAddedCallback entityAddedCallback);

    [DllImport("VrLinkDIS")]
    public extern static void SetEntityRemovedCallback(IntPtr exConnPtr, NetSimAgent.EntityRemovedCallback entityRemovedCallback);

    [DllImport("VrLinkDIS")]
    public extern static void SetEntityStateCallback(IntPtr exConnPtr, NetSimAgent.EntityStateCallback entityStateCallback);

    [DllImport("VrLinkDIS")]
    public extern static void SetEntityStateArtPartCallback(IntPtr exConnPtr, NetSimAgent.EntityStateArtPartCallback entityStateArtPartCallback);

    #region Fire
    [DllImport("VrLinkDIS")]
    public extern static void SetFireCallback(IntPtr exConnPtr, NetSimAgent.FireCallback fireCallback);

    [DllImport("VrLinkDIS", EntryPoint = "SendFireInteraction")]
    public extern static EventID SendFireInteraction(IntPtr exConnPtr, FireInteraction fireInteraction);
    #endregion

    #region Detonation
    [DllImport("VrLinkDIS")]
    public extern static void SetDetonationCallback(IntPtr exConnPtr, NetSimAgent.DetonationCallback detonationCallback);

    [DllImport("VrLinkDIS", EntryPoint = "SendDetonationInteraction")]
    public extern static void SendDetonationInteraction(IntPtr exConnPtr, DetonationInteraction detInteraction);
    #endregion

    #region CreateEntity
    [DllImport("VrLinkDIS")]
    public extern static void SetCreateEntityCallback(IntPtr exConnPtr, NetSimAgent.CreateEntityCallback createEntityCallback);

    [DllImport("VrLinkDIS", EntryPoint = "SendCreateEntityInteraction")]
    public extern static void SendCreateEntityInteraction(IntPtr exConnPtr, CreateEntityInteraction createEntityInteraction);
    #endregion

    #region RemoveEntity
    [DllImport("VrLinkDIS")]
    public extern static void SetRemoveEntityCallback(IntPtr exConnPtr, NetSimAgent.RemoveEntityCallback removeEntityCallback);

    [DllImport("VrLinkDIS", EntryPoint = "SendRemoveEntityInteraction")]
    public extern static void SendRemoveEntityInteraction(IntPtr exConnPtr, RemoveEntityInteraction removeEntityInteraction);
    #endregion

    #region Acknowledge
    [DllImport("VrLinkDIS")]
    public extern static void SetAcknowledgeCallback(IntPtr exConnPtr, NetSimAgent.AcknowledgeCallback acknowledgeCallback);

    [DllImport("VrLinkDIS", EntryPoint = "SendAcknowledgeInteraction")]
    public extern static void SendAcknowledgeInteraction(IntPtr exConnPtr, AcknowledgeInteraction acknowledgeInteraction);
    #endregion

    [DllImport("VrLinkDIS")]
    public extern static IntPtr CreateEntityPublisher(IntPtr exConnPtr, string entityStypeString);

    [DllImport("VrLinkDIS")]
    public extern static void DeleteEntityPublisher(IntPtr entityPublisher);

    [DllImport("VrLinkDIS")]
    public extern static void SendImgShare(IntPtr exConnPtr, string img, string senderID, string recieverID, int frequency, int requestCounter);

    [DllImport("VrLinkDIS")]
    public extern static void SendCreateEntitySetData(IntPtr exConnPtr, EntityId senderId, EntityId recieverId, int requestId, EntityType entityType, XYZ location, ForceType ForceType, double psi, double theta, double phi);

    [DllImport("VrLinkDIS")]
    public extern static void SendComment(IntPtr exConnPtr, EntityId senderId, EntityId recieverId, string comment);

    #region Coordinates converters
    [DllImport("VrLinkDIS")]
    public extern static void SetRefLatLon(double lat, double lon);

    [DllImport("VrLinkDIS")]
    public extern static XYZ localToUtm(double x, double y, double z);

    [DllImport("VrLinkDIS")]
    public extern static XYZ utmToLocal(double x, double y, double z);

    [DllImport("VrLinkDIS")]
    public extern static XYZ localToGeoc(double x, double y, double z);

    [DllImport("VrLinkDIS")]
    public extern static XYZ geocToLocal(double x, double y, double z);

    [DllImport("VrLinkDIS")]
    public extern static XYZ localToGeod(double x, double y, double z);

    [DllImport("VrLinkDIS")]
    public extern static XYZ geodToLocal(double x, double y, double z);

    [DllImport("VrLinkDIS")]
    public extern static XYZ utmWgs84ToEd50(double x, double y, double z);

    [DllImport("VrLinkDIS")]
    public extern static XYZ utmEd50ToWgs84(double x, double y, double z);
    #endregion

    #region Entity Publishing
    [DllImport("VrLinkDIS")]
    public extern static void TickEntityPublisher(IntPtr entityPublisherPtr, double lat, double lon, double height, bool useTopo, double psi, double theta, double phi, double vx, double vy, double vz, int deadReckThreshold);

    [DllImport("VrLinkDIS")]
    public extern static void EntityPublisherSetMarkingText(IntPtr entityPublisherPtr, string markingText);

    [DllImport("VrLinkDIS")]
    public extern static void EntityPublisherSetEntityId(IntPtr entityPublisherPtr, string entityId);

    [DllImport("VrLinkDIS")]
    public extern static void EntityPublisherSetLifeformState(IntPtr entityPublisherPtr, LifeformState lifeformState);

    [DllImport("VrLinkDIS")]
    public extern static void EntityPublisherSetDamageState(IntPtr entityPublisherPtr, DamageState damageState);

    [DllImport("VrLinkDIS")]
    public extern static void EntityPublisherSetPrimaryWeaponState(IntPtr entityPublisherPtr, WeaponState weaponState);

    [DllImport("VrLinkDIS")]
    public extern static void EntityPublisherSetForceId(IntPtr entityPublisherPtr, ForceType forceType);

    [DllImport("VrLinkDIS")]
    public extern static void EntityPublisherSetArtPart(IntPtr entityPublisherPtr, uint partType, int paramType, float value);
    #endregion

}
