#include "Precompiled.h"
#include "DLL.h"
#include "Structs.h"

VRLINK_DIS_DLL_API DtEventReportInteraction * CreateEventReportInteraction(unsigned int eventType)
{
	DtEventReportInteraction* eventReport = new DtEventReportInteraction();
	eventReport->setEventType((DtEventType) eventType);
	return eventReport;
}

VRLINK_DIS_DLL_API void DeleteEventReportInteraction(DtEventReportInteraction * eventReport)
{
	delete eventReport;
}

VRLINK_DIS_DLL_API EntityId SenderId(DtEventReportInteraction * eventReport)
{
	EntityId id;
	id.site = eventReport->senderId().site();
	id.host = eventReport->senderId().host();
	id.app = eventReport->senderId().entityNum();
	return id;
}

VRLINK_DIS_DLL_API void SetSenderId(DtEventReportInteraction * eventReport, EntityId senderId)
{
	eventReport->setSenderId(DtEntityIdentifier(senderId.site, senderId.host, senderId.app));
}

VRLINK_DIS_DLL_API EntityId ReceiverId(DtEventReportInteraction * eventReport)
{
	EntityId id;
	id.site = eventReport->receiverId().site();
	id.host = eventReport->receiverId().host();
	id.app  = eventReport->receiverId().entityNum();
	return id;
}

VRLINK_DIS_DLL_API void SetReceiverId(DtEventReportInteraction * eventReport, EntityId recieverId)
{
	eventReport->setReceiverId(DtEntityIdentifier(recieverId.site, recieverId.host, recieverId.app));
}

VRLINK_DIS_DLL_API void AddFixedInt(DtEventReportInteraction * eventReport, int data, unsigned int datumParam)
{
	eventReport->setNumFixedFields(eventReport->numFixedFields() + 1);
	eventReport->setDatumValInt32(DtFixed, eventReport->numFixedFields(), data);
	eventReport->setDatumParam(DtFIXED, eventReport->numFixedFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API void AddFixedUInt(DtEventReportInteraction * eventReport, unsigned int data, unsigned int datumParam)
{
	eventReport->setNumFixedFields(eventReport->numFixedFields() + 1);
	eventReport->setDatumValUnsigned32(DtFixed, eventReport->numFixedFields(), data);
	eventReport->setDatumParam(DtFIXED, eventReport->numFixedFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API void AddFixedFloat(DtEventReportInteraction * eventReport, float data, unsigned int datumParam)
{
	eventReport->setNumFixedFields(eventReport->numFixedFields() + 1);
	eventReport->setDatumValFloat32(DtFixed, eventReport->numFixedFields(), data);
	eventReport->setDatumParam(DtFIXED, eventReport->numFixedFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API void AddVarString(DtEventReportInteraction * eventReport, char * data,int dataLength, unsigned int datumParam)
{
	eventReport->setNumVarFields(eventReport->numVarFields() + 1);
	eventReport->setVarDataBytes(eventReport->numVarFields(), dataLength);
	eventReport->setDatumValByteArray(DtVar, eventReport->numVarFields(), data);
	eventReport->setDatumParam(DtVAR, eventReport->numVarFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API int ReadFixedInt(DtEventReportInteraction * eventReport, int index)
{
	return eventReport->datumValInt32(DtFixed, index);
}

VRLINK_DIS_DLL_API unsigned int ReadFixedUInt(DtEventReportInteraction * eventReport, int index)
{
	return eventReport->datumValUnsigned32(DtFixed, index);
}

VRLINK_DIS_DLL_API float ReadFixedFloat(DtEventReportInteraction * eventReport, int index)
{
	return eventReport->datumValFloat32(DtFixed, index);
}

VRLINK_DIS_DLL_API const char * ReadVarString(DtEventReportInteraction * eventReport, int index)
{
	return eventReport->datumValByteArray(DtVar, index);
}


VRLINK_DIS_DLL_API void SendEventReportInteraction(DtExerciseConn* conn, DtEventReportInteraction * eventReport)
{
	conn->sendStamped(*eventReport);
}

typedef void(__stdcall * EventReportReceivedCallback)(unsigned int, DtEventReportPdu*);
EventReportReceivedCallback _eventReportReceivedCallback = NULL;

void eventReportReceivedCb(DtEventReportPdu* pdu, void* usr)
{
	_eventReportReceivedCallback(pdu->eventType(), pdu);
}

VRLINK_DIS_DLL_API void SetEventReportReceivedCallback(DtExerciseConn* exConn, EventReportReceivedCallback eventReportReceivedCallback)
{
	_eventReportReceivedCallback = eventReportReceivedCallback;

	DtEventReportInteraction::addCallback(exConn, eventReportReceivedCb, NULL);
}