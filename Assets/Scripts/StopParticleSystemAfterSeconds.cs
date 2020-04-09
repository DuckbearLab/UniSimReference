using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * StopParticleSystemAfterSeconds -
 * DESCRIPTION - Spots a particle system after a set amount of seconds. 
 * =================================================================================== */

[RequireComponent(typeof(ParticleSystem))]
public class StopParticleSystemAfterSeconds : MonoBehaviour
{
    public float Seconds;

    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time > startTime + Seconds)
        {
            GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            AudioSource ausrc = GetComponent<AudioSource>();
            if (null != ausrc)
                ausrc.Stop();
            Destroy(this);
        }
    }
}