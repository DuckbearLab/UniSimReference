using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * ReverseParentScale -
 * DESCRIPTION - Sets the local scale to one over the parent's scale, reversing its effects.
 * =================================================================================== */
 [ExecuteInEditMode]
public class ReverseParentScale : MonoBehaviour
{
    public Transform Parent;

    private Vector3 previousScale;

    void Update()
    {
        if (!Application.isPlaying && null == Parent) //edit mode does not warn about null.
            return;
        if (Parent.localScale != previousScale)
        {
            previousScale = Parent.localScale;
            transform.localScale = new Vector3(1 / previousScale.x, 1 / previousScale.y, 1 / previousScale.z);
        }
    }
}