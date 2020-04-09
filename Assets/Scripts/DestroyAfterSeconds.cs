using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour {
    
    public float SecondsUntilDestroy;
    
	void Start ()
    {
        Destroy(gameObject, SecondsUntilDestroy);
	}
}
