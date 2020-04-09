using NetStructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectedEntity : MonoBehaviour {

    public EntityId EntityId;
    public string MarkingText;
    public NetStructs.EntityType EntityType;

    public LifeformState LifeformState;
    [SerializeField]
    private DamageState damageState;
    public DamageState DamageState
    {
        get
        {
            return damageState;
        }
        set
        {
            if(damageState != value)
            {
                damageState = value;
                if (OnSetDamageState != null)
                {
                    OnSetDamageState(value);
                }
            }
        }
    }


    public ForceType ForceType;
    public WeaponState PrimaryWeaponState;
    public System.Action<DamageState> OnSetDamageState;

    private CenterOfMass CenterOfMass;
    private bool triedToFindCenterOfMass;

    public Vector3 Position
    {
        get
        {
            if (!triedToFindCenterOfMass)
            {
                triedToFindCenterOfMass = true;
                if (null == CenterOfMass)
                    CenterOfMass = GetComponent<CenterOfMass>();
            }
            if (null != CenterOfMass)
                return CenterOfMass.CenterOfMassObject.position;
            else return transform.position;
        }
    }
}
