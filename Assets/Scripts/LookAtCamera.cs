using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera main;

    void Update()
    {
        if (null == main)
            main = Camera.main;
        if(main != null)
        {
            Vector3 target = main.transform.position;
            target.y = transform.position.y;
            transform.LookAt(target, Vector3.up);
        }
	}

}
