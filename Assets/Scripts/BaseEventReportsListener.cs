using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventReports;
using NetStructs;
using System;
/* ===================================================================================
* BaseEventReportsListener - overridable base class for future event report listeners
* DESCRIPTION - Listens to SetLocation, SetRestore and SetDestroy messages.
* =================================================================================== */
namespace Utils
{
    public class BaseEventReportsListener : MonoBehaviour
    {
        public PublishedEntity Entity;
        public System.Action OnDeath;
        public System.Action OnAmmoRestore;        
        public System.Action OnRestore;
        public System.Action<Vector3> OnSetPosition;
        protected virtual void Start()
        {
            EventReportsManager EventReportsManager = EventReportsManager.Instance;
            EventReportsManager.Subscribe<SetLocation>(SetLocationHandler);
            EventReportsManager.Subscribe<SetDestroy>(SetDestroyHandler);
            EventReportsManager.Subscribe<SetAmmunition>(SetAmmoHandler);

            EventReportsManager.Subscribe<SetRestore>(SetRestoreHandler);
            EventReportsManager.Subscribe<DamageResultLite>(DamageResultLiteHandler);
        }

        protected virtual void SetAmmoHandler(SetAmmunition msg)
        {
            if (msg.ReceiverId == Entity.MyEntityId)
            {
                if (OnAmmoRestore != null)
                    OnAmmoRestore();
            }
        }

        protected virtual void SetDestroyHandler(SetDestroy msg)
        {
            if (msg.ReceiverId == Entity.MyEntityId)
            {
                Entity.SetDamageState(DamageState.Destroyed);
                if (OnDeath != null)
                    OnDeath();
            }
        }

        protected virtual void DamageResultLiteHandler(DamageResultLite msg)
        {
            if (msg.ReceiverId == Entity.MyEntityId && msg.DamageResult == DamageResultLite.DamageResultEnum.Destroyed)
            {
                Entity.SetDamageState(DamageState.Destroyed);
                if (OnDeath != null)
                    OnDeath();
            }
        }

        protected virtual void SetRestoreHandler(SetRestore msg)
        {
            if (msg.ReceiverId == Entity.MyEntityId)
            {
                Entity.SetDamageState(DamageState.None);
                if (OnRestore != null)
                    OnRestore();
            }
        }

        protected virtual void SetLocationHandler(SetLocation msg)
        {
            if (msg.ReceiverId == Entity.MyEntityId)
            {
                if (OnSetPosition != null)
                    OnSetPosition(CoordConverter.GeocToLocal(new CppStructs.XYZ(msg.X, msg.Y, msg.Z)));
            }
        }
    }
}