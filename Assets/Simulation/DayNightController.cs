using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventReports;
using NetStructs;

namespace Simulation
{
    public class DayNightController : MonoBehaviour
    {
        public NightLoader NightLoader;
        public InfantryConnection InfantryConnection;
        public PublishedEntity PublishedEntity;
        public EventReportsManager EventReportsManager;
        public SimulationTime SimulationTime;
        public EnterExitTermicStruct.Device_Type Device;

        public GlobalInputListener.Key ModeSwitchKey = GlobalInputListener.Key.JOY_KEY6;

        public bool AutoSwitchMode = true;

        private void Awake()
        {
            //if (InfantryConnection)
            //    InfantryConnection.OnDeploy += () => 
            //    {
            //        if (NightLoader.NightAvailable)
            //            NightLoader.SwitchToDay();

            //        SendEventReport(EnterExitTermicStruct.Action.EXIT_TERMIC);
            //    };

            if (AutoSwitchMode)
            {
                SimulationTime = FindObjectOfType<SimulationTime>();
                SimulationTime.OnDay += SwitchToDay;
                SimulationTime.OnNight += SwitchToNight;
            }
        }

        private void Update()
        {
            //if (!AutoSwitchMode)
            //{
            if (NightLoader == null)
            {
                NightLoader = FindObjectOfType<NightLoader>();
            }
            if (GlobalInputListener.GetKeyDown(ModeSwitchKey) && NightLoader.NightAvailable)
                {
                    NightLoader.SwitchMode();

                    if (NightLoader.IsDay)
                        SendEventReport(EnterExitTermicStruct.Action.EXIT_TERMIC);
                    else
                        SendEventReport(EnterExitTermicStruct.Action.ENTER_TERMIC);
                }
            //}
        }

        private void SwitchToDay()
        {
            if (NightLoader.NightAvailable)
                NightLoader.SwitchToDay();
        }

        private void SwitchToNight()
        {
            if (NightLoader.NightAvailable)
                NightLoader.SwitchToNight();
        }

        private void SendEventReport(EnterExitTermicStruct.Action action)
        {
            EnterExitTermicStruct s = new EnterExitTermicStruct()
            {
                deviceType = Device,
                action = action
            };

            EntityId id = EntityId.NullId;

            if (PublishedEntity)
                id = PublishedEntity.MyEntityId;
            else if (InfantryConnection && InfantryConnection.OperatorReflectedEntity)
            {
                var ent = InfantryConnection.OperatorReflectedEntity;
                id = ent.EntityId;
            }
            else
                return;

            s.SenderId = id;
            s.ReceiverId = id;

            EventReportsManager.Send(s);
        }
    }
}