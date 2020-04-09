using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
/* ===================================================================================
 * StartupSelectScene -
 * DESCRIPTION - Populates the dropdown scene selection in the Startup scene with the 
 * names of every scene in the Editor Build Settings. 
 * To add a new scene to the list, use Tools/Register new scenes then save the Startup scene, 
 * or open the startup scene and go to GameObject Canvas > SimulatorsPanel > Scene Selection Dropdown.
 * The build settings scene list is automatically synced to the list. 
 * =================================================================================== */
namespace Startup
{
    public class StartupSelectScene : MonoBehaviour
    {
        public List<string> SceneNames;

        public Dropdown Scenes;
        [HideInInspector, System.NonSerialized]
        public string InitialScene;

        void Start()
        {
            //Add every scene in the SceneNames list to the Dropdown options. 
            Scenes.ClearOptions();
            Scenes.AddOptions(SceneNames);
            //select the initial scene as configured. Default configuration is Infantry. 
            if (!string.IsNullOrEmpty(InitialScene))
            {
                int index = SceneNames.IndexOf(InitialScene);
                if (-1 != index) Scenes.value = index;
                else Scenes.value = 0;
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            //Sets the list of scene names to all the scenes included in the build settings - excluding the Startup scene. 
            int count = SceneNames.Count;
            SceneNames = EditorBuildSettings.scenes.Select(x => System.IO.Path.GetFileNameWithoutExtension(x.path)).Where(x => !x.Contains("Startup")).ToList();
            if (SceneNames.Count != count)
            {
                EditorSceneManager.MarkAllScenesDirty();
            }
        }

        [MenuItem("Tools/Register New Scenes")]
        static void UpdateScenes()
        {
            List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
            int startupIndex = scenes.FindIndex(x => x.path.Contains("Startup"));
            EditorBuildSettingsScene startup = scenes[startupIndex];
            bool wasEnabled = startup.enabled;
            startup.enabled = true;
            scenes[startupIndex] = startup;
            EditorBuildSettings.scenes = scenes.ToArray();
            EditorSceneManager.OpenScene("Assets/Startup.unity", OpenSceneMode.Single);
            //adding a scene to the editor validates every script in the scene, 
            //which includes registerring a new scene in the editor build settings in this scripts OnValidate method.
            Selection.activeGameObject = FindObjectOfType<StartupSelectScene>().gameObject;
            EditorSceneManager.MarkAllScenesDirty();
        }
#endif
    }
}