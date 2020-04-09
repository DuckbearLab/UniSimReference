using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetStructs;

/* ===================================================================================
 * ReflectedFireParticles - 
 * This component should be placed on a gameObject that has a reflected entity.
 * Listens to fire interactions and plays the appropriate particle systems.
=================================================================================== */

public class ReflectedFireParticles : MonoBehaviour
{
    [System.Serializable]
    public struct MunitionTypeParticlePair
    {
        public string MunitionType;
        public ParticleSystem ParticleSystem;
    }

    public MunitionTypeParticlePair[] ReflectedParticles;

    private ExerciseConnection exerciseConnection;
    private ReflectedEntity reflectedEntity;

    private EntityId entityId;

    private void Start()
    {
        exerciseConnection = FindObjectOfType<ExerciseConnection>();
        reflectedEntity = GetComponent<ReflectedEntity>();

        if (exerciseConnection == null || reflectedEntity == null)
            Destroy(this);

        entityId = reflectedEntity.EntityId;
        exerciseConnection.SubscribeFireInteraction(FireInteractionHandler);
    }

    private void OnDestroy()
    {
        if (exerciseConnection != null)
            exerciseConnection.UnSubscribeFireInteraction(FireInteractionHandler);
    }

    private void FireInteractionHandler(FireInteraction fireInteraction)
    {
        if (!fireInteraction.attacker.Equals(entityId))
            return;

        EntityType munition = fireInteraction.munitionType;

        for (int i = 0; i < ReflectedParticles.Length; i++)
        {
            MunitionTypeParticlePair pair = ReflectedParticles[i];

            if (EntityType.FromString(pair.MunitionType).MatchPattern(munition))
            {
                if (pair.ParticleSystem != null)
                {
                    pair.ParticleSystem.Stop();
                    pair.ParticleSystem.Play();
                }

                break;
            }
        }
    }
}