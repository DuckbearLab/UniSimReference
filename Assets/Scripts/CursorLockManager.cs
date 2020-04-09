using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * CursorLockManager -
 * A script that centralizes locking/unlocking the cursors. This class works with or 
 * without an instance in the scene. 
 * If there is an instance, this class will also lock the cursor on Start(), and check every 
 * Update() for input of UseKey to use the mouse or Mouse0 to release it. 
 * Call CursorLockManager.UseMouse(this) from wherever ('this' being the caller) to add that script 
 * to the list of scripts that currently need the mouse. When you finish using it, just call 
 * CursorLockManager.ReleaseMouse(this) to remove the caller from the list. each caller is
 * only gonna show up once - don't worry about calling UseMouse() or ReleaseMouse() more than once. 
 * =================================================================================== */

public class CursorLockManager : MonoBehaviour
{
    /// <summary>
    /// Creates an instance of this script in every session. 
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void CreateInstance()
    {
        if (!FindObjectOfType<CursorLockManager>())
            (GameObject.Find("Globals") ?? new GameObject("Globals")).AddComponent<CursorLockManager>();
    }
    public KeyCode UseKey = KeyCode.L;

    /// <summary>
    /// Whether the mouse is currently in use. 
    /// </summary>
    public static bool IsInUse
    {
        get
        {
            return IdList.Count > 0;
        }
    }

    private static List<object> IdList = new List<object>();

    /// <summary>
    /// Whether a locked mouse is the default state. 
    /// </summary>
    public static bool LockedIsDefault
    {
        get
        {
            return lockedIsDefault;
        }
        set
        {
            if (value)
            {
                Cursor.lockState = CursorLockMode.Locked;
                lockedIsDefault = true;
            }
            else
            {
                lockedIsDefault = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    private static bool lockedIsDefault;

    void Update()
    {
        if (lockedIsDefault)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                ReleaseMouse(this);
                if (!IsInUse)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            else if (Input.GetKeyDown(UseKey))
            {
                UseMouse(this);
            }
        }
    }

    /// <summary>
    /// Tells the manager that the caller needs to use the mouse. 
    /// <para>
    /// Remember to call <see cref="ReleaseMouse(object)"/> with the same <see cref="object"/> parameter to tell the manager the mouse is no longer needed. 
    /// </para>
    /// </summary>
    /// <param name="Caller">The caller object. Should usually be "this". </param>
    public static void UseMouse(object Caller)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (!IdList.Contains(Caller))
            IdList.Add(Caller);
    }

    /// <summary>
    /// Tells the manager that the caller needs to use the mouse, confined to the play window.
    /// <para>
    /// Remember to call <see cref="ReleaseMouse(object)"/> with the same <see cref="object"/> parameter to tell the manager the mouse is no longer needed. 
    /// </para>
    /// </summary>
    /// <param name="Caller">The caller object. Should usually be "this". </param>
    public static void UseMouseConfined(object Caller)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (!IdList.Contains(Caller))
            IdList.Add(Caller);
    }

    /// <summary>
    /// Tells the manager that the caller no longer needs to use the mouse. 
    /// </summary>
    /// <param name="Caller">The caller object. This should be the same <see cref="object"/> used to call <see cref="UseMouse(object)"/> or <see cref="UseMouseConfined(object)"/>. </param>
    public static void ReleaseMouse(object Caller)
    {
        if (IdList.Contains(Caller))
        {
            IdList.Remove(Caller);
            if (IdList.Count == 0)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}


