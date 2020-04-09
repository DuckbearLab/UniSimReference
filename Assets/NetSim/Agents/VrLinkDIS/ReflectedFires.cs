using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NetStructs;

public class ReflectedFires : MonoBehaviour
{
    public MunitionTypeReflectedModelPair[] ReflectedModels;
    public ExerciseConnection ExerciseConnection;
    public PublishedEntity PublishedEntity;
    private AudioListener AudioListener;
    private Flare tracerBullerFlare;

    private ReflectedEntities ReflectedEntities;

    private float OneOverSpeedOfSound;
    [System.Serializable]
    public struct MunitionTypeReflectedModelPair
    {
        public string Name;
        public string MunitionType;
        public GameObject ReflectedPrefab;
    }

    void Start()
    {
        ReflectedEntities = GetComponent<ReflectedEntities>();
        ExerciseConnection.SubscribeFireInteraction(CreateNewFire);
        //if there isn't a main camera in the start, wait a second and try again.
        if(Camera.main != null)
        {
            AudioListener = Camera.main.GetComponent<AudioListener>();
        }
		else
        {
            new Timer(1f, () => { AudioListener = Camera.main.GetComponent<AudioListener>(); });
		} 
        OneOverSpeedOfSound = 1f / 343f;
        tracerBullerFlare = Resources.Load<Flare>("Tracer Bullet Lens Flare/TracerBulletFlare");
    }

    public void CreateNewFire(FireInteraction fireInteraction)
    {
        string munitionType = fireInteraction.munitionType.ToString();

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

        GameObject currentFire = Instantiate(reflectedPrefab);
        currentFire.transform.position = new Vector3((float)fireInteraction.location.X, (float)fireInteraction.location.Y, (float)fireInteraction.location.Z);
        
		if (PublishedEntity != null && fireInteraction.attacker == PublishedEntity.MyEntityId)
            currentFire.GetComponent<AudioSource>().spatialBlend = 0.0f;

        //this is the reflected projectile model, 
        if (currentFire.transform.childCount > 0)
        {
            ReflectedProjectile refProjectile = currentFire.GetComponentInChildren<ReflectedProjectile>();
            if (null != refProjectile)
            {
                refProjectile.Init(ExerciseConnection, fireInteraction);
                refProjectile.transform.SetParent(null, true);
            }
        }

        AudioSource asrc = currentFire.GetComponent<AudioSource>();
        if (null != asrc && null != AudioListener)
        {
            asrc.Stop();
            asrc.playOnAwake = false;
            if (PublishedEntity != null && fireInteraction.attacker == PublishedEntity.MyEntityId)
            {
                asrc.spatialBlend = 0f;
                //asrc.volume = 1;
                //asrc.spread = 360;
                //asrc.minDistance = 10000;
                //asrc.transform.position += 0.001f * Vector3.one;
            }
            //a delay of the distance to the shot over the speed of sound will delay the shot as if the sound was travelling at the speed of sound
            asrc.PlayDelayed(Vector3.Distance(currentFire.transform.position, AudioListener.transform.position) * OneOverSpeedOfSound);
        }
    }
}