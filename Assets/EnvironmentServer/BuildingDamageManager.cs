using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventReports;

namespace EnvironmentServer
{
    public class BuildingDamageManager : MonoBehaviour
    {
        [System.Serializable]
        private class DamageMessages
        {
            public DamageMessages()
            {
                Messages = new List<BuildingDamageResult>();
            }

            public List<BuildingDamageResult> Messages;
        }

        public EventReportsManager EventReportsManager;
        public bool LoadJSONData = true;

        private DamageMessages damageMessages;

        private const string FileName = "damagedBuildings";

        private void Start()
        {
            damageMessages = new DamageMessages();

            if (LoadJSONData)
            {
                JSONDataSaver.Read<DamageMessages>(damageMessages, FileName);
                PublishReport();
            }

            EventReportsManager.Subscribe<BuildingDamageResult>(BuildingDamageHandler);
            EventReportsManager.Subscribe<DamagedBuildingsRequest>(RequestReceivedHandler);
        }

        private void BuildingDamageHandler(BuildingDamageResult bdr)
        {
            damageMessages.Messages.Add(bdr);
            JSONDataSaver.Write(damageMessages, FileName);
        }

        private void RequestReceivedHandler(DamagedBuildingsRequest dbr)
        {
            PublishReport();
        }

        private void PublishReport()
        {
            DamagedBuildingsReport report = new DamagedBuildingsReport();

            int length = damageMessages.Messages.Count;

            DamagedBuildingsReport.DamagedBuildings[] damagedBuildings = new DamagedBuildingsReport.DamagedBuildings[length];

            for (int i = 0; i < length; i++)
            {
                BuildingDamageResult resultMessage = damageMessages.Messages[i];

                damagedBuildings[i] = new DamagedBuildingsReport.DamagedBuildings()
                {
                    Location = resultMessage.Location,
                    State = (DamagedBuildingsReport.DamageState)resultMessage.Result
                };
            }

            report.DamagedBuildingsLength = length;
            report.Buildings = damagedBuildings;

            EventReportsManager.Send(report);
        }

        public void ClearJSON()
        {
            JSONDataSaver.Clear(FileName);
        }
    }
}