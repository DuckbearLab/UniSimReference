using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetStructs;

public class PublishedEntityArtPart : MonoBehaviour
{
    public uint PartType;

    public bool PublishHeading = true;
    public bool PublishPitch = true;
    public bool PublishRoll = true;
    public bool UseLocalRotation = false;

    public Vector3 RotationOffset = Vector3.zero;

    public Transform[] IgnoredParentRotations; 

    private PublishedEntity publishedEntity;

    private void Start()
    {
        publishedEntity = GetComponentInParent<PublishedEntity>();

        if (publishedEntity == null)
            Destroy(this);
    }

    private void LateUpdate()
    {
        Vector3 rotation;

        if(!UseLocalRotation)
             rotation = transform.eulerAngles;
        else
             rotation = transform.localEulerAngles;

        for (int i = 0; i < IgnoredParentRotations.Length; i++)
        {
            rotation -= IgnoredParentRotations[i].eulerAngles;
        }

        rotation += RotationOffset;

        if (PublishHeading)
            publishedEntity.SetArtPart(PartType, 11, rotation.y);

        if (PublishPitch)
            publishedEntity.SetArtPart(PartType, 13, rotation.x);

        if (PublishRoll)
            publishedEntity.SetArtPart(PartType, 15, rotation.z);
    }
}