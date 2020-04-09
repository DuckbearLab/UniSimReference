#include "Precompiled.h"
#include "DLL.h"
#include "Structs.h"

VRLINK_DIS_DLL_API DtDataQueryInteraction* CreateDataQueryInteraction()
{
	DtDataQueryInteraction* dataQueryInteraction = new DtDataQueryInteraction();
	return dataQueryInteraction;
}

VRLINK_DIS_DLL_API void DeleteDataQueryInteraction(DtDataQueryInteraction * setDataInteraction)
{
	delete setDataInteraction;
}

VRLINK_DIS_DLL_API EntityId DQISenderId(DtDataQueryInteraction * dataQueryInteraction)
{
	EntityId id;
	id.site = dataQueryInteraction->senderId().site();
	id.host = dataQueryInteraction->senderId().host();
	id.app = dataQueryInteraction->senderId().entityNum();
	return id;
}

VRLINK_DIS_DLL_API void SetDQISenderId(DtDataQueryInteraction * dataQueryInteraction, EntityId senderId)
{
	dataQueryInteraction->setSenderId(DtEntityIdentifier(senderId.site, senderId.host, senderId.app));
}

VRLINK_DIS_DLL_API EntityId DQIReceiverId(DtDataQueryInteraction * dataQueryInteraction)
{
	EntityId id;
	id.site = dataQueryInteraction->receiverId().site();
	id.host = dataQueryInteraction->receiverId().host();
	id.app = dataQueryInteraction->receiverId().entityNum();
	return id;
}

VRLINK_DIS_DLL_API void SetDQIReceiverId(DtDataQueryInteraction * dataQueryInteraction, EntityId recieverId)
{
	dataQueryInteraction->setReceiverId(DtEntityIdentifier(recieverId.site, recieverId.host, recieverId.app));
}

VRLINK_DIS_DLL_API unsigned long DQIRequestId(DtDataQueryInteraction * dataQueryInteraction)
{
	return dataQueryInteraction->requestId();
}

VRLINK_DIS_DLL_API void SetDQIRequestId(DtDataQueryInteraction * dataQueryInteraction, unsigned long requestId)
{
	dataQueryInteraction->setRequestId(requestId);
}

VRLINK_DIS_DLL_API void DQIInitDatumIds(DtDataQueryInteraction * dataQueryInteraction, unsigned long numFixedFields, unsigned long numVarFields)
{
	dataQueryInteraction->initDatumIds(numFixedFields, numVarFields);
}

VRLINK_DIS_DLL_API unsigned long DataQueryInteractionNumFixedFields(DtDataQueryInteraction * dataQueryInteraction)
{
	return dataQueryInteraction->numFixedFields();
}

VRLINK_DIS_DLL_API int DataQueryInteractionFixedDatumId(DtDataQueryInteraction * dataQueryInteraction, int index)
{
	return dataQueryInteraction->fixedDatumId(index);
}

VRLINK_DIS_DLL_API int DataQueryInteractionSetFixedDatumId(DtDataQueryInteraction * dataQueryInteraction, int index, DtDatumParam id)
{
	return dataQueryInteraction->setFixedDatumId(index, id);
}

VRLINK_DIS_DLL_API unsigned long DataQueryInteractionNumVarFields(DtDataQueryInteraction * dataQueryInteraction)
{
	return dataQueryInteraction->numVarFields();
}

VRLINK_DIS_DLL_API int DataQueryInteractionVarDatumId(DtDataQueryInteraction * dataQueryInteraction, int index)
{
	return dataQueryInteraction->varDatumId(index);
}

VRLINK_DIS_DLL_API int DataQueryInteractionSetVarDatumId(DtDataQueryInteraction * dataQueryInteraction, int index, DtDatumParam id)
{
	return dataQueryInteraction->setVarDatumId(index, id);
}
////

VRLINK_DIS_DLL_API void SendDQI(DtExerciseConn* conn, DtDataQueryInteraction * dataQueryInteraction)
{
	conn->sendStamped(*dataQueryInteraction);
}

typedef void(__stdcall * DQIReceivedCallback)(DtDataQueryInteraction*);
DQIReceivedCallback _DQIReceivedCallback = NULL;

void DQIReceivedCb(DtDataQueryInteraction* dataQueryInteraction, void* usr)
{
	_DQIReceivedCallback(dataQueryInteraction);
}

VRLINK_DIS_DLL_API void SetDQIReceivedCallback(DtExerciseConn* exConn, DQIReceivedCallback dQIReceivedCallback)
{
	if (exConn)
	{
		_DQIReceivedCallback = dQIReceivedCallback;

		DtDataQueryInteraction::addCallback(exConn, DQIReceivedCb, NULL);
	}
}