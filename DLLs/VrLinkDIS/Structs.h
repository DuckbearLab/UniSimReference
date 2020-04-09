
enum DtLifeformState;

struct EntityId
{
	int site;
	int host;
	unsigned short app;
};

struct EventID
{
	int site;
	int host;
	int eventNum;
};

struct EntityType
{
	int entityKind;
	int domain;
	int country;
	int category;
	int subCategory;
	int specific;
	int extra;
};

struct EntityState
{
	DtReflectedEntity* reflectedEntityPtr;

	EntityId entityId;
	EntityType entityType;

	double posX;
	double posY;
	double posZ;

	double velX;
	double velY;
	double velZ;

	double rotX;
	double rotY;
	double rotZ;

	DtLifeformState lifeformState;
	DtDamageState damageState;
	DtForceType forceType;
	DtWeaponState primaryWeaponState;
};

struct EntityStateArtPart
{
	EntityId entityId;
	unsigned int partId;

	double posX;
	double posY;
	double posZ;

	double rotX;
	double rotY;
	double rotZ;
};

struct XYZ
{
	double X;
	double Y;
	double Z;
};

struct FireInteraction
{
	EntityId attacker;
	EntityType munitionType;
	EntityId target;
	EntityId munition;
	EventID eventId;
	XYZ linVelocity;
	XYZ location; //local pos
	double range;

	DtDetonatorFuze fuseType;
	int quantity;
	int rate;
	DtWarheadType warheadType;
	//DtBurstDescriptor burst;
};

struct DetonationInteraction
{
	EntityId attacker;
	EntityType munitionType;
	EntityId target;
	EntityId munition;
	EventID eventId;
	XYZ linVelocity;
	XYZ worldLocation; //local pos
	XYZ entityLocation; //local pos

	DtDetonatorFuze fuseType;
	int quantity;
	int rate;
	DtWarheadType warheadType;

	//DtBurstDescriptor burst;
	DtDetonationResult result;
};

struct CreateEntityInteraction
{
	EntityId senderId;
	EntityId receiverId;
	int requestId;
};

struct RemoveEntityInteraction
{
	EntityId senderId;
	EntityId receiverId;
	int requestId;
};

struct AcknowledgeInteraction
{
	EntityId senderId;
	EntityId receiverId;
	DtAcknowledgeFlag acknowledgeFlag;
	int requestId;
};
