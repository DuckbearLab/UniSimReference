using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoLineFiller : MonoBehaviour
{

    public Text SliceText; //optional
    public Text EastText;
    public Text NorthText;
    public Text HeightText;
    public Text AzimuthText;
    public Text PitchText;

    public Transform Transform;

    // Use this for initialization
    void Start()
    {
        if(SliceText != null)
            SliceText.text = "36";
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 utmPos = CoordConverter.LocalToUtm(Transform.position);
        EastText.text = Mathf.RoundToInt(utmPos.x).ToString();
        NorthText.text = Mathf.RoundToInt(utmPos.y).ToString();
        HeightText.text = Mathf.RoundToInt(utmPos.z).ToString();

        AzimuthText.text = (Transform.eulerAngles.y).ToString("0.0");

        int pitch = Mathf.FloorToInt(Transform.eulerAngles.x);
        if (pitch > 180)
            pitch -= 360;
        pitch *= -1;
        PitchText.text = Mathf.RoundToInt(pitch).ToString();
    }
}
