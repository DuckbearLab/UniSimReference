using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ===================================================================================
 * TriggerButtonFromElseWhere -
 * DESCRIPTION -
 * =================================================================================== */

public class TriggerButtonFromElseWhere : MonoBehaviour
{
    public Button Button;

    public void TriggerButton()
    {
        if (null != Button)
        {
            Button.onClick.Invoke();
        }
    }
}