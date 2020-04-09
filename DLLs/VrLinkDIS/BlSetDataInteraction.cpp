#include "Precompiled.h"
#include "DLL.h"
#include "Structs.h"

VRLINK_DIS_DLL_API DtSetDataInteraction* CreateSetDataInteraction()
{
	DtSetDataInteraction* setDataInteraction = new DtSetDataInteraction();
	return setDataInteraction;
}

VRLINK_DIS_DLL_API void DeleteSetDataInteraction(DtSetDataInteraction * setDataInteraction)
{
	delete setDataInteraction;
}

VRLINK_DIS_DLL_API EntityId SDISenderId(DtSetDataInteraction * setDataInteraction)
{
	EntityId id;
	id.site = setDataInteraction->senderId().site();
	id.host = setDataInteraction->senderId().host();
	id.app = setDataInteraction->senderId().entityNum();
	return id;
}

VRLINK_DIS_DLL_API void SetSDISenderId(DtSetDataInteraction * setDataInteraction, EntityId senderId)
{
	setDataInteraction->setSenderId(DtEntityIdentifier(senderId.site, senderId.host, senderId.app));
}

VRLINK_DIS_DLL_API EntityId SDIReceiverId(DtSetDataInteraction * setDataInteraction)
{
	EntityId id;
	id.site = setDataInteraction->receiverId().site();
	id.host = setDataInteraction->receiverId().host();
	id.app = setDataInteraction->receiverId().entityNum();
	return id;
}

VRLINK_DIS_DLL_API void SetSDIReceiverId(DtSetDataInteraction * setDataInteraction, EntityId recieverId)
{
	setDataInteraction->setReceiverId(DtEntityIdentifier(recieverId.site, recieverId.host, recieverId.app));
}

VRLINK_DIS_DLL_API unsigned long SDIRequestId(DtSetDataInteraction * setDataInteraction)
{
	return setDataInteraction->requestId();
}

VRLINK_DIS_DLL_API void SetSDIRequestId(DtSetDataInteraction * setDataInteraction, unsigned long requestId)
{
	setDataInteraction->setRequestId(requestId);
}

VRLINK_DIS_DLL_API void AddSetSetDataInteractionFixedInt(DtSetDataInteraction * setDataInteraction, int data, unsigned int datumParam)
{
	setDataInteraction->setNumFixedFields(setDataInteraction->numFixedFields() + 1);
	setDataInteraction->setDatumValInt32(DtFixed, setDataInteraction->numFixedFields(), data);
	setDataInteraction->setDatumParam(DtFIXED, setDataInteraction->numFixedFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API void AddSetSetDataInteractionFixedUInt(DtSetDataInteraction * setDataInteraction, unsigned int data, unsigned int datumParam)
{
	setDataInteraction->setNumFixedFields(setDataInteraction->numFixedFields() + 1);
	setDataInteraction->setDatumValUnsigned32(DtFixed, setDataInteraction->numFixedFields(), data);
	setDataInteraction->setDatumParam(DtFIXED, setDataInteraction->numFixedFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API void AddSetSetDataInteractionFixedFloat(DtSetDataInteraction * setDataInteraction, float data, unsigned int datumParam)
{
	setDataInteraction->setNumFixedFields(setDataInteraction->numFixedFields() + 1);
	setDataInteraction->setDatumValFloat32(DtFixed, setDataInteraction->numFixedFields(), data);
	setDataInteraction->setDatumParam(DtFIXED, setDataInteraction->numFixedFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API void AddSetSetDataInteractionVarString(DtSetDataInteraction * setDataInteraction, char * data, int dataLength, unsigned int datumParam)
{
	setDataInteraction->setNumVarFields(setDataInteraction->numVarFields() + 1);
	setDataInteraction->setVarDataBytes(setDataInteraction->numVarFields(), dataLength);
	setDataInteraction->setDatumValByteArray(DtVar, setDataInteraction->numVarFields(), data);
	setDataInteraction->setDatumParam(DtVAR, setDataInteraction->numVarFields(), static_cast<DtDatumParam>(datumParam));
}

VRLINK_DIS_DLL_API int ReadSetDataInteractionFixedInt(DtSetDataInteraction * setDataInteraction, int index)
{
	return setDataInteraction->datumValInt32(DtFixed, index);
}

VRLINK_DIS_DLL_API unsigned int ReadSetDataInteractionFixedUInt(DtSetDataInteraction * setDataInteraction, int index)
{
	return setDataInteraction->datumValUnsigned32(DtFixed, index);
}

VRLINK_DIS_DLL_API float ReadSetDataInteractionFixedFloat(DtSetDataInteraction * setDataInteraction, int index)
{
	return setDataInteraction->datumValFloat32(DtFixed, index);
}

VRLINK_DIS_DLL_API const char * ReadSetDataInteractionVarString(DtSetDataInteraction * setDataInteraction, int index)
{
	return setDataInteraction->datumValByteArray(DtVar, index);
}

VRLINK_DIS_DLL_API int SetDataInteractionNumFixedFields(DtSetDataInteraction * setDataInteraction)
{
	return setDataInteraction->numFixedFields();
}

VRLINK_DIS_DLL_API int SetDataInteractionFixedDatumId(DtSetDataInteraction * setDataInteraction, int index)
{
	return setDataInteraction->fixedDatumId(index);
}

VRLINK_DIS_DLL_API int SetDataInteractionNumVarFields(DtSetDataInteraction * setDataInteraction)
{
	return setDataInteraction->numVarFields();
}

VRLINK_DIS_DLL_API int SetDataInteractionVarDatumId(DtSetDataInteraction * setDataInteraction, int index)
{
	return setDataInteraction->varDatumId(index);
}



VRLINK_DIS_DLL_API void SendSDI(DtExerciseConn* conn, DtSetDataInteraction * setDataInteraction)
{
	conn->sendStamped(*setDataInteraction);
}

typedef void(__stdcall * SDIReceivedCallback)(DtSetDataInteraction*);
SDIReceivedCallback _SDIReceivedCallback = NULL;

void SDIReceivedCb(DtSetDataInteraction* setDataInteraction, void* usr)
{
	_SDIReceivedCallback(setDataInteraction);
}

VRLINK_DIS_DLL_API void SetSDIReceivedCallback(DtExerciseConn* exConn, SDIReceivedCallback sDIReceivedCallback)
{
	if (exConn)
	{
		_SDIReceivedCallback = sDIReceivedCallback;

		DtSetDataInteraction::addCallback(exConn, SDIReceivedCb, NULL);
	}
}