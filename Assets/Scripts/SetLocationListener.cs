using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventReports;
/* ===================================================================================
 * SetLocationListener -
 * DESCRIPTION - Listens to SetLocation message and applies location to transform. 
 * =================================================================================== */
namespace Utils
{
    public class SetLocationListener : MonoBehaviour
    {
        public EventReportsManager EventReportsManager;
        public PublishedEntity Entity;
        public Transform Target;
        void Start()
        {
            EventReportsManager.Subscribe<SetLocation>(Set);
        }

        private void Set(SetLocation msg)
        {
            if (msg.ReceiverId == Entity.MyEntityId)
                Target.position = CoordConverter.GeocToLocal(new CppStructs.XYZ(msg.X, msg.Y, msg.Z));
        }
    }
}