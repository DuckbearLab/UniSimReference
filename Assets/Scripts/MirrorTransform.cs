using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * MirrorTransform -
 * DESCRIPTION - Mirrors another transform's position and rotation. 
 * =================================================================================== */

public class MirrorTransform : MonoBehaviour
{
    public Transform Target;

    void LateUpdate()
    {
        transform.SetPositionAndRotation(Target.position, Target.rotation);
    }
}