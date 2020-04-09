#include "Precompiled.h"
#include "DLL.h"
#include "Structs.h"

VRLINK_DIS_DLL_API DtDataInteraction* CreateDataInteraction()
{
	DtDataInteraction* dataInteraction = new DtDataInteraction();
	return dataInteraction;
}

VRLINK_DIS_DLL_API void DeleteDataInteraction(DtDataInteraction * dataInteraction)
{
	delete dataInteraction;
}

VRLINK_DIS_DLL_API EntityId DISenderId(DtDataInteraction * dataInteraction)
{
	EntityId id;
	id.site = dataInteraction->senderId().site();
	id.host = dataInteraction->senderId().host();
	id.app = dataInteraction->senderId().entityNum();
	return id;
}

VRLINK_DIS_DLL_API void SetDISenderId(DtDataInteraction * dataInteraction, EntityId senderId)
{
	dataInteraction->setSenderId(DtEntityIdentifier(senderId.site, senderId.host, senderId.app));
}

VRLINK_DIS_DLL_API EntityId DIReceiverId(DtDataInteraction * dataInteraction)
{
	EntityId id;
	id.site = dataInteraction->receiverId().site();
	id.host = dataInteraction->receiverId().host();
	id.app = dataInteraction->receiverId().entityNum();
	return id;
}

VRLINK_DIS_DLL_API void SetDIReceiverId(DtDataInteraction * dataInteraction, EntityId recieverId)
{
	dataInteraction->setReceiverId(DtEntityIdentifier(recieverId.site, recieverId.host, recieverId.app));
}

VRLINK_DIS_DLL_API unsigned long DIRequestId(DtDataInteraction * dataInteraction)
{
	return dataInteraction->requestId();
}

VRLINK_DIS_DLL_API void SetDIRequestId(DtDataInteraction * dataInteraction, unsigned long requestId)
{
	dataInteraction->setRequestId(requestId);
}

VRLINK_DIS_DLL_API void AddDataInteractionFixedInt(DtDataInteraction * dataInteraction, int data, unsigned int datumParam)
{
	dataInteraction->setNumFixedFields(dataInteraction->numFixedFields() + 1);
	dataInteraction->setDatumValInt32(DtFixed, dataInteraction->numFixedFields(), data);
	dataInteraction->setDatumParam(DtFIXED, dataInteraction->numFixedFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API void AddDataInteractionFixedUInt(DtDataInteraction * dataInteraction, unsigned int data, unsigned int datumParam)
{
	dataInteraction->setNumFixedFields(dataInteraction->numFixedFields() + 1);
	dataInteraction->setDatumValUnsigned32(DtFixed, dataInteraction->numFixedFields(), data);
	dataInteraction->setDatumParam(DtFIXED, dataInteraction->numFixedFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API void AddDataInteractionFixedFloat(DtDataInteraction * dataInteraction, float data, unsigned int datumParam)
{
	dataInteraction->setNumFixedFields(dataInteraction->numFixedFields() + 1);
	dataInteraction->setDatumValFloat32(DtFixed, dataInteraction->numFixedFields(), data);
	dataInteraction->setDatumParam(DtFIXED, dataInteraction->numFixedFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API void AddDataInteractionVarString(DtDataInteraction * dataInteraction, char * data, int dataLength, unsigned int datumParam)
{
	dataInteraction->setNumVarFields(dataInteraction->numVarFields() + 1);
	dataInteraction->setVarDataBytes(dataInteraction->numVarFields(), dataLength);
	dataInteraction->setDatumValByteArray(DtVar, dataInteraction->numVarFields(), data);
	dataInteraction->setDatumParam(DtVAR, dataInteraction->numVarFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API int ReadDataInteractionFixedInt(DtDataInteraction * dataInteraction, int index)
{
	return dataInteraction->datumValInt32(DtFixed, index);
}

VRLINK_DIS_DLL_API unsigned int ReadDataInteractionFixedUInt(DtDataInteraction * dataInteraction, int index)
{
	return dataInteraction->datumValUnsigned32(DtFixed, index);
}

VRLINK_DIS_DLL_API float ReadDataInteractionFixedFloat(DtDataInteraction * dataInteraction, int index)
{
	return dataInteraction->datumValFloat32(DtFixed, index);
}

VRLINK_DIS_DLL_API const char * ReadDataInteractionVarString(DtDataInteraction * dataInteraction, int index)
{
	return dataInteraction->datumValByteArray(DtVar, index);
}

VRLINK_DIS_DLL_API int DataInteractionNumFixedFields(DtDataInteraction * dataInteraction)
{
	return dataInteraction->numFixedFields();
}

VRLINK_DIS_DLL_API int DataInteractionFixedDatumId(DtDataInteraction * dataInteraction, int index)
{
	return dataInteraction->fixedDatumId(index);
}

VRLINK_DIS_DLL_API int DataInteractionNumVarFields(DtDataInteraction * dataInteraction)
{
	return dataInteraction->numVarFields();
}

VRLINK_DIS_DLL_API int DataInteractionVarDatumId(DtDataInteraction * dataInteraction, int index)
{
	return dataInteraction->varDatumId(index);
}

VRLINK_DIS_DLL_API void SendDI(DtExerciseConn* conn, DtDataInteraction * dataInteraction)
{
	conn->sendStamped(*dataInteraction);
}

typedef void(__stdcall * DIReceivedCallback)(DtDataInteraction*);
DIReceivedCallback _DIReceivedCallback = NULL;

void DIReceivedCb(DtDataInteraction* dataInteraction, void* usr)
{
	_DIReceivedCallback(dataInteraction);
}

VRLINK_DIS_DLL_API void SetDIReceivedCallback(DtExerciseConn* exConn, DIReceivedCallback dIReceivedCallback)
{
	if (exConn)
	{
		_DIReceivedCallback = dIReceivedCallback;

		DtDataInteraction::addCallback(exConn, DIReceivedCb, NULL);
	}
}