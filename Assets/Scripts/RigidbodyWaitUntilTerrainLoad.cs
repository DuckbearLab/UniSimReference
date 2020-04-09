using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * RigidbodyWaitUntilTerrainLoad -
 * DESCRIPTION - Makes a rigidbody kinematic until the terrain is loaded. 
 * =================================================================================== */

    [RequireComponent(typeof(Rigidbody))]
public class RigidbodyWaitUntilTerrainLoad : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        TerrainLoader.Instance.TerrainFullyLoaded += SetToPhysical;
    }

    /// <summary>
    /// Returns the rigidbody to normal, i.e. not kinematic. 
    /// </summary>
    private void SetToPhysical()
    {
        rb.isKinematic = false;
    }
}