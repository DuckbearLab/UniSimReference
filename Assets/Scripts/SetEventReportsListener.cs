using EventReports;
using NetStructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infantry;
/* ===================================================================================
 * SetEventReportsListener - 
 * DESCRIPTION - event report listener for infantry
 * =================================================================================== */
public class SetEventReportsListener : MonoBehaviour
{

    public PublishedEntity PublishedEntity;
    public EventReportsManager EventReportsManager;
    public WalkingModel WalkingModel;

    // Use this for initialization
    void Start()
    {
        EventReportsManager.Subscribe<SetLocation>(SetLocationCallback);
        EventReportsManager.Subscribe<SetDestroy>(SetDestroyCallback);
        EventReportsManager.Subscribe<DamageResultLite>(SetDamageResultLiteCallback);
        EventReportsManager.Subscribe<SetRestore>(SetRestoreCallback);
        EventReportsManager.Subscribe<SetPosture>(SetPostureCallback);
        //delete and put a reference in the inspector if this script is only used for the infantry.
        WalkingModel = PublishedEntity.GetComponent<WalkingModel>();

    }

    virtual protected void SetLocationCallback(SetLocation sl)
    {
        if (sl.EntityId.Equals(PublishedEntity.MyEntityId))
        {
            Vector3 geocLoc = new Vector3((float)sl.X, (float)sl.Y, (float)sl.Z);
            Vector3 localLoc = CoordConverter.GeocToLocal(geocLoc);
            //localLoc.y += 10;

            transform.position = localLoc;
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().velocity = Vector3.zero;

            if (GetComponent<InfantryFirstPersonController>())
                GetComponent<InfantryFirstPersonController>().StopMoving();
        }
    }

    virtual protected void SetDestroyCallback(SetDestroy sd)
    {
        if (sd.EntityId == PublishedEntity.MyEntityId)
        {
            if (WalkingModel != null)
            {
                WalkingModel.InfantryLife.Die();
            }
            else
            {
                LogWriter.LogLine("Die");
                PublishedEntity.SetDamageState(DamageState.Destroyed);
            }

        }
    }

    virtual protected void SetDamageResultLiteCallback(DamageResultLite sd)
    {
        Debug.Log("DamageResult = " + sd.DamageResult);

        if (sd.TargetId == PublishedEntity.MyEntityId && sd.DamageResult != DamageResultLite.DamageResultEnum.None)
            WalkingModel.InfantryLife.GotHit(sd);

        //if (sd.TargetId.Equals(id))
        //{
        //    //if (BodyDamgeUI)
        //    //    BodyDamgeUI.GotHit(sd.DamageResult);

        //    if (sd.DamageResult == DamageResultLite.DamageResultEnum.MovementDisabled)
        //        PublishedEntity.GetComponent<WalkingModel>().InfantryLife.Die();
        //    else if (sd.DamageResult == DamageResultLite.DamageResultEnum.WeaponDisabled)
        //        PublishedEntity.SetDamageState(DamageState.Moderate);
        //}
        //if (sd.TargetId.Equals(id) && sd.DamageResult == DamageResultLite.DamageResultEnum.Destroyed)
        //{
        //    PublishedEntity.GetComponent<WalkingModel>().InfantryLife.Die();
        //}
    }

    virtual protected void SetRestoreCallback(SetRestore sr)
    {
        if (sr.EntityId == PublishedEntity.MyEntityId)
        {
            if (WalkingModel != null)
                WalkingModel.InfantryLife.Revive();
        }
    }

    /// <summary>
    /// set posture callback
    /// </summary>
    /// <param name="sps"></param>
    virtual protected void SetPostureCallback(SetPosture sps)
    {
        string markingText = PublishedEntity.MarkingText;
        if (sps.markingText.Equals(markingText))
        {
            if (WalkingModel.InfantryLife.IsAlive)
            {
                if (WalkingModel.InfantryLife.canChangePosture)
                {
                    switch (sps.posture)
                    {
                        case SetPosture.Posture.Standing:
                            {
                                WalkingModel.SetPosture(WalkingModel.Posture.Standing);
                                break;
                            }
                        case SetPosture.Posture.Kneeling:
                            {
                                WalkingModel.SetPosture(WalkingModel.Posture.Kneeling);
                                break;
                            }
                        case SetPosture.Posture.Prone:
                            {
                                WalkingModel.SetPosture(WalkingModel.Posture.Prone);
                                break;
                            }
                        default :
                            break;
                    }
                }
            }
            else
            {
                PublishedEntity.SetLifeformState(sps.ConvertedPosture);
            }
        }
    }
}
