using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
/* ===================================================================================
* FollowTransformEditor -
* DESCRIPTION - A custom editor for the FollowTransform class. 
* Hides the last property (rotation lerp amount) if not using slerp rotation.
* =================================================================================== */
namespace Scripts.Editor
{
    [CustomEditor(typeof(FollowTransform))]
    public class FollowTransformEditor : Utils.Editor.DefaultEditorUnless
    {
        public override void HandleSpecialProperty(SerializedProperty property)
        {
            if ((target as FollowTransform).SlerpRotation)
                DrawDefault(property);
        }

        public override bool ShouldDrawDefault(SerializedProperty property, int propertyIndex)
        {
            return propertyIndex != properties.Length - 1;
        }
    }
}