using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowPositionSaver : MonoBehaviour {

    private string processName;
    private string sceneName;

	void Awake () {

        if (!Application.isEditor)
        {
            processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += LoadNewScenePosition;
            DontDestroyOnLoad(gameObject);
            sceneName = "Startup";
            LoadPosition();
            
            StartCoroutine(SaveScreenPositionPeriodically());
        }
	}

    private void LoadPosition()
    {
        if (HasKey("x") && HasKey("y") && HasKey("w") && HasKey("h"))
        {
            int x = GetKey("x"), y = GetKey("y"), h = GetKey("h"), w = GetKey("w");

            if (/*GetKey("x") >= -1000 && GetKey("y") >= 0 &&*/ GetKey("w") > 20 && GetKey("h") > 20)
            {
                Debug.Log("WindowPositionSaver LOADING: " + x + ", " + y + ", " +w + ", " + h);
                ScreenManager.SetScreenPos(x, y, w, h);
            }
        }
    }


    private void LoadNewScenePosition(UnityEngine.SceneManagement.Scene prevScene, UnityEngine.SceneManagement.Scene newScene)
    {
        sceneName = newScene.name;
        LoadPosition();
    }

    private IEnumerator SaveScreenPositionPeriodically()
    {
        while(true)
        {
            yield return new WaitForSeconds(3);

            var rect = ScreenManager.GetWindowRect();

            int x = rect.Left;
            int y = rect.Top;
            int w = rect.Right - rect.Left;
            int h = rect.Bottom - rect.Top;

            if (/*x >= 0 && y >= 0 &&*/ w > 20 && h > 20)
            {
                SetKey("x", rect.Left);
                SetKey("y", rect.Top);
                SetKey("w", rect.Right - rect.Left);
                SetKey("h", rect.Bottom - rect.Top);
            }
        }
    }

    private void SetKey(string key, int value)
    {
        PlayerPrefs.SetInt(BuildKey(processName, "::", sceneName, "::WindowPositionSaver::", key), value);
    }

    private int GetKey(string key)
    {
        return PlayerPrefs.GetInt(BuildKey(processName, "::", sceneName, "::WindowPositionSaver::", key));
    }

    private bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(BuildKey(processName, "::", sceneName, "::WindowPositionSaver::", key));
    }
	

    private string BuildKey(params object[] parts)
    {
        System.Text.StringBuilder b = new System.Text.StringBuilder();
        foreach (object part in parts)
            b.Append(part);
        return b.ToString();
    }
}
