using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenAudioEnds : MonoBehaviour {

    public AudioSource AudioSource;

	// Use this for initialization
	void Start() 
    {
        Destroy(gameObject, AudioSource.clip.length);
	}
}
