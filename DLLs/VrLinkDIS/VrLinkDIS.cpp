#include "Precompiled.h"
#include "DLL.h"
#include "Structs.h"
#include<iostream>

DtVector localToGeoc(const DtVector& posLocal);
DtVector geocToLocal(const DtVector& geocLocal);

DtVector utmToGeoc(const DtVector& utmLoc);
DtVector geocToUtm(const DtVector& geocLoc);

DtVector utmWgs84ToEd50(const DtVector& wgs84);
DtVector utmEd50ToWgs84(const DtVector& ed50);

DtReflectedEntityList* reflectedEntities = NULL;
std::vector<DtEntityType> m_filteredEntities;

typedef void(__stdcall * EntityAddedCallback)(EntityId);
EntityAddedCallback _entityAddedCallback = NULL;
typedef void(__stdcall * EntityRemovedCallback)(EntityId);
EntityRemovedCallback _entityRemovedCallback = NULL;
typedef void(__stdcall * EntityStateCallback)(EntityState);
EntityStateCallback _entityStateCallback = NULL;
typedef void(__stdcall * EntityStateArtPartCallback)(EntityStateArtPart);
EntityStateArtPartCallback _entityStateArtPartCallback = NULL;

typedef void(__stdcall * FireCallback)(FireInteraction);
FireCallback _fireCallback = NULL;
typedef void(__stdcall * DetonationCallback)(DetonationInteraction);
DetonationCallback _detonationCallback = NULL;

typedef void(__stdcall * CreateEntityCallback)(CreateEntityInteraction);
CreateEntityCallback _createEntityCallback = NULL;
typedef void(__stdcall * AcknowledgeCallback)(AcknowledgeInteraction);
AcknowledgeCallback _acknowledgeCallback = NULL;
typedef void(__stdcall * RemoveEntityCallback)(RemoveEntityInteraction);
RemoveEntityCallback _removeEntityCallback = NULL;

double refLat = 51.507351;
double refLon = -0.127758;

void processFilteredEntities(char* filteredEntities)
{
	char* sp = strtok(filteredEntities, ",");
	while (sp != NULL)
	{
		m_filteredEntities.push_back(DtEntityType(sp));
		//MessageBox(NULL, sp, NULL, NULL);
		sp = strtok(NULL, ",");
	}
}

VRLINK_DIS_DLL_API DtExerciseConn* CreateExerciseConnection(int port, int exerciseId, int siteId, int applicationNumber, char* filteredEntities)
{
	m_filteredEntities.clear();

	DtExerciseConnInitializer exInit;
	exInit.setPort(port);
	exInit.setExerciseId(exerciseId);
	exInit.setSiteId(siteId);
	exInit.setApplicationNumber(applicationNumber);
	exInit.setDisVersionToSend(5);
	//////////////////////////////////////////////////////////////////////////
	processFilteredEntities(filteredEntities);
	//////////////////////////////////////////////////////////////////////////
	try
	{
		DtExerciseConn* exConn = new DtExerciseConn(exInit);

		reflectedEntities = new DtReflectedEntityList(exConn);
		return exConn;
	}
	catch (DtException e)
	{
		std::cout <<e.what();
		std::cout << e.where();
		
	}
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void DrainInput(DtExerciseConn* exConn)
{
	if (exConn)
	{
		exConn->clock()->setSimTime(exConn->clock()->elapsedRealTime());
		exConn->drainInput();
	}
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void UpdateRemoteEntities(DtExerciseConn* exConn)
{
	if (exConn && _entityStateCallback)
	{
		for (DtReflectedEntity* refEntity = reflectedEntities->first(); refEntity != NULL; refEntity = refEntity->next())
		{
			
			DtEntityStateRepository* entityEsr = refEntity->esr();
			if (entityEsr == NULL)
				continue;
			
			// Ignore unwanted entities.
			// Unwanted entities contained in m_filteredEntities.
			// m_filteredEntities initialized in CreateExerciseConnection func'.

			if ((std::find(m_filteredEntities.begin(), m_filteredEntities.end(), entityEsr->entityType()) != m_filteredEntities.end()))
				continue;
			


			EntityState es;
			
			es.reflectedEntityPtr = refEntity;

			es.entityId.site = entityEsr->entityId().site();
			es.entityId.host = entityEsr->entityId().host();
			es.entityId.app = entityEsr->entityId().entityNum();

			es.entityType.entityKind = entityEsr->entityType().kind();
			es.entityType.domain = entityEsr->entityType().domain();
			es.entityType.country = entityEsr->entityType().country();
			es.entityType.category = entityEsr->entityType().category();
			es.entityType.subCategory = entityEsr->entityType().subCategory();
			es.entityType.specific = entityEsr->entityType().specific();
			es.entityType.extra = entityEsr->entityType().extra();

			DtVector localPosition = geocToLocal(DtVector(entityEsr->location().x(), entityEsr->location().y(), entityEsr->location().z()));
			es.posX = localPosition.x();
			es.posY = localPosition.y();
			es.posZ = localPosition.z();

			DtVector localPositionAfterVelocity = geocToLocal(DtVector(entityEsr->location().x() + entityEsr->velocity().x(), entityEsr->location().y() + entityEsr->velocity().y(), entityEsr->location().z() + entityEsr->velocity().z()));
			DtVector velocity = localPositionAfterVelocity - localPosition;

			DtTopoView view(entityEsr, DtDeg2Rad(refLat), DtDeg2Rad(refLon));
			es.rotX = DtRad2Deg(view.orientation().psi());
			es.rotY = DtRad2Deg(view.orientation().theta());
			es.rotZ = DtRad2Deg(view.orientation().phi());

			es.velX = velocity.x();
			es.velY = velocity.y();
			es.velZ = velocity.z();

			es.lifeformState = entityEsr->lifeformState();
			es.damageState = entityEsr->damageState();
			es.forceType = entityEsr->forceId();
			es.primaryWeaponState = entityEsr->primaryWeaponState();

			_entityStateCallback(es);

			
			DtArticulatedPartCollection::const_iterator artPart;
			for (artPart = entityEsr->artPartList()->begin(); artPart != entityEsr->artPartList()->end(); artPart++)
			{
				EntityStateArtPart esap;

				esap.entityId.site = entityEsr->entityId().site();
				esap.entityId.host = entityEsr->entityId().host();
				esap.entityId.app = entityEsr->entityId().entityNum();
				
				esap.partId = artPart->first;

				esap.posX = artPart->second->translation().x();
				esap.posY = artPart->second->translation().y();
				esap.posZ = artPart->second->translation().z();

				esap.rotX = DtRad2Deg(artPart->second->orientation().psi());
				esap.rotY = DtRad2Deg(artPart->second->orientation().theta());
				esap.rotZ = DtRad2Deg(artPart->second->orientation().phi());

				_entityStateArtPartCallback(esap);
			}
		}

	}
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API const char* GetReflectedEntityMarkingText(DtReflectedEntity* reflectedEntity)
{
	if (reflectedEntity)
		return reflectedEntity->esr()->markingText();
	else
		return "Hmm?";
}
// ======================================================================================================== //
static void entityAddedCb(DtReflectedEntity* newEntity, void* usr)
{
	EntityId entityId;
	entityId.site = newEntity->esr()->entityId().site();
	entityId.host = newEntity->esr()->entityId().host();
	entityId.app = newEntity->esr()->entityId().entityNum();
	_entityAddedCallback(entityId);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SetEntityAddedCallback(DtExerciseConn* exConn, EntityAddedCallback entityAddedCallback)
{
	if (exConn)
	{
		_entityAddedCallback = entityAddedCallback;

		reflectedEntities->addEntityAdditionCallback(entityAddedCb, NULL);
	}
}
// ======================================================================================================== //
static void entityRemovedCb(DtReflectedEntity* removedEntity, void* usr)
{
	EntityId entityId;
	entityId.site = removedEntity->esr()->entityId().site();
	entityId.host = removedEntity->esr()->entityId().host();
	entityId.app = removedEntity->esr()->entityId().entityNum();
	_entityRemovedCallback(entityId);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SetEntityRemovedCallback(DtExerciseConn* exConn, EntityRemovedCallback entityRemovedCallback)
{
	if (exConn)
	{
		_entityRemovedCallback = entityRemovedCallback;

		reflectedEntities->addEntityRemovalCallback(entityRemovedCb, NULL);
	}
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SetEntityStateCallback(DtExerciseConn* exConn, EntityStateCallback entityStateCallback)
{
	if (exConn)
	{
		_entityStateCallback = entityStateCallback;
		reflectedEntities = new DtReflectedEntityList(exConn);
	}
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SetEntityStateArtPartCallback(DtExerciseConn* exConn, EntityStateArtPartCallback entityStateArtPartCallback)
{
	if (exConn)
	{
		_entityStateArtPartCallback = entityStateArtPartCallback;
	}
}
// ======================================================================================================== //
static void fireCb(DtFireInteraction* pdu, void* usr)
{
	FireInteraction fireInteraction;

	fireInteraction.attacker.site = pdu->attackerId().site();
	fireInteraction.attacker.host = pdu->attackerId().host();
	fireInteraction.attacker.app = pdu->attackerId().entityNum();

	fireInteraction.munitionType.entityKind = pdu->munitionType().kind();
	fireInteraction.munitionType.domain = pdu->munitionType().domain();
	fireInteraction.munitionType.country = pdu->munitionType().country();
	fireInteraction.munitionType.category = pdu->munitionType().category();
	fireInteraction.munitionType.subCategory = pdu->munitionType().subCategory();
	fireInteraction.munitionType.specific = pdu->munitionType().specific();
	fireInteraction.munitionType.extra = pdu->munitionType().extra();

	fireInteraction.target.site = pdu->targetId().site();
	fireInteraction.target.host = pdu->targetId().host();
	fireInteraction.target.app = pdu->targetId().entityNum();

	fireInteraction.munition.site = pdu->munitionId().site();
	fireInteraction.munition.host = pdu->munitionId().host();
	fireInteraction.munition.app = pdu->munitionId().entityNum();

	fireInteraction.eventId.site = pdu->eventId().site();
	fireInteraction.eventId.host = pdu->eventId().host();
	fireInteraction.eventId.eventNum = pdu->eventId().eventNum();

	DtVector linVelocity = DtVector32To64(pdu->velocity());
	fireInteraction.linVelocity.X = linVelocity.x();
	fireInteraction.linVelocity.Y = linVelocity.y();
	fireInteraction.linVelocity.Z = linVelocity.z();

	DtVector localPosition = geocToLocal(pdu->location());
	fireInteraction.location.X = localPosition.x();
	fireInteraction.location.Y = localPosition.y();
	fireInteraction.location.Z = localPosition.z();

	fireInteraction.range = pdu->range();

	fireInteraction.fuseType = pdu->fuseType();

	fireInteraction.quantity = pdu->quantity();

	fireInteraction.rate = pdu->rate();

	fireInteraction.warheadType = pdu->warheadType();

	_fireCallback(fireInteraction);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SetFireCallback(DtExerciseConn* exConn, FireCallback fireCallback)
{
	if (exConn)
	{
		_fireCallback = fireCallback;

		DtFireInteraction::addCallback(exConn, fireCb, NULL);
	}
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API EventID SendFireInteraction(DtExerciseConn* exConn, FireInteraction fireInteraction)
{
	DtFireInteraction myFire = DtFireInteraction();

	DtEntityIdentifier attackerEntityId = DtEntityIdentifier(fireInteraction.attacker.site, fireInteraction.attacker.host, fireInteraction.attacker.app);
	myFire.setAttackerId(attackerEntityId);

	DtEntityIdentifier targetEntityId = DtEntityIdentifier(fireInteraction.target.site, fireInteraction.target.host, fireInteraction.target.app);
	myFire.setTargetId(targetEntityId);

	DtEntityIdentifier munitionEntityId = DtEntityIdentifier(fireInteraction.munition.site, fireInteraction.munition.host, fireInteraction.munition.app);
	myFire.setMunitionId(munitionEntityId);

	myFire.setEventId(exConn->nextEventId());

	//virtual unsigned long fireMissionIndex() const;  //TODO:: implement
	//virtual void setFireMissionIndex(unsigned long i);

	DtVector32 linVelocity = DtVector32(fireInteraction.linVelocity.X, fireInteraction.linVelocity.Y, fireInteraction.linVelocity.Z);
	myFire.setVelocity(linVelocity);

	DtVector posGeoc = localToGeoc(DtVector(fireInteraction.location.X, fireInteraction.location.Y, fireInteraction.location.Z));
	myFire.setLocation(posGeoc);

	myFire.setRange(fireInteraction.range);

	myFire.setFuseType(fireInteraction.fuseType);

	myFire.setMunitionType(DtEntityType(fireInteraction.munitionType.entityKind,
										fireInteraction.munitionType.domain,
										fireInteraction.munitionType.country,
										fireInteraction.munitionType.category,
										fireInteraction.munitionType.subCategory,
										fireInteraction.munitionType.specific,
										fireInteraction.munitionType.extra));

	myFire.setQuantity(fireInteraction.quantity);
	myFire.setRate(fireInteraction.rate);
	myFire.setWarheadType(fireInteraction.warheadType);

	exConn->sendStamped(myFire);

	EventID eventId;
	eventId.site = myFire.eventId().site();
	eventId.host = myFire.eventId().host();
	eventId.eventNum = myFire.eventId().eventNum();
	return eventId;
}
// ======================================================================================================== //
static void detonationCb(DtDetonationInteraction* pdu, void* usr)
{
	DetonationInteraction detInteraction;

	detInteraction.attacker.site = pdu->attackerId().site();
	detInteraction.attacker.host = pdu->attackerId().host();
	detInteraction.attacker.app = pdu->attackerId().entityNum();

	detInteraction.munitionType.entityKind = pdu->munitionType().kind();
	detInteraction.munitionType.domain = pdu->munitionType().domain();
	detInteraction.munitionType.country = pdu->munitionType().country();
	detInteraction.munitionType.category = pdu->munitionType().category();
	detInteraction.munitionType.subCategory = pdu->munitionType().subCategory();
	detInteraction.munitionType.specific = pdu->munitionType().specific();
	detInteraction.munitionType.extra = pdu->munitionType().extra();

	detInteraction.target.site = pdu->targetId().site();
	detInteraction.target.host = pdu->targetId().host();
	detInteraction.target.app = pdu->targetId().entityNum();

	detInteraction.munition.site = pdu->munitionId().site();
	detInteraction.munition.host = pdu->munitionId().host();
	detInteraction.munition.app = pdu->munitionId().entityNum();

	detInteraction.eventId.site = pdu->eventId().site();
	detInteraction.eventId.host = pdu->eventId().host();
	detInteraction.eventId.eventNum = pdu->eventId().eventNum();

	DtVector linVelocity = DtVector32To64(pdu->velocity());
	detInteraction.linVelocity.X = linVelocity.x();
	detInteraction.linVelocity.Y = linVelocity.y();
	detInteraction.linVelocity.Z = linVelocity.z();

	DtVector detLocalPosition = geocToLocal(pdu->worldLocation());
	detInteraction.worldLocation.X = detLocalPosition.x();
	detInteraction.worldLocation.Y = detLocalPosition.y();
	detInteraction.worldLocation.Z = detLocalPosition.z();

	DtVector entLocalPosition = geocToLocal(DtVector32To64(pdu->entityLocation()));
	detInteraction.entityLocation.X = entLocalPosition.x();
	detInteraction.entityLocation.Y = entLocalPosition.y();
	detInteraction.entityLocation.Z = entLocalPosition.z();

	detInteraction.result = pdu->result();

	detInteraction.fuseType = pdu->fuseType();

	detInteraction.quantity = pdu->quantity();

	detInteraction.rate = pdu->rate();

	detInteraction.warheadType = pdu->warheadType();

	_detonationCallback(detInteraction);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SetDetonationCallback(DtExerciseConn* exConn, DetonationCallback detonationCallback)
{
	if (exConn)
	{
		_detonationCallback = detonationCallback;

		DtDetonationInteraction::addCallback(exConn, detonationCb, NULL);
	}
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SendDetonationInteraction(DtExerciseConn* exConn, DetonationInteraction detInteraction)
{
	DtDetonationInteraction myDetonation = DtDetonationInteraction();

	DtEntityIdentifier attackerEntityId = DtEntityIdentifier(detInteraction.attacker.site, detInteraction.attacker.host, detInteraction.attacker.app);
	myDetonation.setAttackerId(attackerEntityId);

	DtEntityIdentifier targetEntityId = DtEntityIdentifier(detInteraction.target.site, detInteraction.target.host, detInteraction.target.app);
	myDetonation.setTargetId(targetEntityId);

	DtEntityIdentifier munitionEntityId = DtEntityIdentifier(detInteraction.munition.site, detInteraction.munition.host, detInteraction.munition.app);
	myDetonation.setMunitionId(munitionEntityId);

	DtEventID eventId = DtEventID(detInteraction.eventId.site, detInteraction.eventId.host, detInteraction.eventId.eventNum);
	myDetonation.setEventId(eventId);

	DtVector32 linVelocity = DtVector32(detInteraction.linVelocity.X, detInteraction.linVelocity.Y, detInteraction.linVelocity.Z);
	myDetonation.setVelocity(linVelocity);

	DtVector worldPosGeoc = localToGeoc(DtVector(detInteraction.worldLocation.X, detInteraction.worldLocation.Y, detInteraction.worldLocation.Z));
	myDetonation.setWorldLocation(worldPosGeoc);

	DtVector32 entityPosGeoc = DtVector64To32(localToGeoc(DtVector(detInteraction.entityLocation.X, detInteraction.entityLocation.Y, detInteraction.entityLocation.Z)));
	myDetonation.setEntityLocation(entityPosGeoc);

	myDetonation.setResult(detInteraction.result);

	myDetonation.setFuseType(detInteraction.fuseType);

	myDetonation.setMunitionType(DtEntityType(detInteraction.munitionType.entityKind,
		detInteraction.munitionType.domain,
		detInteraction.munitionType.country,
		detInteraction.munitionType.category,
		detInteraction.munitionType.subCategory,
		detInteraction.munitionType.specific,
		detInteraction.munitionType.extra));

	myDetonation.setQuantity(detInteraction.quantity);
	myDetonation.setRate(detInteraction.rate);
	myDetonation.setWarheadType(detInteraction.warheadType);

	exConn->sendStamped(myDetonation);
}
// ======================================================================================================== //
static void createEntityCb(DtCreateEntityInteraction* pdu, void* usr)
{
	CreateEntityInteraction createEntityInteraction;

	createEntityInteraction.senderId.site = pdu->senderId().site();
	createEntityInteraction.senderId.host = pdu->senderId().host();
	createEntityInteraction.senderId.app = pdu->senderId().entityNum();

	createEntityInteraction.receiverId.site = pdu->receiverId().site();
	createEntityInteraction.receiverId.host = pdu->receiverId().host();
	createEntityInteraction.receiverId.app = pdu->receiverId().entityNum();

	createEntityInteraction.requestId = pdu->requestId();

	_createEntityCallback(createEntityInteraction);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SetCreateEntityCallback(DtExerciseConn* exConn, CreateEntityCallback createEntityCallback)
{
	if (exConn)
	{
		_createEntityCallback = createEntityCallback;

		DtCreateEntityInteraction::addCallback(exConn, createEntityCb, NULL);
	}
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SendCreateEntityInteraction(DtExerciseConn* exConn, CreateEntityInteraction createEntityInteraction)
{
	DtCreateEntityInteraction myCreateEntity = DtCreateEntityInteraction();

	DtEntityIdentifier senderEntityId = DtEntityIdentifier(createEntityInteraction.senderId.site, createEntityInteraction.senderId.host, createEntityInteraction.senderId.app);
	myCreateEntity.setSenderId(senderEntityId);

	DtEntityIdentifier receiverEntityId = DtEntityIdentifier(createEntityInteraction.receiverId.site, createEntityInteraction.receiverId.host, createEntityInteraction.receiverId.app);
	myCreateEntity.setReceiverId(receiverEntityId);

	myCreateEntity.setRequestId(createEntityInteraction.requestId);

	exConn->sendStamped(myCreateEntity);
}
// ======================================================================================================== //
static void removeEntityCb(DtRemoveEntityInteraction* pdu, void* usr)
{
	RemoveEntityInteraction removeEntityInteraction;

	removeEntityInteraction.senderId.site = pdu->senderId().site();
	removeEntityInteraction.senderId.host = pdu->senderId().host();
	removeEntityInteraction.senderId.app = pdu->senderId().entityNum();

	removeEntityInteraction.receiverId.site = pdu->receiverId().site();
	removeEntityInteraction.receiverId.host = pdu->receiverId().host();
	removeEntityInteraction.receiverId.app = pdu->receiverId().entityNum();

	removeEntityInteraction.requestId = pdu->requestId();

	_removeEntityCallback(removeEntityInteraction);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SetRemoveEntityCallback(DtExerciseConn* exConn, RemoveEntityCallback removeEntityCallback)
{
	if (exConn)
	{
		_removeEntityCallback = removeEntityCallback;

		DtRemoveEntityInteraction::addCallback(exConn, removeEntityCb, NULL);
	}
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SendRemoveEntityInteraction(DtExerciseConn* exConn, RemoveEntityInteraction removeEntityInteraction)
{
	DtRemoveEntityInteraction myRemoveEntity = DtRemoveEntityInteraction();

	DtEntityIdentifier senderEntityId = DtEntityIdentifier(removeEntityInteraction.senderId.site, removeEntityInteraction.senderId.host, removeEntityInteraction.senderId.app);
	myRemoveEntity.setSenderId(senderEntityId);

	DtEntityIdentifier receiverEntityId = DtEntityIdentifier(removeEntityInteraction.receiverId.site, removeEntityInteraction.receiverId.host, removeEntityInteraction.receiverId.app);
	myRemoveEntity.setReceiverId(receiverEntityId);

	myRemoveEntity.setRequestId(removeEntityInteraction.requestId);

	exConn->sendStamped(myRemoveEntity);
}
// ======================================================================================================== //
static void acknowledgeCb(DtAcknowledgeInteraction* pdu, void* usr)
{
	AcknowledgeInteraction acknowledgeInteraction;

	acknowledgeInteraction.senderId.site = pdu->senderId().site();
	acknowledgeInteraction.senderId.host = pdu->senderId().host();
	acknowledgeInteraction.senderId.app = pdu->senderId().entityNum();

	acknowledgeInteraction.receiverId.site = pdu->receiverId().site();
	acknowledgeInteraction.receiverId.host = pdu->receiverId().host();
	acknowledgeInteraction.receiverId.app = pdu->receiverId().entityNum();

	acknowledgeInteraction.requestId = pdu->requestId();

	acknowledgeInteraction.acknowledgeFlag = pdu->acknowledgeFlag();

	_acknowledgeCallback(acknowledgeInteraction);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SetAcknowledgeCallback(DtExerciseConn* exConn, AcknowledgeCallback acknowledgeInteraction)
{
	if (exConn)
	{
		_acknowledgeCallback = acknowledgeInteraction;

		DtAcknowledgeInteraction::addCallback(exConn, acknowledgeCb, NULL);
	}
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SendAcknowledgeInteraction(DtExerciseConn* exConn, AcknowledgeInteraction acknowledgeInteraction)
{
	DtAcknowledgeInteraction myAcknowledge = DtAcknowledgeInteraction();

	DtEntityIdentifier senderEntityId = DtEntityIdentifier(acknowledgeInteraction.senderId.site, acknowledgeInteraction.senderId.host, acknowledgeInteraction.senderId.app);
	myAcknowledge.setSenderId(senderEntityId);

	DtEntityIdentifier receiverEntityId = DtEntityIdentifier(acknowledgeInteraction.receiverId.site, acknowledgeInteraction.receiverId.host, acknowledgeInteraction.receiverId.app);
	myAcknowledge.setReceiverId(receiverEntityId);

	myAcknowledge.setRequestId(acknowledgeInteraction.requestId);

	myAcknowledge.setAcknowledgeFlag(acknowledgeInteraction.acknowledgeFlag);

	exConn->sendStamped(myAcknowledge);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void Destroy()
{
	//delete exConn;
	//exConn = NULL;
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API DtEntityPublisher* CreateEntityPublisher(DtExerciseConn* exConn, char* entityTypeString)
{
	//DtEntityType entityType(/*"1:2:225:2:4:1:0"*//*"1:1:235:27:0:1:1"*//*"3:1:105:1:1:32:0"*/);
	DtEntityType entityType(entityTypeString);
	DtEntityPublisher* entityPublisher = new DtEntityPublisher(entityType, exConn, DtDrDrmRvw, DtForceFriendly, DtEntityPublisher::guiseSameAsType());
	return entityPublisher;
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void DeleteEntityPublisher(DtEntityPublisher* entityPublisher)
{
	delete entityPublisher;
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SetRefLatLon(double lat, double lon)
{
	refLat = lat;
	refLon = lon;
}
// ======================================================================================================== //
DtVector localToGeoc(const DtVector& posLocal)
{
	const double theMultigenFlatEarthRadius = 6366707.02;

	double y = posLocal.z();
	double x = posLocal.x();
	double z = posLocal.y();

	double latGeod = (y * 180.0) / (theMultigenFlatEarthRadius * M_PI) + refLat;
	double lonGeod = (x * 180.0) / (theMultigenFlatEarthRadius * M_PI * cos(DtDeg2Rad(refLat))) + refLon;

	DtGeodeticCoord geodeticPosition(DtDeg2Rad(latGeod), DtDeg2Rad(lonGeod), z);
	DtVector geoc;
	geodeticPosition.getGeocentric(geoc);

	return geoc;
}
// ======================================================================================================== //
DtVector geocToLocal(const DtVector& geocLocal)
{
	const double theMultigenFlatEarthRadius = 6366707.02;

	DtGeodeticCoord geodeticPosition;

	geodeticPosition.setGeocentric(geocLocal);
	double lat = DtRad2Deg(geodeticPosition.lat());
	double lon = DtRad2Deg(geodeticPosition.lon());

	DtVector out;

	out.setZ(((lat - refLat) *
		(theMultigenFlatEarthRadius * M_PI)) / 180.0);

	out.setX(((lon - refLon) *
		(theMultigenFlatEarthRadius * M_PI * cos(DtDeg2Rad(refLat)))) / 180.0);

	out.setY(geodeticPosition.alt());

	return out;
}
// ======================================================================================================== //
DtVector localToGeod(const DtVector& posLocal)
{
	const double theMultigenFlatEarthRadius = 6366707.02;

	double y = posLocal.z();
	double x = posLocal.x();
	double z = posLocal.y();

	double latGeod = (y * 180.0) / (theMultigenFlatEarthRadius * M_PI) + refLat;
	double lonGeod = (x * 180.0) / (theMultigenFlatEarthRadius * M_PI * cos(DtDeg2Rad(refLat))) + refLon;

	DtGeodeticCoord geodeticPosition(DtDeg2Rad(latGeod), DtDeg2Rad(lonGeod), z);
	
	return DtVector(geodeticPosition.lat(), geodeticPosition.lon(), geodeticPosition.alt());
}
// ======================================================================================================== //
DtVector geodToLocal(const DtVector& geodLocal)
{//inefficient because it was converted from geoc to local, can't be to fix it if it breaks
	const double theMultigenFlatEarthRadius = 6366707.02;

	DtGeodeticCoord geodeticPosition(geodLocal.x(), geodLocal.y(), geodLocal.z());

	double lat = DtRad2Deg(geodeticPosition.lat());
	double lon = DtRad2Deg(geodeticPosition.lon());

	DtVector out;

	out.setZ(((lat - refLat) *
		(theMultigenFlatEarthRadius * M_PI)) / 180.0);

	out.setX(((lon - refLon) *
		(theMultigenFlatEarthRadius * M_PI * cos(DtDeg2Rad(refLat)))) / 180.0);

	out.setY(geodeticPosition.alt());

	return out;
}
// ======================================================================================================== //
DtVector utmToGeoc(const DtVector& utmLoc)
{
	/*DtDegMinSec latRef;
	latRef.deg = 1;
	latRef.min = 2;
	latRef.sec = 3;
	latRef.direction = DtNorth;
	DtDegMinSec lonRef;
	lonRef.deg = 1;
	lonRef.min = 2;
	lonRef.sec = 3;
	lonRef.direction = DtEast;

	DtUtmInit(latRef, lonRef, 1);

	DtVector geocLoc;
	DtUtmCoord locUtm(utmLoc.x(), utmLoc.y(), utmLoc.z());
	locUtm.getGeocentric(geocLoc);

	return geocLoc;*/
	/*DtDegMinSec latitudeRef = { 0.0, 0.0, 0.0, DtNorth };
	DtDegMinSec longitudeRef = { 33.0, 0.0, 0.0, DtEast };

	DtUtmReferencePoint Wgs84RefPoint(latitudeRef, longitudeRef, 1, 0, DtWGS84);

	DtUtmCoord utm(utmLoc.x(), utmLoc.y(), utmLoc.z(), &Wgs84RefPoint);*/

	DtDegMinSec lat = { 0, 0, 0, DtNorth };
	DtDegMinSec lon = { 33, 0, 0, DtEast };
	DtUtmInit(lat, lon);
	DtUtmCoord tmpUtm(utmLoc.x(), utmLoc.y(), utmLoc.z());

	return tmpUtm.geocentric();
}
// ======================================================================================================== //
DtVector geocToUtm(const DtVector& geocLoc)
{
	/*DtDegMinSec latRef;
	latRef.deg = 1;
	latRef.min = 2;
	latRef.sec = 3;
	latRef.direction = DtNorth;
	DtDegMinSec lonRef;
	lonRef.deg = 1;
	lonRef.min = 2;
	lonRef.sec = 3;
	lonRef.direction = DtEast;

	DtUtmInit(latRef, lonRef, 1);

	DtUtmCoord locUtm;
	locUtm = locUtm.setGeocentric(geocLoc);
	return locUtm;*/
	DtDegMinSec lat = { 0, 0, 0, DtNorth };
	DtDegMinSec lon = { 33, 0, 0, DtEast };
	DtUtmInit(lat, lon);
	DtUtmCoord utm;
	utm = utm.setGeocentric(DtVector(geocLoc.x(), geocLoc.y(), geocLoc.z()));

	return utm;
}
// ======================================================================================================== //
DtVector utmWgs84ToEd50(const DtVector& wgs84)
{
	DtDegMinSec latitudeRef = { 0.0, 0.0, 0.0, DtNorth };
	DtDegMinSec longitudeRef = { 33.0, 0.0, 0.0, DtEast };

	DtUtmReferencePoint Wgs84RefPoint(latitudeRef, longitudeRef, 1, 0, DtWGS84);
	DtUtmCoord utmWgs84(wgs84.x(), wgs84.y(), wgs84.z(), &Wgs84RefPoint);

	DtVector geoc(0, 0, 0);
	utmWgs84.getGeocentric(geoc);

	DtUtmReferencePoint Ed50RefPoint(latitudeRef, longitudeRef, 1, 0, DtED50);
	DtUtmCoord utmEd50(0, 0, 0, &Ed50RefPoint);

	utmEd50.setGeocentric(geoc);

	return DtVector(utmEd50.x(), utmEd50.y(), utmEd50.z());
}
// ======================================================================================================== //
DtVector utmEd50ToWgs84(const DtVector& ed50)
{
	DtDegMinSec latitudeRef = { 0.0, 0.0, 0.0, DtNorth };
	DtDegMinSec longitudeRef = { 33.0, 0.0, 0.0, DtEast };

	DtUtmReferencePoint Ed50RefPoint(latitudeRef, longitudeRef, 1, 0, DtED50);
	DtUtmCoord utmEd50(ed50.x(), ed50.y(), ed50.z(), &Ed50RefPoint);

	DtVector geoc(0, 0, 0);
	utmEd50.getGeocentric(geoc);

	DtUtmReferencePoint Wgs84RefPoint(latitudeRef, longitudeRef, 1, 0, DtWGS84);
	DtUtmCoord utmWgs84(0, 0, 0, &Wgs84RefPoint);

	utmWgs84.setGeocentric(geoc);

	return DtVector(utmWgs84.x(), utmWgs84.y(), utmWgs84.z());
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void EntityPublisherSetMarkingText(DtEntityPublisher* entityPublisher, char* markingText)
{
	entityPublisher->esr()->setMarkingText(markingText);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void EntityPublisherSetEntityId(DtEntityPublisher* entityPublisher, char* entityId)
{
	entityPublisher->esr()->setEntityId(DtEntityIdentifier(entityId));
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void EntityPublisherSetLifeformState(DtEntityPublisher* entityPublisher, DtLifeformState lifeformSate)
{
	entityPublisher->esr()->setLifeformState(lifeformSate);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void EntityPublisherSetForceId(DtEntityPublisher* entityPublisher, DtForceType forceType)
{
	entityPublisher->esr()->setForceId(forceType);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void EntityPublisherSetDamageState(DtEntityPublisher* entityPublisher, DtDamageState damageState)
{
	entityPublisher->esr()->setDamageState(damageState);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void EntityPublisherSetPrimaryWeaponState(DtEntityPublisher* entityPublisher, DtWeaponState weaponState)
{
	entityPublisher->esr()->setPrimaryWeaponState(weaponState);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void EntityPublisherSetArtPart(DtEntityPublisher* entityPublisher, unsigned int partType, int paramType, float value)
{
	DtEntityStateRepository* esr = entityPublisher->entityStateRep();
	DtArticulatedPartCollection* artPartCol = esr->artPartList();

	DtArticulatedPart& artPart = artPartCol->getPart(partType);

	if (&artPart != NULL)
		artPart.setParameter(paramType, DtDeg2Rad(value));
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void TickEntityPublisher(DtEntityPublisher* entityPublisher, double x, double y, double z, bool useTopo, double psi, double theta, double phi, double vx, double vy, double vz, int deadReckThreshold)
{
	if (useTopo)
	{
		DtTopoView view(entityPublisher->esr(), DtDeg2Rad(refLat), DtDeg2Rad(refLon));
		view.setLocation(DtVector(x, y, z));
		//view.setLocation(DtVector(1234.567, 5789.123, 3355.7458));
		view.setOrientation(DtTaitBryan(DtDeg2Rad(psi), DtDeg2Rad(theta), DtDeg2Rad(phi)));
		view.setVelocity(DtVector32(vx, vy, vz));
	}
	else
	{
		DtVector posGeoc = localToGeoc(DtVector(x, y, z));

		DtVector posAfterVelocityGeoc = localToGeoc(DtVector(x, y, z) + DtVector(vx, vy, vz));
		DtVector velocityGeoc = posAfterVelocityGeoc - posGeoc;

		entityPublisher->esr()->setLocation(posGeoc);
		entityPublisher->esr()->setVelocity(DtVector64To32(velocityGeoc));


		DtTopoView view(entityPublisher->esr(), DtDeg2Rad(refLat), DtDeg2Rad(refLon));
		view.setOrientation(DtTaitBryan(DtDeg2Rad(psi), DtDeg2Rad(theta), DtDeg2Rad(phi)));

		DtEntityType MyType = entityPublisher->esr()->entityType();
		DtEntityType NamerType = DtEntityType(1, 1, 105, 2, 6, 0, 1);
		DtEntityType TankType = DtEntityType(1, 1, 105, 1, 2, 5, 1);
		/********************************************
		In comment only for drone pack 2018 expriment.
		*********************************************
		if (MyType.DtTypeMatchPattern(NamerType) || MyType.DtTypeMatchPattern(TankType))
		*********************************************/
		{
			entityPublisher->esr()->useDeadReckoner();
			entityPublisher->setDfltRotationThreshold((entityPublisher->dfltRotationThreshold() / deadReckThreshold));
			entityPublisher->setDfltTranslationThreshold(0.3);
		}
	}

	entityPublisher->tick();
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API XYZ localToUtm(double x, double y, double z)
{
	DtVector result = geocToUtm(localToGeoc(DtVector(x, y, z)));
	XYZ xyz;
	xyz.X = result.x();
	xyz.Y = result.y();
	xyz.Z = result.z();
	return xyz;
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API XYZ utmToLocal(double x, double y, double z)
{
	DtVector result = geocToLocal(utmToGeoc(DtVector(x, y, z)));
	XYZ xyz;
	xyz.X = result.x();
	xyz.Y = result.y();
	xyz.Z = result.z();
	return xyz;
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API XYZ localToGeoc(double x, double y, double z)
{
	DtVector result = localToGeoc(DtVector(x, y, z));
	XYZ xyz;
	xyz.X = result.x();
	xyz.Y = result.y();
	xyz.Z = result.z();
	return xyz;
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API XYZ geocToLocal(double x, double y, double z)
{
	DtVector result = geocToLocal(DtVector(x, y, z));
	XYZ xyz;
	xyz.X = result.x();
	xyz.Y = result.y();
	xyz.Z = result.z();
	return xyz;
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API XYZ localToGeod(double x, double y, double z)
{
	DtVector result = localToGeod(DtVector(x, y, z));
	XYZ xyz;
	xyz.X = result.x();
	xyz.Y = result.y();
	xyz.Z = result.z();
	return xyz;
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API XYZ geodToLocal(double x, double y, double z)
{
	DtVector result = geodToLocal(DtVector(x, y, z));
	XYZ xyz;
	xyz.X = result.x();
	xyz.Y = result.y();
	xyz.Z = result.z();
	return xyz;
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API XYZ utmWgs84ToEd50(double x, double y, double z)
{
	DtVector result = utmWgs84ToEd50(DtVector(x, y, z));
	XYZ xyz;
	xyz.X = result.x();
	xyz.Y = result.y();
	xyz.Z = result.z();
	return xyz;
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API XYZ utmEd50ToWgs84(double x, double y, double z)
{
	DtVector result = utmEd50ToWgs84(DtVector(x, y, z));
	XYZ xyz;
	xyz.X = result.x();
	xyz.Y = result.y();
	xyz.Z = result.z();
	return xyz;
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SendImgShare(DtExerciseConn* const conn, const char* img, const char* senderID, const char* recieverID, const int frequency, const int requestCounter)
{
	const int DATUM_IMAGE_FREQUENCY = 420004;
	const int DATUM_IMAGE_ADDRESS = 420005;

	DtSetDataPdu pdu;
	pdu.setSenderId(DtEntityIdentifier(senderID));
	pdu.setReceiverId(DtEntityIdentifier(recieverID));
	pdu.setRequestId(requestCounter);

	// Create numer of fixed fileds
	pdu.setNumFixedFields(1);

	// PDU index values start at 1
	// assign enumaration to datum
	pdu.setDatumParam(DtFixed, 1, static_cast<DtDatumParam>(DATUM_IMAGE_FREQUENCY));
	pdu.setDatumValUnsigned32(static_cast<DtDatumParam>(DATUM_IMAGE_FREQUENCY), frequency);

	int       indexVarFields = 1;
	const int numVarFields = 1;

	pdu.setNumVarFields(numVarFields);

	// PDU index values start at 1
	// assign enumaration to datum
	pdu.setDatumParam(DtVar, indexVarFields, static_cast<DtDatumParam>(DATUM_IMAGE_ADDRESS));
	// Allocate space for data
	pdu.setVarDataBytes(indexVarFields, ((strlen(img) + 1)*sizeof(char)));
	// Set data
	pdu.setDatumValByteArray(DtVar, indexVarFields, img);

	conn->sendStamped(pdu);
}
// ======================================================================================================== //
VRLINK_DIS_DLL_API void SendCreateEntitySetData(DtExerciseConn* const conn, EntityId senderId, EntityId recieverId, int requestId, EntityType entityType, XYZ location, 
												DtForceType ForceType, double psi, double theta, double phi)
{
	DtSetDataInteraction sdPDU;
	sdPDU.setSenderId(DtEntityIdentifier(senderId.site, senderId.host, senderId.app));
	sdPDU.setRequestId(requestId);
	sdPDU.setReceiverId(DtEntityIdentifier(recieverId.site, recieverId.host, recieverId.app));
	sdPDU.setNumFixedFields(14);

	sdPDU.setDatumValInt32(DtFixed, 1, entityType.entityKind);
	sdPDU.setDatumParam(DtFixed, 1, DtDatumKind);
	sdPDU.setDatumValInt32(DtFixed, 2, entityType.domain);
	sdPDU.setDatumParam(DtFixed, 2, DtDatumDomain);
	sdPDU.setDatumValInt32(DtFixed, 3, entityType.country);
	sdPDU.setDatumParam(DtFixed, 3, DtDatumCountry);
	sdPDU.setDatumValInt32(DtFixed, 4, entityType.category);
	sdPDU.setDatumParam(DtFixed, 4, DtDatumCategory);
	sdPDU.setDatumValInt32(DtFixed, 5, entityType.subCategory);
	sdPDU.setDatumParam(DtFixed, 5, DtDatumSubCategory);
	sdPDU.setDatumValInt32(DtFixed, 6, entityType.specific);
	sdPDU.setDatumParam(DtFixed, 6, DtDatumSpecific);
	sdPDU.setDatumValInt32(DtFixed, 7, entityType.extra);
	sdPDU.setDatumParam(DtFixed, 7, DtDatumExtra);

	DtVector posGeoc = localToGeoc(DtVector(location.X, location.Y, location.Z));
	sdPDU.setDatumValFloat32(DtFixed, 8, posGeoc.x());
	sdPDU.setDatumParam(DtFixed, 8, DtDatumXpos);
	sdPDU.setDatumValFloat32(DtFixed, 9, posGeoc.y());
	sdPDU.setDatumParam(DtFixed, 9, DtDatumYpos);
	sdPDU.setDatumValFloat32(DtFixed, 10, posGeoc.z());
	sdPDU.setDatumParam(DtFixed, 10, DtDatumZpos);

	sdPDU.setDatumValInt32(DtFixed, 11, ForceType);
	sdPDU.setDatumParam(DtFixed, 11, DtDatumForceId);

	sdPDU.setDatumValFloat32(DtFixed, 12, psi);
	sdPDU.setDatumParam(DtFixed, 12, DtDatumPsi);
	sdPDU.setDatumValFloat32(DtFixed, 13, theta);
	sdPDU.setDatumParam(DtFixed, 13, DtDatumTheta);
	sdPDU.setDatumValFloat32(DtFixed, 14, phi);
	sdPDU.setDatumParam(DtFixed, 14, DtDatumPhi);

	conn->sendStamped(sdPDU);
}

VRLINK_DIS_DLL_API void SendComment(DtExerciseConn* const conn, EntityId senderId, EntityId recieverId, char* comment)
{
	DtCommentInteraction c;
	c.setSenderId(DtEntityIdentifier(senderId.site, senderId.host, senderId.app));
	c.setReceiverId(DtEntityIdentifier(recieverId.site, recieverId.host, recieverId.app));
	c.setComment(comment, strlen(comment));

	conn->sendStamped(c);
}