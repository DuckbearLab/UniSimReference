using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrLinkNetSimAgent : NetSimAgent.INetSimAgent {

    public System.IntPtr CreateExerciseConnection(int port, int exerciseId, int siteId, int applicationNumber, string filteredEntities)
    {
        return CppExerciseConnection.CreateExerciseConnection(port, exerciseId, siteId, applicationNumber, filteredEntities);
    }

    public void UpdateRemoteEntities(System.IntPtr exConnPtr)
    {
        CppExerciseConnection.UpdateRemoteEntities(exConnPtr);
    }

    public string GetReflectedEntityMarkingText(System.IntPtr reflectedEntityPtr)
    {
        System.IntPtr markingTextPtr = CppExerciseConnection.GetReflectedEntityMarkingText(reflectedEntityPtr);
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(markingTextPtr);
    }

    public void DrainInput(System.IntPtr exConnPtr)
    {
        CppExerciseConnection.DrainInput(exConnPtr);
    }

    public void SetEntityAddedCallback(System.IntPtr exConnPtr, NetSimAgent.EntityAddedCallback entityAddedCallback)
    {
        CppExerciseConnection.SetEntityAddedCallback(exConnPtr, entityAddedCallback);
    }

    public void SetEntityRemovedCallback(System.IntPtr exConnPtr, NetSimAgent.EntityRemovedCallback entityRemovedCallback)
    {
        CppExerciseConnection.SetEntityRemovedCallback(exConnPtr, entityRemovedCallback);
    }

    public void SetEntityStateCallback(System.IntPtr exConnPtr, NetSimAgent.EntityStateCallback entityStateCallback)
    {
        CppExerciseConnection.SetEntityStateCallback(exConnPtr, entityStateCallback);
    }

    public void SetEntityStateArtPartCallback(System.IntPtr exConnPtr, NetSimAgent.EntityStateArtPartCallback entityStateArtPartCallback)
    {
        CppExerciseConnection.SetEntityStateArtPartCallback(exConnPtr, entityStateArtPartCallback);
    }

    public void SetFireCallback(System.IntPtr exConnPtr, NetSimAgent.FireCallback fireCallback)
    {
        CppExerciseConnection.SetFireCallback(exConnPtr, fireCallback);
    }

    public NetStructs.EventID SendFireInteraction(System.IntPtr exConnPtr, NetStructs.FireInteraction fireInteraction)
    {
        return CppExerciseConnection.SendFireInteraction(exConnPtr, fireInteraction);
    }

    public void SetDetonationCallback(System.IntPtr exConnPtr, NetSimAgent.DetonationCallback detonationCallback)
    {
        CppExerciseConnection.SetDetonationCallback(exConnPtr, detonationCallback);
    }

    public void SendDetonationInteraction(System.IntPtr exConnPtr, NetStructs.DetonationInteraction detInteraction)
    {
        CppExerciseConnection.SendDetonationInteraction(exConnPtr, detInteraction);
    }

    public void SetCreateEntityCallback(System.IntPtr exConnPtr, NetSimAgent.CreateEntityCallback createEntityCallback)
    {
        CppExerciseConnection.SetCreateEntityCallback(exConnPtr, createEntityCallback);
    }

    public void SendCreateEntityInteraction(System.IntPtr exConnPtr, NetStructs.CreateEntityInteraction createEntityInteraction)
    {
        CppExerciseConnection.SendCreateEntityInteraction(exConnPtr, createEntityInteraction);
    }

    public void SetRemoveEntityCallback(System.IntPtr exConnPtr, NetSimAgent.RemoveEntityCallback removeEntityCallback)
    {
        CppExerciseConnection.SetRemoveEntityCallback(exConnPtr, removeEntityCallback);
    }

    public void SendRemoveEntityInteraction(System.IntPtr exConnPtr, NetStructs.RemoveEntityInteraction removeEntityInteraction)
    {
        CppExerciseConnection.SendRemoveEntityInteraction(exConnPtr, removeEntityInteraction);
    }

    public void SetAcknowledgeCallback(System.IntPtr exConnPtr, NetSimAgent.AcknowledgeCallback acknowledgeCallback)
    {
        CppExerciseConnection.SetAcknowledgeCallback(exConnPtr, acknowledgeCallback);
    }

    public void SendAcknowledgeInteraction(System.IntPtr exConnPtr, NetStructs.AcknowledgeInteraction acknowledgeInteraction)
    {
        CppExerciseConnection.SendAcknowledgeInteraction(exConnPtr, acknowledgeInteraction);
    }

    public System.IntPtr CreateEntityPublisher(System.IntPtr exConnPtr, string entityStypeString)
    {
        return CppExerciseConnection.CreateEntityPublisher(exConnPtr, entityStypeString);
    }

    public void DeleteEntityPublisher(System.IntPtr exConnPtr, System.IntPtr entityPublisher)
    {
        CppExerciseConnection.DeleteEntityPublisher(entityPublisher);
    }

    public void SendImgShare(System.IntPtr exConnPtr, string img, string senderID, string recieverID, int frequency, int requestCounter)
    {
        CppExerciseConnection.SendImgShare(exConnPtr, img, senderID, recieverID, frequency, requestCounter);
    }

    public void SendCreateEntitySetData(System.IntPtr exConnPtr, NetStructs.EntityId senderId, NetStructs.EntityId recieverId, int requestId, NetStructs.EntityType entityType, CppStructs.XYZ location, NetStructs.ForceType ForceType, double psi, double theta, double phi)
    {
        CppExerciseConnection.SendCreateEntitySetData(exConnPtr, senderId, recieverId, requestId, entityType, location, ForceType, psi, theta, phi);
    }

    public void SendComment(System.IntPtr exConnPtr, NetStructs.EntityId senderId, NetStructs.EntityId recieverId, string comment)
    {
        CppExerciseConnection.SendComment(exConnPtr, senderId, recieverId, comment);
    }

    public void SetRefLatLon(double lat, double lon)
    {
        CppExerciseConnection.SetRefLatLon(lat, lon);
    }

    public CppStructs.XYZ localToUtm(double x, double y, double z)
    {
        return CppExerciseConnection.localToUtm(x, y, z);
    }

    public CppStructs.XYZ utmToLocal(double x, double y, double z)
    {
        return CppExerciseConnection.utmToLocal(x, y, z);
    }

    public CppStructs.XYZ localToGeoc(double x, double y, double z)
    {
        return CppExerciseConnection.localToGeoc(x, y, z);
    }

    public CppStructs.XYZ geocToLocal(double x, double y, double z)
    {
        return CppExerciseConnection.geocToLocal(x, y, z);
    }

    public CppStructs.XYZ localToGeod(double x, double y, double z)
    {
        return CppExerciseConnection.localToGeod(x, y, z);
    }

    public CppStructs.XYZ geodToLocal(double x, double y, double z)
    {
        return CppExerciseConnection.geodToLocal(x, y, z);
    }

    public CppStructs.XYZ utmWgs84ToEd50(double x, double y, double z)
    {
        return CppExerciseConnection.utmWgs84ToEd50(x, y, z);
    }

    public CppStructs.XYZ utmEd50ToWgs84(double x, double y, double z)
    {
        return CppExerciseConnection.utmEd50ToWgs84(x, y, z);
    }

    public void TickEntityPublisher(System.IntPtr entityPublisherPtr, double lat, double lon, double height, bool useTopo, double psi, double theta, double phi, double vx, double vy, double vz, int deadReckThreshold)
    {
        CppExerciseConnection.TickEntityPublisher(entityPublisherPtr, lat, lon, height, useTopo, psi, theta, phi, vx, vy, vz, deadReckThreshold);
    }

    public void EntityPublisherSetMarkingText(System.IntPtr entityPublisherPtr, string markingText)
    {
        CppExerciseConnection.EntityPublisherSetMarkingText(entityPublisherPtr, markingText);
    }

    public void EntityPublisherSetEntityId(System.IntPtr entityPublisherPtr, string entityId)
    {
        CppExerciseConnection.EntityPublisherSetEntityId(entityPublisherPtr, entityId);
    }

    public void EntityPublisherSetLifeformState(System.IntPtr entityPublisherPtr, NetStructs.LifeformState lifeformState)
    {
        CppExerciseConnection.EntityPublisherSetLifeformState(entityPublisherPtr, lifeformState);
    }

    public void EntityPublisherSetDamageState(System.IntPtr entityPublisherPtr, NetStructs.DamageState damageState)
    {
        CppExerciseConnection.EntityPublisherSetDamageState(entityPublisherPtr, damageState);
    }

    public void EntityPublisherSetPrimaryWeaponState(System.IntPtr entityPublisherPtr, NetStructs.WeaponState weaponState)
    {
        CppExerciseConnection.EntityPublisherSetPrimaryWeaponState(entityPublisherPtr, weaponState);
    }

    public void EntityPublisherSetForceId(System.IntPtr entityPublisherPtr, NetStructs.ForceType forceType)
    {
        CppExerciseConnection.EntityPublisherSetForceId(entityPublisherPtr, forceType);
    }

    public void EntityPublisherSetArtPart(System.IntPtr entityPublisherPtr, uint partType, int paramType, float value)
    {
        CppExerciseConnection.EntityPublisherSetArtPart(entityPublisherPtr, partType, paramType, value);
    }

    public System.IntPtr CreateDataInteraction()
    {
        return CppDataInteraction.CreateDataInteraction();
    }

    public void DeleteDataInteraction(System.IntPtr interaction)
    {
        CppDataInteraction.DeleteDataInteraction(interaction);
    }

    public NetStructs.EntityId DISenderId(System.IntPtr interaction)
    {
        return CppDataInteraction.DISenderId(interaction);
    }

    public void SetDISenderId(System.IntPtr interaction, NetStructs.EntityId senderId)
    {
        CppDataInteraction.SetDISenderId(interaction, senderId);
    }

    public NetStructs.EntityId DIReceiverId(System.IntPtr interaction)
    {
        return CppDataInteraction.DIReceiverId(interaction);
    }

    public void SetDIReceiverId(System.IntPtr interaction, NetStructs.EntityId recieverId)
    {
        CppDataInteraction.SetDIReceiverId(interaction, recieverId);
    }

    public ulong DIRequestId(System.IntPtr interaction)
    {
        return CppDataInteraction.DIRequestId(interaction);
    }

    public void SetDIRequestId(System.IntPtr interaction, ulong requestId)
    {
        CppDataInteraction.SetDIRequestId(interaction, requestId);
    }

    public void AddDataInteractionFixedInt(System.IntPtr interaction, int data, uint datumParam)
    {
        CppDataInteraction.AddDataInteractionFixedInt(interaction, data, datumParam);
    }

    public int ReadDataInteractionFixedInt(System.IntPtr interaction, int index)
    {
        return CppDataInteraction.ReadDataInteractionFixedInt(interaction, index);
    }

    public void AddDataInteractionFixedUInt(System.IntPtr interaction, uint data, uint datumParam)
    {
        CppDataInteraction.AddDataInteractionFixedUInt(interaction, data, datumParam);
    }

    public uint ReadDataInteractionFixedUInt(System.IntPtr interaction, int index)
    {
        return CppDataInteraction.ReadDataInteractionFixedUInt(interaction, index);
    }

    public void AddDataInteractionFixedFloat(System.IntPtr interaction, float data, uint datumParam)
    {
        CppDataInteraction.AddDataInteractionFixedFloat(interaction, data, datumParam);
    }

    public float ReadDataInteractionFixedFloat(System.IntPtr interaction, int index)
    {
        return CppDataInteraction.ReadDataInteractionFixedFloat(interaction, index);
    }

    public void AddDataInteractionVarString(System.IntPtr interaction, string data, int dataLength, uint datumParam)
    {
        CppDataInteraction.AddDataInteractionVarString(interaction, data, dataLength, datumParam);
    }

    public System.IntPtr ReadDataInteractionVarString(System.IntPtr interaction, int index)
    {
        return CppDataInteraction.ReadDataInteractionVarString(interaction, index);
    }

    public int DataInteractionNumFixedFields(System.IntPtr interaction)
    {
        return CppDataInteraction.DataInteractionNumFixedFields(interaction);
    }

    public int DataInteractionFixedDatumId(System.IntPtr interaction, int index)
    {
        return CppDataInteraction.DataInteractionFixedDatumId(interaction, index);
    }

    public int DataInteractionNumVarFields(System.IntPtr interaction)
    {
        return CppDataInteraction.DataInteractionNumVarFields(interaction);
    }

    public int DataInteractionVarDatumId(System.IntPtr interaction, int index)
    {
        return CppDataInteraction.DataInteractionVarDatumId(interaction, index);
    }

    public void SendDI(System.IntPtr exConn, System.IntPtr interaction)
    {
        CppDataInteraction.SendDI(exConn, interaction);
    }

    public void SetDIReceivedCallback(System.IntPtr exConnPtr, NetSimAgent.DIReceivedCallback dIReceivedCallback)
    {
        CppDataInteraction.SetDIReceivedCallback(exConnPtr, dIReceivedCallback);
    }

    public System.IntPtr CreateDataQueryInteraction()
    {
        return CppDataQueryInteraction.CreateDataQueryInteraction();
    }

    public void DeleteDataQueryInteraction(System.IntPtr interaction)
    {
        CppDataQueryInteraction.DeleteDataQueryInteraction(interaction);
    }

    public NetStructs.EntityId DQISenderId(System.IntPtr interaction)
    {
        return CppDataQueryInteraction.DQISenderId(interaction);
    }

    public void SetDQISenderId(System.IntPtr interaction, NetStructs.EntityId senderId)
    {
        CppDataQueryInteraction.SetDQISenderId(interaction, senderId);
    }

    public NetStructs.EntityId DQIReceiverId(System.IntPtr interaction)
    {
        return CppDataQueryInteraction.DQIReceiverId(interaction);
    }

    public void SetDQIReceiverId(System.IntPtr interaction, NetStructs.EntityId recieverId)
    {
        CppDataQueryInteraction.SetDQIReceiverId(interaction, recieverId);
    }

    public ulong DQIRequestId(System.IntPtr interaction)
    {
        return CppDataQueryInteraction.DQIRequestId(interaction);
    }

    public void SetDQIRequestId(System.IntPtr interaction, ulong requestId)
    {
        CppDataQueryInteraction.SetDQIRequestId(interaction, requestId);
    }

    public void DQIInitDatumIds(System.IntPtr interaction, ulong numFixedFields, ulong numVarFields)
    {
        CppDataQueryInteraction.DQIInitDatumIds(interaction, numFixedFields, numVarFields);
    }

    public int DataQueryInteractionNumFixedFields(System.IntPtr interaction)
    {
        return CppDataQueryInteraction.DataQueryInteractionNumFixedFields(interaction);
    }

    public int DataQueryInteractionFixedDatumId(System.IntPtr interaction, int index)
    {
        return CppDataQueryInteraction.DataQueryInteractionFixedDatumId(interaction, index);
    }

    public int DataQueryInteractionSetFixedDatumId(System.IntPtr interaction, int index, int id)
    {
        return CppDataQueryInteraction.DataQueryInteractionSetFixedDatumId(interaction, index, id);
    }

    public int DataQueryInteractionNumVarFields(System.IntPtr interaction)
    {
        return CppDataQueryInteraction.DataQueryInteractionNumVarFields(interaction);
    }

    public int DataQueryInteractionVarDatumId(System.IntPtr interaction, int index)
    {
        return CppDataQueryInteraction.DataQueryInteractionVarDatumId(interaction, index);
    }

    public int DataQueryInteractionSetVarDatumId(System.IntPtr interaction, int index, int id)
    {
        return CppDataQueryInteraction.DataQueryInteractionSetVarDatumId(interaction, index, id);
    }

    public void SendDQI(System.IntPtr exConn, System.IntPtr interaction)
    {
        CppDataQueryInteraction.SendDQI(exConn, interaction);
    }

    public void SetDQIReceivedCallback(System.IntPtr exConnPtr, NetSimAgent.DQIReceivedCallback dIReceivedCallback)
    {
        CppDataQueryInteraction.SetDQIReceivedCallback(exConnPtr, dIReceivedCallback);
    }

    public System.IntPtr CreateEventReportInteraction(uint eventType)
    {
        return CppEventReport.CreateEventReportInteraction(eventType);
    }

    public void DeleteEventReportInteraction(System.IntPtr pdu)
    {
        CppEventReport.DeleteEventReportInteraction(pdu);
    }

    public void SetSenderId(System.IntPtr pdu, NetStructs.EntityId senderId)
    {
        CppEventReport.SetSenderId(pdu, senderId);
    }

    public NetStructs.EntityId SenderId(System.IntPtr pdu)
    {
        return CppEventReport.SenderId(pdu);
    }

    public void SetReceiverId(System.IntPtr pdu, NetStructs.EntityId recieverId)
    {
        CppEventReport.SetReceiverId(pdu, recieverId);
    }

    public NetStructs.EntityId ReceiverId(System.IntPtr pdu)
    {
        return CppEventReport.ReceiverId(pdu);
    }

    public void AddFixedInt(System.IntPtr eventReport, int data, uint datumParam)
    {
        CppEventReport.AddFixedInt(eventReport, data, datumParam);
    }

    public int ReadFixedInt(System.IntPtr eventReport, int index)
    {
        return CppEventReport.ReadFixedInt(eventReport, index);
    }

    public void AddFixedUInt(System.IntPtr eventReport, uint data, uint datumParam)
    {
        CppEventReport.AddFixedUInt(eventReport, data, datumParam);
    }

    public uint ReadFixedUInt(System.IntPtr eventReport, int index)
    {
        return CppEventReport.ReadFixedUInt(eventReport, index);
    }

    public void AddFixedFloat(System.IntPtr eventReport, float data, uint datumParam)
    {
        CppEventReport.AddFixedFloat(eventReport, data, datumParam);
    }

    public float ReadFixedFloat(System.IntPtr eventReport, int index)
    {
        return CppEventReport.ReadFixedFloat(eventReport, index);
    }

    public void AddVarString(System.IntPtr eventReport, string data, int dataLength, uint datumParam)
    {
        CppEventReport.AddVarString(eventReport, data, dataLength, datumParam);
    }

    public System.IntPtr ReadVarString(System.IntPtr eventReport, int index)
    {
        return CppEventReport.ReadVarString(eventReport, index);
    }

    public void SendEventReportInteraction(System.IntPtr exConn, System.IntPtr eventReport)
    {
        CppEventReport.SendEventReportInteraction(exConn, eventReport);
    }

    public void SetEventReportReceivedCallback(System.IntPtr exConnPtr, NetSimAgent.EventReportReceivedCallback eventReportReceivedCallback)
    {
        CppEventReport.SetEventReportReceivedCallback(exConnPtr, eventReportReceivedCallback);
    }

    public System.IntPtr CreateSetDataInteraction()
    {
        return CppSetDataInteraction.CreateSetDataInteraction();
    }

    public void DeleteSetDataInteraction(System.IntPtr interaction)
    {
        CppSetDataInteraction.DeleteSetDataInteraction(interaction);
    }

    public NetStructs.EntityId SDISenderId(System.IntPtr interaction)
    {
        return CppSetDataInteraction.SDISenderId(interaction);
    }

    public void SetSDISenderId(System.IntPtr interaction, NetStructs.EntityId senderId)
    {
        CppSetDataInteraction.SetSDISenderId(interaction, senderId);
    }

    public NetStructs.EntityId SDIReceiverId(System.IntPtr interaction)
    {
        return CppSetDataInteraction.SDIReceiverId(interaction);
    }

    public void SetSDIReceiverId(System.IntPtr interaction, NetStructs.EntityId recieverId)
    {
        CppSetDataInteraction.SetSDIReceiverId(interaction, recieverId);
    }

    public ulong SDIRequestId(System.IntPtr interaction)
    {
        return CppSetDataInteraction.SDIRequestId(interaction);
    }

    public void SetSDIRequestId(System.IntPtr interaction, ulong requestId)
    {
        CppSetDataInteraction.SetSDIRequestId(interaction, requestId);
    }

    public void AddSetDataInteractionFixedInt(System.IntPtr interaction, int data, uint datumParam)
    {
        CppSetDataInteraction.AddSetDataInteractionFixedInt(interaction, data, datumParam);
    }

    public int ReadSetDataInteractionFixedInt(System.IntPtr interaction, int index)
    {
        return CppSetDataInteraction.ReadSetDataInteractionFixedInt(interaction, index);
    }

    public void AddSetDataInteractionFixedUInt(System.IntPtr interaction, uint data, uint datumParam)
    {
        CppSetDataInteraction.AddSetDataInteractionFixedUInt(interaction, data, datumParam);
    }

    public uint ReadSetDataInteractionFixedUInt(System.IntPtr interaction, int index)
    {
        return CppSetDataInteraction.ReadSetDataInteractionFixedUInt(interaction, index);
    }

    public void AddSetDataInteractionFixedFloat(System.IntPtr interaction, float data, uint datumParam)
    {
        CppSetDataInteraction.AddSetDataInteractionFixedFloat(interaction, data, datumParam);
    }

    public float ReadSetDataInteractionFixedFloat(System.IntPtr interaction, int index)
    {
        return CppSetDataInteraction.ReadSetDataInteractionFixedFloat(interaction, index);
    }

    public void AddSetDataInteractionVarString(System.IntPtr interaction, string data, int dataLength, uint datumParam)
    {
        CppSetDataInteraction.AddSetDataInteractionVarString(interaction, data, dataLength, datumParam);
    }

    public System.IntPtr ReadSetDataInteractionVarString(System.IntPtr interaction, int index)
    {
        return CppSetDataInteraction.ReadSetDataInteractionVarString(interaction, index);
    }

    public int SetDataInteractionNumFixedFields(System.IntPtr interaction)
    {
        return CppSetDataInteraction.SetDataInteractionNumFixedFields(interaction);
    }

    public int SetDataInteractionFixedDatumId(System.IntPtr interaction, int index)
    {
        return CppSetDataInteraction.SetDataInteractionFixedDatumId(interaction, index);
    }

    public int SetDataInteractionNumVarFields(System.IntPtr interaction)
    {
        return CppSetDataInteraction.SetDataInteractionNumVarFields(interaction);
    }

    public int SetDataInteractionVarDatumId(System.IntPtr interaction, int index)
    {
        return CppSetDataInteraction.SetDataInteractionVarDatumId(interaction, index);
    }

    public void SendSDI(System.IntPtr exConn, System.IntPtr interaction)
    {
        CppSetDataInteraction.SendSDI(exConn, interaction);
    }

    public void SetSDIReceivedCallback(System.IntPtr exConnPtr, NetSimAgent.SDIReceivedCallback sDIReceivedCallback)
    {
        CppSetDataInteraction.SetSDIReceivedCallback(exConnPtr, sDIReceivedCallback);
    }
}
