using UnityEngine;

namespace Simulation
{
    public class SimulationConfiguration : ConfigurationScript<SimulationConfiguration.Configuration>
    {

        [System.Serializable]
        public class Configuration
        {
            [Header("Exercise")]
            public int ExerciseId;
            public NightLoader.Mode NightMode;
            [Header("Terrain")]
            public string TerrainPath;
            public string NavMeshBundlePath;
            public bool FullTerrain = true;
            public string ImprovedTerrainBundlePath;
            public int LoadingRadius;
            public bool ImprovedTerrain;

            [Header("Dynamic Loading Only")]
            public string OriginalsBundlePath;
        }
        [Header("References")]
        public ExerciseConnection ExerciseConnection;
        public NightLoader NightLoader;

        [Header("Terrain Configuration")]
        public TerrainLoader_Full TerrainLoaderFull;
        public TerrainLoader_Dynamic TerrainLoaderDynamic;

        [Header("Editor Only"), Tooltip("This will not affect external builds, but will override EditorConfig.FullTerrain")]
        public bool FullTerrain = true;

        override protected void ApplyConfiguration(bool GotConfiguration)
        {
            //Try finding references
            if (!ExerciseConnection)
                ExerciseConnection = FindObjectOfType<ExerciseConnection>();
            if (!NightLoader)
                NightLoader = FindObjectOfType<NightLoader>();
            if (!TerrainLoaderFull)
                TerrainLoaderFull = FindObjectOfType<TerrainLoader_Full>();
            if (!TerrainLoaderDynamic)
                TerrainLoaderDynamic = FindObjectOfType<TerrainLoader_Dynamic>();

            //Load command line arguments as overrides
            PutArg("FullTerrain", ref Config.FullTerrain, "FullTerrain");
            PutArg("LoadingRadius", ref Config.LoadingRadius, "LoadingRadius");
            PutArg("ImprovedTerrain", ref Config.ImprovedTerrain, "ImprovedTerrain");
            PutArg("TerrainPath", ref Config.TerrainPath, "TerrainPath");
            PutArg("NavMeshBundlePath", ref Config.TerrainPath, "NavMeshBundlePath");
            PutArg("ImprovedTerrainBundlePath", ref Config.ImprovedTerrainBundlePath, "ImprovedTerrainBundlePath");
            PutArg("ExerciseId", ref Config.ExerciseId, "ExerciseId");
            PutArg("NightMode", ref Config.NightMode, EnumParser<NightLoader.Mode>(), "NightMode");

            //forces improved terrain on TerrainName
            if (GotConfiguration && null != Config.TerrainPath)
            {
                string lowerTerrainPath = Config.TerrainPath.ToLower();
                if (lowerTerrainPath.Contains("terrainname"))
                    Config.ImprovedTerrain = true;
            }

            //Apply ExerciseId
            if (ExerciseConnection)
                TryApplyTo(Config.ExerciseId, ref ExerciseConnection.exerciseId, "ExerciseId");

            //Override if in editor
            if (Application.isEditor)
                Config.FullTerrain = FullTerrain;
            
            //Apply terrain values, destroy the loader that wasn't picked 
            if (Config.FullTerrain)
            {
                Destroy(TerrainLoaderDynamic);
                TerrainLoader.Instance = TerrainLoaderFull;

                TryApplyTo(Config.ImprovedTerrain, ref TerrainLoaderFull.ImprovedTerrain, "ImprovedTerrain");
            }
            else
            {
                Destroy(TerrainLoaderFull);
                TerrainLoader.Instance = TerrainLoaderDynamic;

                TryApplyTo(Config.LoadingRadius, ref TerrainLoaderDynamic.DynamicLoadingRadius, "LoadingRadius");
                TryApplyTo(Config.ImprovedTerrain, ref TerrainLoaderDynamic.ImprovedTerrain, "ImprovedTerrain");
            }
            //apply NightMode, add "_night" to terrain path
            if (NightLoader && WasConfigured("NightMode"))
            {
                /*switch (Config.NightMode)
                {
                    case "DayOnly":
                    {
                        NightLoader.NightLoadingMode = NightLoader.Mode.DayOnly;
                        break;
                    }
                    case "NightOnly":
                    {
                        if (!Config.TerrainPath.EndsWith("_night"))
                            Config.TerrainPath += "_night";
                        NightLoader.NightLoadingMode = NightLoader.Mode.NightOnly;
                        break;
                    }
                    case "Dual":
                    {
                        NightLoader.NightLoadingMode = NightLoader.Mode.Dual;
                        break;
                    }
                }*/
                NightLoader.NightLoadingMode = Config.NightMode;
            }

            //Apply paths
            if (TerrainLoader.Instance)
            {
                TryApplyTo(Config.TerrainPath, ref TerrainLoader.Instance.TerrainBundlePath, "TerrainPath");
                TryApplyTo(Config.NavMeshBundlePath, ref TerrainLoader.Instance.NavMeshBundlePath, "NavMeshBundlePath");
                TryApplyTo(Config.ImprovedTerrainBundlePath, ref TerrainLoader.Instance.ImprovedTerrainBundlePath, "ImprovedTerrainBundlePath");

                if (!Config.FullTerrain)
                {
                    PutArg("OriginalsBundlePath", ref Config.OriginalsBundlePath, "OriginalsBundlePath");
                    TryApplyTo(Config.OriginalsBundlePath, ref TerrainLoaderDynamic.TerrainOriginalsBundlePath, "OriginalsBundlePath");
                }
            }

        }

#if UNITY_EDITOR
        /// <summary>
        /// Looks for the relevant references when adding this script to the scene/using the Reset option
        /// </summary>
        void Reset()
        {
            // TerrainLoader = FindObjectOfType<TerrainLoader>();
            ExerciseConnection = FindObjectOfType<ExerciseConnection>();
            NightLoader = FindObjectOfType<NightLoader>();
            TerrainLoaderFull = FindObjectOfType<TerrainLoader_Full>();
            TerrainLoaderDynamic = FindObjectOfType<TerrainLoader_Dynamic>();
            SetEditorConfig(true, new Configuration()
            {
                ExerciseId = ExerciseConnection ? ExerciseConnection.exerciseId : 0,
                FullTerrain = true,
                ImprovedTerrain = TerrainLoaderFull.ImprovedTerrain,
                ImprovedTerrainBundlePath = TerrainLoaderFull.ImprovedTerrainBundlePath,
                LoadingRadius = TerrainLoaderDynamic.DynamicLoadingRadius,
                NightMode = NightLoader.Mode.DayOnly,
                OriginalsBundlePath = TerrainLoaderDynamic.TerrainOriginalsBundlePath,
                TerrainPath = TerrainLoaderFull.TerrainBundlePath
            });
        }
#endif

    }
}