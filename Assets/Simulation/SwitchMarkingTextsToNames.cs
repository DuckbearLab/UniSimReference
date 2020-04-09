using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
/* ===================================================================================
 * SwitchMarkingTextsToNames -
 * DESCRIPTION - Receives a file path under MarkingTextsToNamesFilePath config value. 
 * Then parse the contents of that file as a Dictionary<string, string> to replace the shown
 * marking texts with names. 
 * Example File:
{
    "stationName": "Kfir"
}
 * Keys are case insensitive. Values keep their casing. 
 * =================================================================================== */

namespace Simulation
{
    public class SwitchMarkingTextsToNames : ConfigurationScript<SwitchMarkingTextsToNames.Configuraion>
    {
        private static Dictionary<string, string> configuredSwitches;
        [System.Serializable]
        public struct Configuraion
        {
            public string MarkingTextsToNamesFilePath;
        }

        protected override void ApplyConfiguration(bool GotConfiguration)
        {
            if (!GotConfiguration)
                return;
            if (!WasConfigured("MarkingTextsToNamesFilePath"))
                return;
            if (!File.Exists(Config.MarkingTextsToNamesFilePath))
                return;
            try
            {
                //read configured file
                Dictionary<string, string> toAdd = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Config.MarkingTextsToNamesFilePath));
                configuredSwitches = new Dictionary<string, string>();
                //copy a lowercase version of every marking text into the static dictionary
                foreach (KeyValuePair<string, string> pair in toAdd)
                {
                    configuredSwitches.Add(pair.Key.ToLower(), pair.Value);
                }
            }
            catch
            {
                configuredSwitches = null;
            }
        }

        /// <summary>
        /// Checks if a name was configured to the given case insensitive Marking Text, and if it was, returns the value it was configured for.
        /// Otherwise, returns the given marking text. 
        /// </summary>
        /// <param name="MarkingText">The marking text to switch. Case insensitive. </param>
        /// <returns></returns>
        public static string GetConfiguredName(string MarkingText)
        {
            if (null == MarkingText || null == configuredSwitches)
                return MarkingText;
            string name;
            if (configuredSwitches.TryGetValue(MarkingText.ToLower(), out name))
                return name;
            return MarkingText;
        }
    }
}