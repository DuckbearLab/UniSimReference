using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * CompassRose -
 * DESCRIPTION - Rotates an image to match a Target transform's heading while keeping the letters
 * right side up. 
 * =================================================================================== */
namespace Simulation
{
    public class CompassRose : MonoBehaviour
    {
        public enum MyForward
        {
            Up, Forward, Right, Left, Down, Back
        }

        public MyForward TargetForward;

        public Transform Target;
        public RectTransform[] Letters;

        void Update()
        {
            //rotates the image on the z axis by the amount of degrees between the target's forward axis and the north axis. 
            //- for clockwise degrees
            transform.rotation = Quaternion.AngleAxis(-Quaternion.FromToRotation(Direction(), Vector3.forward).eulerAngles.y, Vector3.forward);
            foreach (RectTransform letter in Letters)
            {
                letter.rotation = Quaternion.identity;
            }
        }

        private Vector3 Direction()
        {
            switch (TargetForward)
            {
                case MyForward.Back:
                {
                    return -Target.forward;
                }
                case MyForward.Down:
                {
                    return -Target.up;
                }
                default:
                case MyForward.Forward:
                {
                    return Target.forward;
                }
                case MyForward.Left:
                {
                    return -Target.right;
                }
                case MyForward.Right:
                {
                    return Target.right;
                }
                case MyForward.Up:
                {
                    return Target.up;
                }
            }
        }
    }
}