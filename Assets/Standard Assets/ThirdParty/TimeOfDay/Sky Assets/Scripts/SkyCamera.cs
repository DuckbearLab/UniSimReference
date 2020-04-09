using UnityEngine;

public class SkyCamera : MonoBehaviour
{
    public Sky sky;

    protected void OnEnable()
    {
        if (!sky)
        {
            Debug.LogError("Sky instance reference not set. Disabling script.");
            this.enabled = false;
        }
    }

    protected void OnPreCull()
    {
        sky.transform.position = this.transform.position;
    }
}
