using NetStructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Infantry.Devices
{
    public class DataQueryListener : MonoBehaviour
    {
        public DataQueryInteractionManager DataQueryInteractionManager;
        public DataInteractionManager DataInteractionManager;

        public Camera Camera;
        public PublishedEntity PublishedEntity;

        public InfantryDevices InfantryDevices;

        // Use this for initialization
        void Start()
        {
            DataQueryInteractionManager.SubscribeDataQueryInteraction(DataQueryInteractionCallBack);
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    DataQueryInteraction dataQueryInteraction = new DataQueryInteraction();
            //    dataQueryInteraction.SenderId = EntityId.FromString(PublishedEntity.MyEntityId);
            //    dataQueryInteraction.ReceiverId = EntityId.FromString(PublishedEntity.MyEntityId);
            //    dataQueryInteraction.RequestId = 19;

            //    dataQueryInteraction.initDatumIds(4, 1);

            //    dataQueryInteraction.SetFixedDatumId(1, (int)NetStructs.DatumId.DATUM_STATUS_ORIENTATION_HEADING_FLOAT);
            //    dataQueryInteraction.SetFixedDatumId(2, (int)NetStructs.DatumId.DATUM_STATUS_ORIENTATION_PITCH_FLOAT);
            //    dataQueryInteraction.SetFixedDatumId(3, (int)NetStructs.DatumId.DATUM_STATUS_FOV_HORIZONTAL);
            //    dataQueryInteraction.SetFixedDatumId(4, (int)NetStructs.DatumId.DATUM_STATUS_FOV_VERTICAL);

            //    dataQueryInteraction.SetVarDatumId(1, (int)NetStructs.DatumId.DATUM_STATUS_DEVICE);

            //    DataQueryInteractionManager.Send(dataQueryInteraction);
            //}
        }

        void DataQueryInteractionCallBack(DataQueryInteraction dataQueryInteraction)
        {
            if (!dataQueryInteraction.ReceiverId.Equals(PublishedEntity.MyEntityId))
                return;

            int numFixedFieldsRequested = dataQueryInteraction.NumFixedFields;
            int numVarFieldsRequested = dataQueryInteraction.NumVarFields;

            if ((numFixedFieldsRequested + numVarFieldsRequested) <= 0)
                return;

            DataInteraction dataInteraction = new DataInteraction();

            for (int i = 1; i <= numFixedFieldsRequested; ++i)
            {
                int datumId = dataQueryInteraction.FixedDatumId(i);

                switch (datumId)
                {
                    case (int)NetStructs.DatumId.DATUM_STATUS_ORIENTATION_HEADING_FLOAT:
                    {
                        dataInteraction.AddDataInteractionFixedFloat(Camera.transform.rotation.eulerAngles.y, (uint)datumId);
                        break;
                    }
                    case (int)NetStructs.DatumId.DATUM_STATUS_ORIENTATION_PITCH_FLOAT:
                    {
                        float pitch = Camera.transform.rotation.eulerAngles.x;
                        if (pitch > 180)
                            pitch -= 360;
                        pitch *= -1;

                        dataInteraction.AddDataInteractionFixedFloat(pitch, (uint)datumId);
                        break;
                    }
                    case (int)NetStructs.DatumId.DATUM_STATUS_FOV_HORIZONTAL:
                    {
                        float hfov = Mathf.Rad2Deg * (2 * Mathf.Atan(Mathf.Tan((Camera.fieldOfView / 2) * Mathf.Deg2Rad) * Camera.aspect)); //google

                        dataInteraction.AddDataInteractionFixedFloat(hfov, (uint)datumId);
                        break;
                    }
                    case (int)NetStructs.DatumId.DATUM_STATUS_FOV_VERTICAL:
                    {
                        float vfov = Camera.fieldOfView;

                        dataInteraction.AddDataInteractionFixedFloat(vfov, (uint)datumId);
                        break;
                    }
                    default:
                    break;
                }
            }

            for (int i = 1; i <= numVarFieldsRequested; ++i)
            {
                int datumId = dataQueryInteraction.VarDatumId(i);

                switch (datumId)
                {
                    case (int)NetStructs.DatumId.DATUM_STATUS_DEVICE:
                    {
                        string weaponName = InfantryDevices.SelectedDeviceEngName;

                        dataInteraction.AddDataInteractionVarString(weaponName, (int)((weaponName.Length + 1) * sizeof(char)), (uint)datumId);
                        break;
                    }
                    default:
                    break;
                }
            }

            dataInteraction.SenderId = PublishedEntity.MyEntityId;
            dataInteraction.ReceiverId = dataQueryInteraction.SenderId;
            dataInteraction.RequestId = dataQueryInteraction.RequestId;
            DataInteractionManager.Send(dataInteraction);
        }
    }
}