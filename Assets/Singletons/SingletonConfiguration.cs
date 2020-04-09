using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * SingletonConfiguration -
 * DESCRIPTION -  makes whatever inherits from it a configuration singleton.
 * =================================================================================== */
public abstract class SingletonConfiguration< TConfig, T> : ConfigurationScript<TConfig> where T : SingletonConfiguration<TConfig, T> where TConfig : new()
{
    private static GameObject Globals;
    private static T _instance;
    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    protected override void Awake()
    {
        if (null != _instance)
        {
            Debug.LogError("Multiple instances of " + GetType());
            Destroy(this);
        }
        _instance = (T)this;
        base.Awake();
    }
}