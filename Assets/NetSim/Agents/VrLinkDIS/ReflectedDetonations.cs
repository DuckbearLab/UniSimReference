using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NetStructs;

public class ReflectedDetonations : MonoBehaviour
{
    public ReflectedEntities ReflectedEntities;
    public MunitionTypeReflectedModelPair[] ReflectedModels;
    public ExerciseConnection ExerciseConnection;

    [System.Serializable]
    public struct MunitionTypeReflectedModelPair
    {
        public string Name;
        public string MunitionType;
        public GameObject ReflectedPrefab;
    }

    void Start()
    {
        ExerciseConnection.SubscribeDetonationInteraction(CreateNewDetonation);
        for (int i = 0; i < ReflectedModels.Length; i++)
        {
            ReflectedModels[i] = new MunitionTypeReflectedModelPair
            {
                MunitionType = ReflectedModels[i].MunitionType.Trim(),
                Name = ReflectedModels[i].Name,
                ReflectedPrefab = ReflectedModels[i].ReflectedPrefab
            };
        }
        if (null == ReflectedEntities)
            ReflectedEntities = FindObjectOfType<ReflectedEntities>();
    }

    public void CreateNewDetonation(DetonationInteraction detInteraction)
    {
        string munitionType = detInteraction.munitionType.ToString();

        GameObject reflectedPrefab = null;

        foreach (var reflectedModelPair in ReflectedModels)
        {
            if (reflectedModelPair.MunitionType == munitionType)
            {
                reflectedPrefab = reflectedModelPair.ReflectedPrefab;
                break;
            }
        }

        if (reflectedPrefab == null)
            return;

        GameObject currentDetonation = Instantiate(reflectedPrefab);

        RotateBulletHole bulletHole = currentDetonation.GetComponentInChildren<RotateBulletHole>();
        if (null != bulletHole)
        {
            ReflectedEntity attacker = ReflectedEntities.GetEntity(detInteraction.attacker);
            if (null != attacker)
            {
                bulletHole.AttackerPosition = attacker.transform.position;
            }
            else
            {
                foreach (PublishedEntity ent in  ExerciseConnection.LocalPublishedEntities)
                {
                    if (detInteraction.attacker == ent.MyEntityId)
                    {
                        bulletHole.AttackerPosition = ent.transform.position;
                    }
                }
            }
        }
        DetonationParticles detParticles = currentDetonation.GetComponent<DetonationParticles>();
        if (null != detParticles)
        {
            detParticles.PlayImpact(detInteraction.result);
        }
        Rigidbody detonationRb = currentDetonation.GetComponent<Rigidbody>();
        if (detonationRb != null)
        {
            detonationRb.velocity= detInteraction.linVelocity.ToVector3();
            //check if detonation has collider to elevate the detonation above the ground
            Collider detonationCollider = currentDetonation.GetComponent<Collider>();
            //collider.bounds.size.y gives the height of the collider. 
            //if detonation has a rigidbody but no collider, default to 15cm above ground
            float y = null != detonationCollider ? detonationCollider.bounds.size.y + 0.02f : 0.15f;
            currentDetonation.transform.position = detInteraction.worldLocation.ToVector3() + new Vector3(0, y, 0);
        }
        else
        {
            currentDetonation.transform.position= detInteraction.worldLocation.ToVector3();
        }
    }
}
