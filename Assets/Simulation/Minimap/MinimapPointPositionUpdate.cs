using Simulation.Minimap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinimapPointPositionUpdate : MonoBehaviour
{
    public Minimap Minimap;
    public MinimapPointPositioner MinimapPositioner;

    public InputField InputFieldX;
    public InputField InputFieldY;

    public void OnUpdatePositionBtnClick()
    {
        int x = 0, y = 0;

        if (InputFieldX)
            if (!int.TryParse(InputFieldX.text, out x))
            {
                Debug.Log("Error Input X");
                return;
            }

        if (InputFieldY)
            if (!int.TryParse(InputFieldY.text, out y))
            {
                Debug.Log("Error Input Y");
                return;
            }

        Vector3 localPos = CoordConverter.UtmToLocal(new Vector3(x, y, 0));
        MinimapPositioner.SetWorldPoint(localPos);
        Minimap.FocusOnWorldPoint(localPos);

    }

}
