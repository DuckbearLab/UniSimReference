using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetStructs;

/* ===================================================================================
 * ReflectedFireEffects - 
 * This component should be placed on a gameObject that has a reflected entity.
 * Listens to fire interactions and plays the appropriate particle systems and audio.
=================================================================================== */

public class ReflectedFireEffects : MonoBehaviour 
{

    [System.Serializable]
    public struct MunitionTypeEffects
    {
        public string MunitionType;
        public ParticleSystem ParticleSystem;
        public AudioSource AudioSource;
    }

    public MunitionTypeEffects[] ReflectedEffects;

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

        for (int i = 0; i < ReflectedEffects.Length; i++)
        {
            MunitionTypeEffects effects = ReflectedEffects[i];

            if (EntityType.FromString(effects.MunitionType).MatchPattern(munition))
            {
                if (effects.ParticleSystem != null)
                {
                    effects.ParticleSystem.Stop();
                    effects.ParticleSystem.Play();
                }

                if(effects.AudioSource != null)
                {
                    effects.AudioSource.Stop();
                    effects.AudioSource.Play();
                }

                break;
            }
        }
    }
}
