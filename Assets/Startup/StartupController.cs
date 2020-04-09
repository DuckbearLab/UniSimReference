using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Startup
{
    public class StartupController : MonoBehaviour
    {
        public Dropdown ScenesDropdown;

        public void LoadSimulator(string simulatorName)
        {
            SceneManager.LoadScene(simulatorName);
        }
        public void Go()
        {
            LoadSimulator(ScenesDropdown.options[ScenesDropdown.value].text);
        }
    }
}