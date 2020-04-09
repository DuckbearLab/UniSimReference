using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetStructs;

/* ===================================================================================
 * ImpactParticles -
 * DESCRIPTION -
 * =================================================================================== */

public class DetonationParticles : MonoBehaviour
{
    public enum DetonationResult
    {
        Any = -1,
        EntityImpact = NetStructs.DetonationResult.DtDetResEntityImpact,
        BuildingImpact, 
        GroundImpact = NetStructs.DetonationResult.DtDetResGroundImpact,
        AirHit = NetStructs.DetonationResult.DtDetResAirHit
    }

    [System.Serializable]
    public class ImpactType
    {
        public List<DetonationResult> Detonations;
        public ParticleSystem[] ParticlesToPlay;
    }

    public ImpactType[] Impacts;

    void Start()
    {
        foreach (ImpactType impact in Impacts)
        {
            foreach (ParticleSystem system in impact.ParticlesToPlay)
            {
                if (system.main.playOnAwake)
                {
                    Debug.LogError("The system " + system.name + "  Under " + name + " is set to play on an impact type but also to play on awake. ", gameObject);
                    Debug.DebugBreak();
                }
            }
        }
    }

    public void PlayImpact(NetStructs.DetonationResult InteractionResult)
    {
        DetonationResult res = Convert(InteractionResult);
        foreach (ImpactType impact in Impacts)
        {
            if (impact.Detonations.Contains(DetonationResult.Any) || impact.Detonations.Contains(res))
            {
                foreach (ParticleSystem system in impact.ParticlesToPlay)
                {
                    system.Play();
                }
                break;
            }
        }
    }

    private DetonationResult Convert(NetStructs.DetonationResult result)
    {
        switch (result)
        {
            case NetStructs.DetonationResult.DtDetResBuildingHitLarge:
            case NetStructs.DetonationResult.DtDetResBuildingHitMedium:
            case NetStructs.DetonationResult.DtDetResBuildingHitSmall:
            {
                return DetonationResult.BuildingImpact;
            }
            case NetStructs.DetonationResult.DtDetResAirBurst:
            case NetStructs.DetonationResult.DtDetResAirHit:
            {
                return DetonationResult.AirHit;
            }
            case NetStructs.DetonationResult.DtDetResDirtBlastLarge:
            case NetStructs.DetonationResult.DtDetResDirtBlastMedium:
            case NetStructs.DetonationResult.DtDetResDirtBlastSmall:
            case NetStructs.DetonationResult.DtDetResGroundImpact:
            case NetStructs.DetonationResult.DtDetResGroundProximate:
            {
                return DetonationResult.GroundImpact;
            }
            case NetStructs.DetonationResult.DtDetResEntityImpact:
            {
                return DetonationResult.EntityImpact;
            }
            default:
            {
                return (DetonationResult) (int.MinValue);
            }
        }
    }
}