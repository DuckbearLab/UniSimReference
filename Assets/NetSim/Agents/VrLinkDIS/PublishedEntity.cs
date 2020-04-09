using UnityEngine;
using System.Collections;
using NetStructs;

public class PublishedEntity : MonoBehaviour
{
    public ExerciseConnection ExerciseConnection;
    public ReflectedEntities ReflectedEntities;
    public string MarkingText;
    public string PlayerEntityType;
    public EntityId MyEntityId;
    public ForceType ForceType;
    public DamageState DamageState;
    public LifeformState LifeformState { get; private set; }
    public WeaponState PrimaryWeaponState { get; private set; }
    public bool IgnoreSelf = true;

    public bool publishLocation = true;
    public bool publishHeading  = true;
    public bool publishPitch    = true;
    public bool publishRoll     = true;
    public bool publishVelocity = true;

    public EntityPublisher entityPublisher { get; private set; }

    public System.Action<DamageState> DamageStateChanged;

    public void Awake()
    {
  
    }

    // Use this for initialization
    void Start()
    {
        if (MyEntityId.IsNullId)
            MyEntityId = new EntityId(1, Random.Range(0, 9999), (ushort)Random.Range(0, 255));

        if (ReflectedEntities == null)
        {
            ReflectedEntities = FindObjectOfType<ReflectedEntities>();
        }
        if (IgnoreSelf)
            ReflectedEntities.AddToIgnore(MyEntityId);
        if (ExerciseConnection == null)
        {
            ExerciseConnection = FindObjectOfType<ExerciseConnection>();
        }
        entityPublisher = ExerciseConnection.CreateEntityPublisher(
            this, PlayerEntityType,
            publishLocation, publishHeading, publishPitch, publishRoll, publishVelocity);

        SetPrimaryWeaponState(WeaponState.WeaponDeployed);
    }

    void LateUpdate()
    {
        entityPublisher.SetEntityId((string)MyEntityId);
        entityPublisher.SetForceId(ForceType);
        entityPublisher.SetDamageState(DamageState);
        MarkingText = string.IsNullOrEmpty(MarkingText) ? SystemInfo.deviceName : MarkingText;
        entityPublisher.SetMarkingText(MarkingText);
        entityPublisher.Follow(transform);
        entityPublisher.Tick();
    }

    void OnDestroy()
    {
        if (null == ExerciseConnection)
            ExerciseConnection = FindObjectOfType<ExerciseConnection>();
        if (null != ExerciseConnection)
            ExerciseConnection.RemovePublishedEntity(this);
        if(entityPublisher != null)
            entityPublisher.Destroy();
    }

    public void SetLifeformState(LifeformState lifeformState)
    {
        LifeformState = lifeformState;
        entityPublisher.SetLifeformState(lifeformState);

    }

    public void SetDamageState(DamageState damageState)
    {
        //entityPublisher.SetDamageState(damageState);
        DamageState = damageState;
        if (DamageStateChanged != null)
        {
            DamageStateChanged(damageState);
        }
    }

    public void SetPrimaryWeaponState(WeaponState weaponState)
    {
        PrimaryWeaponState = weaponState;
        entityPublisher.SetPrimaryWeaponState(weaponState);
    }

    public void SetArtPart(uint partType, int paramType, float value)
    {
        entityPublisher.SetArtPart(partType, paramType, value);
    }
}
