using UnityEngine;
using System.Collections;
using System;

namespace Infantry.Devices
{
    public class DeployFoldStructSender : MonoBehaviour
    {
        //will register to events of whatever objects are given.
        public Deployable Deployable;

        public Placeable Placeable;
        public Destroyable Destroyable;

        public PublishedEntity PublishedEntity;

        public EventReportsManager EventReportsManager;

        public EventReports.DeployFoldDevice.DeviceType DeviceType;

        private InfantryLife infantryLife;
        private Device device;

        private void Awake()
        {
            infantryLife = PublishedEntity.GetComponent<InfantryLife>();
            device = gameObject.GetComponent<Device>();

            infantryLife.OnDeath += () =>
            {
                if (device.Selected)
                {
                    SendDeployFoldStruct(EventReports.DeployFoldDevice.Action.START_FOLD);
                    SendDeployFoldStruct(EventReports.DeployFoldDevice.Action.END_FOLD);
                }
            };
        }

        // Use this for initialization
        void Start()
        {
            Action[] actions = new Action[6];
            setSendActions(actions);
            int i = 0;
            if (Deployable != null)
            {
                Deployable.OnStartDeploy += actions[i++];
                Deployable.OnDeployed += actions[i++];
                Deployable.OnCanceledDeploy += actions[i++];
                Deployable.OnStartFold += actions[i++];
                Deployable.OnFolded += actions[i++];
                Deployable.OnCanceledFold += actions[i++];
            }
            else
            {
                int temp = i++;
                Placeable.OnStartPlace += (Vector3 a, Vector3 b) => actions[temp]();
                int temp2 = i++;
                Placeable.OnPlace += (Vector3 a, Vector3 b) => actions[temp2]();
                Placeable.OnCanceledPlace += actions[i++];
                Destroyable.OnStartDestroy += actions[i++];
                Destroyable.OnDestroy += actions[i++];
                Destroyable.OnCanceledDestroy += actions[i++];
            }
        }

        private void setSendActions(Action[] actions)
        {
            if (actions.Length < 6)
                throw new Exception("not enough actions in array");
            int i = 0;
            actions[i++] = () => { SendDeployFoldStruct(EventReports.DeployFoldDevice.Action.START_DEPLOY); };
            actions[i++] = () => { SendDeployFoldStruct(EventReports.DeployFoldDevice.Action.END_DEPLOY); };
            actions[i++] = () => { SendDeployFoldStruct(EventReports.DeployFoldDevice.Action.ABORT_DEPLOY); };
            actions[i++] = () => { SendDeployFoldStruct(EventReports.DeployFoldDevice.Action.START_FOLD); };
            actions[i++] = () => { SendDeployFoldStruct(EventReports.DeployFoldDevice.Action.END_FOLD); };
            actions[i++] = () => { SendDeployFoldStruct(EventReports.DeployFoldDevice.Action.ABORT_FOLD); };
        }

        private void SendDeployFoldStruct(EventReports.DeployFoldDevice.Action deviceAction)
        {
            EventReports.DeployFoldDevice s = new EventReports.DeployFoldDevice()
            {
                action = deviceAction,
                deviceType = DeviceType,
                SenderId = PublishedEntity.MyEntityId
            };
            EventReportsManager.Send(s);
        }


    }
}