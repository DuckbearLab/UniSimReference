using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetStructs;

/* ===================================================================================
 * ReflectedProjectile -
 * DESCRIPTION - For ballistic fires with a slow enough projectile, add a mesh GameObject 
 * with this component as a child of the main reflected fire. 
 * =================================================================================== */
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class ReflectedProjectile : MonoBehaviour
{
    [Tooltip("Whether to show the GameObject in the attacker's local scene.")]
    public bool ShowOnLocal;
    private new Rigidbody rigidbody;

    public void Init(ExerciseConnection ExerciseConnection, FireInteraction Fire)
    {
        if (!ShowOnLocal)
        {
            foreach (PublishedEntity entity in ExerciseConnection.LocalPublishedEntities)
            {
                if (entity.MyEntityId == Fire.attacker)
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = Fire.linVelocity;
        Destroy(gameObject, 12);
    }

    void OnCollisionEnter()
    {
        Destroy(gameObject);
    }
}