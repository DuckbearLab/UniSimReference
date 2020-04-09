using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
/* ===================================================================================
 * FogTesting -
 * DESCRIPTION -
 * =================================================================================== */
public class FogTesting : MonoBehaviour
{
    [SerializeField] private MouseLook m;
    private Transform cam;
    void Start()
    {
        CursorLockManager.LockedIsDefault = true;
        cam = transform.GetChild(0);
    }

    void Update()
    {
        if (!CursorLockManager.IsInUse)
            m.RotateCamera(transform, cam.transform, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        transform.position += GetMovement();
    }

    Vector3 GetMovement()
    {
        Vector3 result = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            result += cam.forward;
        else if (Input.GetKey(KeyCode.S))
            result -= cam.forward;
        if (Input.GetKey(KeyCode.D))
            result += cam.right;
        else if (Input.GetKey(KeyCode.A))
            result -= cam.right;
        if (Input.GetKey(KeyCode.Q))
            result += cam.up;
        else if (Input.GetKey(KeyCode.Z))
            result -= cam.up;
        float speed = Input.GetKey(KeyCode.LeftControl) ? 6 : (Input.GetKey(KeyCode.LeftShift) ? 50 : 25);
        return result.normalized * (speed * Time.deltaTime);
    }
}