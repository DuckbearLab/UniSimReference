using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * SingeltonMonoBehavior - 
 * DESCRIPTION -makes whatever inherits from it a singleton.
 * =================================================================================== */

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    private static GameObject Globals;
    private static T _instance;
    public static T Instance
    {
        get
        {
            //there has to be an instance to work.
            if (null == _instance)
            {
                _instance = ObjectFinder.FindObjectOfType<T>();
                if (null == _instance)
                {
                        Globals = Globals ?? (GameObject.Find("Globals") ?? new GameObject("Globals"));
                    _instance = Globals.AddComponent<T>();
                    _instance.OnNewInstanceCreated();
                }
            }
            _instance.OnGetInstance();
            return _instance;
        }
    }

    /// <summary>
    /// When Overriding, don't forget to call base.Awake();
    /// </summary>
    protected virtual void Awake()
    {
        if (null == _instance)
        {
            _instance = (T)this;
        }
        else if (_instance != this)
        {
            throw new System.Exception("Multiple instances of " + GetType().Name + " present");
        }
    }

    /// <summary>
    /// called when an instance isn't found and was created.
    /// </summary>
    protected virtual void OnNewInstanceCreated() { }

    /// <summary>
    /// called when getting instance.
    /// </summary>
    protected virtual void OnGetInstance() { }

    /// <summary>
    /// override "OnGetInstance" with this to prevent creating a new instance.
    /// </summary>
    protected static void ThrowExceptionOnNewInstance()
    {
        throw new System.Exception("Can't create a new instance of " + typeof(T));
    }
}


