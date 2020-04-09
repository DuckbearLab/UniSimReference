using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * FollowTransform -
 * DESCRIPTION - Makes a transform lerp-follow another transform. 
 * =================================================================================== */

public class FollowTransform : MonoBehaviour
{
    [Tooltip("Leave null to use this transform. ")]
    public Transform Follower;
    public Transform Target;
    [Range(0.1f, 10)]
    public float PositionLerpAmount = 4f;
    public bool SlerpRotation;
    [Range(0.1f, 10)]
    public float RotationSlerpAmount = 4f;

    void LateUpdate()
    {
        Transform follower = Follower ?? transform;
        follower.transform.position = follower.transform.position.Lerp(Target.position, PositionLerpAmount * Time.deltaTime);
        if (SlerpRotation)
            follower.transform.rotation = follower.transform.rotation.Slerp(Target.rotation, RotationSlerpAmount * Time.deltaTime);
    }
}