using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using CppStructs;
using NetStructs;
using System;
using UnityEngine.AI;

public class EntityPublisher
{
    public bool publishLocation;
    public bool publishHeading;
    public bool publishPitch;
    public bool publishRoll;
    public bool publishVelocity;
    public int DeadReckThreshold; //for Drones

    private IntPtr exerciseConnPtr;
    private IntPtr entityPublisherPtr;

    private Transform toFollow;

    public EntityPublisher(IntPtr exerciseConnPtr, IntPtr entityPublisherPtr, 
        bool publishLocation, bool publishHeading, bool publishPitch,bool publishRoll, bool publishVelocity)
    {
        this.exerciseConnPtr     = exerciseConnPtr;
        this.entityPublisherPtr  = entityPublisherPtr;
        this.publishLocation     = publishLocation;
        this.publishHeading      = publishHeading;
        this.publishPitch        = publishPitch;
        this.publishRoll         = publishRoll;
        this.publishVelocity     = publishVelocity;

        NetSimAgent.Instance.EntityPublisherSetLifeformState(this.entityPublisherPtr, LifeformState.NA);
        NetSimAgent.Instance.EntityPublisherSetDamageState(this.entityPublisherPtr, DamageState.None);


        //NetSimAgent.Instance.EntityPublisherSetForceId(this.entityPublisherPtr, ForceType.DtForceFriendly);
    }

    public void Follow(Transform transform)
    {
        toFollow = transform;
    }

    public void SetMarkingText(string markingText)
    {
        NetSimAgent.Instance.EntityPublisherSetMarkingText(entityPublisherPtr, markingText);
    }

    public void SetEntityId(string entityId)
    {
        NetSimAgent.Instance.EntityPublisherSetEntityId(entityPublisherPtr, entityId);
    }

    public void SetForceId(ForceType forceType)
    {
        NetSimAgent.Instance.EntityPublisherSetForceId(entityPublisherPtr, forceType);
    }

    public void SetDamageState(DamageState damageState)
    {
        NetSimAgent.Instance.EntityPublisherSetDamageState(entityPublisherPtr, damageState);
    }

    public void SetPrimaryWeaponState(WeaponState weaponState)
    {
        NetSimAgent.Instance.EntityPublisherSetPrimaryWeaponState(entityPublisherPtr, weaponState);
    }

    public void SetLifeformState(LifeformState lifeformState)
    {
        NetSimAgent.Instance.EntityPublisherSetLifeformState(entityPublisherPtr, lifeformState);
    }

    public void SetArtPart(uint partType, int paramType, float value)
    {
        NetSimAgent.Instance.EntityPublisherSetArtPart(entityPublisherPtr, partType, paramType, value);
    }

    public void Tick()
    {
        //const double theMultigenFlatEarthRadius = 6366707.02;

        bool useTopo = false;
        double vx = 0, vy = 0, vz = 0;

        var infantryCharacterController = toFollow.GetComponent<Infantry.InfantryFirstPersonController>();
        var characterController = toFollow.GetComponent<CharacterController>();
        var rigidbody = toFollow.GetComponent<Rigidbody>();
        var navmeshagent = toFollow.GetComponent<NavMeshAgent>();

        if (infantryCharacterController)
        {
            if(!infantryCharacterController.m_IsImmobile)
            {
                vx = infantryCharacterController.Velocity.x;
                vy = infantryCharacterController.Velocity.y;
                vz = infantryCharacterController.Velocity.z;
            }
            else
            {
                vx = infantryCharacterController.ManuallySpeed.x;
                vy = infantryCharacterController.ManuallySpeed.y;
                vz = infantryCharacterController.ManuallySpeed.z;
            }
            //else if (rigidbody != null)
            //{
            //    vx = rigidbody.velocity.x;
            //    vy = rigidbody.velocity.y;
            //    vz = rigidbody.velocity.z;
            //}
        }
        else if (characterController != null)
        {
            vx = characterController.velocity.x;
            vy = characterController.velocity.y;
            vz = characterController.velocity.z;
        }
        else if (rigidbody != null)
        {
            vx = rigidbody.velocity.x;
            vy = rigidbody.velocity.y;
            vz = rigidbody.velocity.z;
        }
        else if (navmeshagent != null)
        {
            vx = navmeshagent.velocity.x;
            vy = navmeshagent.velocity.y;
            vz = navmeshagent.velocity.z;
        }

        vx = publishLocation ? vx : 0;
        vy = publishLocation ? vy : 0;
        vz = publishLocation ? vz : 0;

        float px, py, pz;
        px = publishLocation ? toFollow.position.x : 0;
        py = publishLocation ? toFollow.position.y : 0;
        pz = publishLocation ? toFollow.position.z : 0;

        float rx, ry, rz;
        rx = publishHeading ? toFollow.rotation.eulerAngles.y : 0;
        ry = publishPitch   ? -toFollow.rotation.eulerAngles.x : 0;
        rz = publishRoll    ? -toFollow.rotation.eulerAngles.z : 0;

        NetSimAgent.Instance.TickEntityPublisher(entityPublisherPtr,
        px,py,pz,
        useTopo,
        rx,ry,rz,
        vx, vy, vz, DeadReckThreshold);
    }

    public void Destroy()
    {
        NetSimAgent.Instance.DeleteEntityPublisher(exerciseConnPtr, entityPublisherPtr);
    }

}
