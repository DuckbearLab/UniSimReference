using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Startup
{
/* ===================================================================================
 * StartupConfiguration -
 * DESCRIPTION - Configure the startup scene. 
 * IMPORTANT
 * -----------
 * Don't forget to include the Startup scene in the build settings in order to DebugConfig. 
 * -----------
 * Fields: 
 * - Simulator: The selected scene to jump to. 
 * - DebugConfig: If false, immediately launches the scene that is given in Config.Simulator;
 *                If true, selects that scene in the scene dropdown list and waits. The user
 *                can use this to attach a debugger and then launch the scene, in order to debug 
 *                the configuration script. 
 * =================================================================================== */
    /// <summary>
    /// Configures the Startup scene. 
    /// </summary>
    public class StartupConfiguration : ConfigurationScript<StartupConfiguration.Configuration>
    {

        [System.Serializable]
        public class Configuration
        {
            public string Simulator;
            public bool DebugConfig;
        }

        public StartupController StartupController;
        public StartupSelectScene StartupSelectScene;

        protected override void ApplyConfiguration(bool GotConfiguration)
        {
            PutArg("Simulator", ref Config.Simulator);
            if (!Config.DebugConfig)
            {
                if (!string.IsNullOrEmpty(Config.Simulator))
                {
                    StartupController.LoadSimulator(Config.Simulator);
                }
            }
            else
            {
                StartupSelectScene.InitialScene = Config.Simulator;
            }
        }

    }
}
