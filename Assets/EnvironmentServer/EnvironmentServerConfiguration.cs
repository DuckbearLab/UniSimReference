using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace EnvironmentServer
{
    public class EnvironmentServerConfiguration : ConfigurationScript<EnvironmentServerConfiguration.Configuration>
    {
        public ExerciseConnection ExerciseConnection;
        public ScenarioInfo ConfigScenarioInfo;
        public ScenarioInfo DestinyScenarioInfo;
        public UnityEngine.UI.Text ExerciseNum;
        public GameObject LinePrefab;
        public GameObject DestinyScenarioInformationGrid;
        public Button setScenarionsButton;
        public Button ReadConfigButton;
        public InputField PathInputFiled;
        public string Path;
        [System.Serializable]
        public class Configuration
        {
            public string LoadJSON;
        }
        [System.Serializable]
        public class ConfigScenarioLine
        {
            public string StartTime;
            public string Duration;
            public string Weather;
            public bool Fog;
            public bool Lightning;
            public string Intensity;
            public string CloudCovrege;

            public ConfigScenarioLine(ScenarioValues line)
            {
                StartTime = line.StartTime.text;
                Duration = line.Duration.text;
                Weather = line.Weather.text;
                Fog = line.Fog.isOn;
                Lightning = line.Lightning.isOn;
                Intensity = line.Intensity.text;
                CloudCovrege = line.CloudCovrege.text;
            }
        }
        public class ScenariosList
        {
            public List<ConfigScenarioLine> Scenarios_List { get; set; }
        }
        public BuildingDamageManager BuildingDamageManager;

        protected override void ApplyConfiguration(bool GotConfiguration)
        {
            PutArg("LoadJSON", ref Config.LoadJSON);

            TryApplyTo(Config.LoadJSON, ref BuildingDamageManager.LoadJSONData);
        }
        void Start()
        {
            ExerciseNum.text = ExerciseConnection.exerciseId.ToString();
            setScenarionsButton.onClick.AddListener(startSaving);
            ReadConfigButton.onClick.AddListener(StartReading);
        }
        public void startSaving()
        {
            if (PathInputFiled.text != null)
                if (File.Exists(PathInputFiled.text))
                    Path = PathInputFiled.text;
            int count = 0;
            ConfigScenarioLine[] ConfigScenarioLines = new ConfigScenarioLine[ConfigScenarioInfo.GetScenariosList().Count];
            foreach (ScenarioValues sv in ConfigScenarioInfo.GetScenariosList())
            {
                ConfigScenarioLines[count] = new ConfigScenarioLine(sv);
                count++;
            }
            SaveToJSON(ConfigScenarioLines, Path);
        }
        public void StartReading()
        {
            if (PathInputFiled.text != null)
                if (File.Exists(PathInputFiled.text))
                    Path = PathInputFiled.text;
            ReadFromJSON(Path);
        }

        public void SaveToJSON(ConfigScenarioLine[] ScenarioLines, string Path)
        {
            int temp = 0;
            if (!Path.EndsWith(".json"))
                Path += ".json";

            string FinalJson = "";
            foreach (ConfigScenarioLine sl in ScenarioLines)
            {
                if (temp != 0)
                    FinalJson += "|";
                FinalJson += JsonUtility.ToJson(sl);
                temp++;
            }
            System.IO.File.WriteAllText(Path, FinalJson, System.Text.Encoding.UTF8);
        }

        public ConfigScenarioLine[] ReadFromJSON(string Path)
        {
            string verJSON = System.IO.File.ReadAllText(Path, System.Text.Encoding.UTF8);
            string[] Lines = verJSON.Split('|');
            ConfigScenarioLine[] result = new ConfigScenarioLine[Lines.Length];
            for (int i = 0; i < Lines.Length; i++)
            {
                result[i] = JsonUtility.FromJson<ConfigScenarioLine>(Lines[i]);
            }
            foreach (ConfigScenarioLine line in result)
            {
                GameObject newLine = Instantiate(LinePrefab, DestinyScenarioInformationGrid.transform, false);
                ((RectTransform)DestinyScenarioInformationGrid.transform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 1, 30 * DestinyScenarioInformationGrid.transform.childCount);
                ScenarioValues senValues = newLine.GetComponent<ScenarioValues>();
                senValues.GetConfigInfo(line);
                DestinyScenarioInfo.AddScenario(newLine);
            }
            return result;
        }
        public ScenarioValues ConfigLineToScenarioValues(ConfigScenarioLine line)
        {
            GameObject newLine = Instantiate(LinePrefab, DestinyScenarioInformationGrid.transform, false);
            ((RectTransform)DestinyScenarioInformationGrid.transform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 1, 30 * DestinyScenarioInformationGrid.transform.childCount);
            ScenarioValues senValues = newLine.GetComponent<ScenarioValues>();

            senValues.StartTime.text = line.StartTime;
            senValues.Duration.text = line.Duration;
            senValues.Weather.text = line.Weather;
            senValues.Fog.isOn = line.Fog;
            senValues.Lightning.isOn = line.Lightning;
            senValues.Intensity.text = line.Intensity;
            senValues.CloudCovrege.text = line.CloudCovrege;
            senValues.ScenarioID = -5;

            return senValues;
        }
    }
}