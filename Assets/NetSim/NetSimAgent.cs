using CppStructs;
using NetStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class NetSimAgent
{

    public static INetSimAgent Instance = new VrLinkNetSimAgent();// VrLinkNetSimAgent(); // PitchNetSimAgent();

    #region Deleagtes
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void DIReceivedCallback(IntPtr pduPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void DQIReceivedCallback(IntPtr pduPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void EventReportReceivedCallback(uint eventType, IntPtr pduPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void EntityRemovedCallback(EntityId entityId);

    public delegate void EntityAddedCallback(EntityId entityId);

    public delegate void EntityStateCallback(EntityState entityState);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void EntityStateArtPartCallback(EntityStateArtPart entityStateArtPart);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void FireCallback(FireInteraction fireInteraction);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void DetonationCallback(DetonationInteraction detonationInteraction);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CreateEntityCallback(CreateEntityInteraction createEntityInteraction);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void RemoveEntityCallback(RemoveEntityInteraction removeEntityInteraction);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void AcknowledgeCallback(AcknowledgeInteraction acknowledgeInteraction);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void SDIReceivedCallback(IntPtr pduPtr);
    #endregion

    public interface INetSimAgent
    {
        #region ExerciseConnection
        IntPtr CreateExerciseConnection(int port, int exerciseId, int siteId, int applicationNumber, string filteredEntities);

        void UpdateRemoteEntities(IntPtr exConnPtr);

        string GetReflectedEntityMarkingText(IntPtr reflectedEntityPtr);

        void DrainInput(IntPtr exConnPtr);

        void SetEntityAddedCallback(IntPtr exConnPtr, EntityAddedCallback entityAddedCallback);

        void SetEntityRemovedCallback(IntPtr exConnPtr, EntityRemovedCallback entityRemovedCallback);

        void SetEntityStateCallback(IntPtr exConnPtr, EntityStateCallback entityStateCallback);
        
        void SetEntityStateArtPartCallback(IntPtr exConnPtr, EntityStateArtPartCallback entityStateArtPartCallback);

        #region Fire
        void SetFireCallback(IntPtr exConnPtr, FireCallback fireCallback);

        EventID SendFireInteraction(IntPtr exConnPtr, FireInteraction fireInteraction);
        #endregion

        #region Detonation
        void SetDetonationCallback(IntPtr exConnPtr, DetonationCallback detonationCallback);

        void SendDetonationInteraction(IntPtr exConnPtr, DetonationInteraction detInteraction);
        #endregion

        #region CreateEntity
        void SetCreateEntityCallback(IntPtr exConnPtr, CreateEntityCallback createEntityCallback);

        void SendCreateEntityInteraction(IntPtr exConnPtr, CreateEntityInteraction createEntityInteraction);
        #endregion

        #region RemoveEntity
        void SetRemoveEntityCallback(IntPtr exConnPtr, RemoveEntityCallback removeEntityCallback);

        void SendRemoveEntityInteraction(IntPtr exConnPtr, RemoveEntityInteraction removeEntityInteraction);
        #endregion

        #region Acknowledge
        void SetAcknowledgeCallback(IntPtr exConnPtr, AcknowledgeCallback acknowledgeCallback);

        void SendAcknowledgeInteraction(IntPtr exConnPtr, AcknowledgeInteraction acknowledgeInteraction);
        #endregion

        IntPtr CreateEntityPublisher(IntPtr exConnPtr, string entityStypeString);

        void DeleteEntityPublisher(System.IntPtr exConnPtr, IntPtr entityPublisher);

        void SendImgShare(IntPtr exConnPtr, string img, string senderID, string recieverID, int frequency, int requestCounter);

        void SendCreateEntitySetData(IntPtr exConnPtr, EntityId senderId, EntityId recieverId, int requestId, EntityType entityType, XYZ location, ForceType ForceType, double psi, double theta, double phi);

        void SendComment(IntPtr exConnPtr, EntityId senderId, EntityId recieverId, string comment);

        #region Coordinates converters
        void SetRefLatLon(double lat, double lon);

        XYZ localToUtm(double x, double y, double z);

        XYZ utmToLocal(double x, double y, double z);

        XYZ localToGeoc(double x, double y, double z);

        XYZ geocToLocal(double x, double y, double z);

        XYZ localToGeod(double x, double y, double z);

        XYZ geodToLocal(double x, double y, double z);

        XYZ utmWgs84ToEd50(double x, double y, double z);

        XYZ utmEd50ToWgs84(double x, double y, double z);
        #endregion

        #region Entity Publishing
        void TickEntityPublisher(IntPtr entityPublisherPtr, double lat, double lon, double height, bool useTopo, double psi, double theta, double phi, double vx, double vy, double vz, int deadReckThreshold);

        void EntityPublisherSetMarkingText(IntPtr entityPublisherPtr, string markingText);

        void EntityPublisherSetEntityId(IntPtr entityPublisherPtr, string entityId);

        void EntityPublisherSetLifeformState(IntPtr entityPublisherPtr, LifeformState lifeformState);

        void EntityPublisherSetDamageState(IntPtr entityPublisherPtr, DamageState damageState);

        void EntityPublisherSetPrimaryWeaponState(IntPtr entityPublisherPtr, WeaponState weaponState);

        void EntityPublisherSetForceId(IntPtr entityPublisherPtr, ForceType forceType);

        void EntityPublisherSetArtPart(IntPtr entityPublisherPtr, uint partType, int paramType, float value);
        #endregion
        #endregion

        #region DataInteraction
        IntPtr CreateDataInteraction();

        void DeleteDataInteraction(IntPtr interaction);

        EntityId DISenderId(IntPtr interaction);

        void SetDISenderId(IntPtr interaction, EntityId senderId);

        EntityId DIReceiverId(IntPtr interaction);

        void SetDIReceiverId(IntPtr interaction, EntityId recieverId);

        ulong DIRequestId(IntPtr interaction);

        void SetDIRequestId(IntPtr interaction, ulong requestId);

        void AddDataInteractionFixedInt(IntPtr interaction, int data, uint datumParam);

        int ReadDataInteractionFixedInt(IntPtr interaction, int index);

        void AddDataInteractionFixedUInt(IntPtr interaction, uint data, uint datumParam);

        uint ReadDataInteractionFixedUInt(IntPtr interaction, int index);

        void AddDataInteractionFixedFloat(IntPtr interaction, float data, uint datumParam);

        float ReadDataInteractionFixedFloat(IntPtr interaction, int index);

        void AddDataInteractionVarString(IntPtr interaction, string data, int dataLength, uint datumParam);

        IntPtr ReadDataInteractionVarString(IntPtr interaction, int index);

        int DataInteractionNumFixedFields(IntPtr interaction);

        int DataInteractionFixedDatumId(IntPtr interaction, int index);

        int DataInteractionNumVarFields(IntPtr interaction);

        int DataInteractionVarDatumId(IntPtr interaction, int index);

        void SendDI(IntPtr exConn, IntPtr interaction);

        void SetDIReceivedCallback(IntPtr exConnPtr, DIReceivedCallback dIReceivedCallback);
        #endregion

        #region DataQueryInteraction
        IntPtr CreateDataQueryInteraction();

        void DeleteDataQueryInteraction(IntPtr interaction);

        EntityId DQISenderId(IntPtr interaction);

        void SetDQISenderId(IntPtr interaction, EntityId senderId);

        EntityId DQIReceiverId(IntPtr interaction);

        void SetDQIReceiverId(IntPtr interaction, EntityId recieverId);

        ulong DQIRequestId(IntPtr interaction);

        void SetDQIRequestId(IntPtr interaction, ulong requestId);

        void DQIInitDatumIds(IntPtr interaction, ulong numFixedFields, ulong numVarFields);

        int DataQueryInteractionNumFixedFields(IntPtr interaction);

        int DataQueryInteractionFixedDatumId(IntPtr interaction, int index);

        int DataQueryInteractionSetFixedDatumId(IntPtr interaction, int index, int id);

        int DataQueryInteractionNumVarFields(IntPtr interaction);

        int DataQueryInteractionVarDatumId(IntPtr interaction, int index);

        int DataQueryInteractionSetVarDatumId(IntPtr interaction, int index, int id);

        void SendDQI(IntPtr exConn, IntPtr interaction);

        void SetDQIReceivedCallback(IntPtr exConnPtr, DQIReceivedCallback dIReceivedCallback);
        #endregion

        #region EventReport
        IntPtr CreateEventReportInteraction(uint eventType);

        void DeleteEventReportInteraction(IntPtr pdu);

        void SetSenderId(IntPtr pdu, EntityId senderId);

        EntityId SenderId(IntPtr pdu);

        void SetReceiverId(IntPtr pdu, EntityId recieverId);

        EntityId ReceiverId(IntPtr pdu);

        void AddFixedInt(IntPtr eventReport, int data, uint datumParam);

        int ReadFixedInt(IntPtr eventReport, int index);

        void AddFixedUInt(IntPtr eventReport, uint data, uint datumParam);

        uint ReadFixedUInt(IntPtr eventReport, int index);

        void AddFixedFloat(IntPtr eventReport, float data, uint datumParam);

        float ReadFixedFloat(IntPtr eventReport, int index);

        void AddVarString(IntPtr eventReport, string data, int dataLength, uint datumParam);

        IntPtr ReadVarString(IntPtr eventReport, int index);

        void SendEventReportInteraction(IntPtr exConn, IntPtr eventReport);

        void SetEventReportReceivedCallback(IntPtr exConnPtr, EventReportReceivedCallback eventReportReceivedCallback);
        #endregion

        #region SetDataInteraction
        IntPtr CreateSetDataInteraction();

        void DeleteSetDataInteraction(IntPtr interaction);

        EntityId SDISenderId(IntPtr interaction);

        void SetSDISenderId(IntPtr interaction, EntityId senderId);

        EntityId SDIReceiverId(IntPtr interaction);

        void SetSDIReceiverId(IntPtr interaction, EntityId recieverId);

        ulong SDIRequestId(IntPtr interaction);

        void SetSDIRequestId(IntPtr interaction, ulong requestId);

        void AddSetDataInteractionFixedInt(IntPtr interaction, int data, uint datumParam);

        int ReadSetDataInteractionFixedInt(IntPtr interaction, int index);

        void AddSetDataInteractionFixedUInt(IntPtr interaction, uint data, uint datumParam);

        uint ReadSetDataInteractionFixedUInt(IntPtr interaction, int index);

        void AddSetDataInteractionFixedFloat(IntPtr interaction, float data, uint datumParam);

        float ReadSetDataInteractionFixedFloat(IntPtr interaction, int index);

        void AddSetDataInteractionVarString(IntPtr interaction, string data, int dataLength, uint datumParam);

        IntPtr ReadSetDataInteractionVarString(IntPtr interaction, int index);

        int SetDataInteractionNumFixedFields(IntPtr interaction);

        int SetDataInteractionFixedDatumId(IntPtr interaction, int index);

        int SetDataInteractionNumVarFields(IntPtr interaction);

        int SetDataInteractionVarDatumId(IntPtr interaction, int index);

        void SendSDI(IntPtr exConn, IntPtr interaction);

        void SetSDIReceivedCallback(IntPtr exConnPtr, SDIReceivedCallback sDIReceivedCallback);
        #endregion
    }
}
