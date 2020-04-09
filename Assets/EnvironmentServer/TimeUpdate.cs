using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimeUpdate : MonoBehaviour 
{
    public Button CloseButton;
    public InputField HouresInputField;
    public InputField MinutesInputField;
    public InputField SecondesInputField;
    public Slider TimeSlider;
	// Use this for initialization
	void Start () {
        Button btn = CloseButton.GetComponent<Button>();
        btn.onClick.AddListener(UpdateTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void UpdateTime()
    {
        if (HouresInputField.text != "" || MinutesInputField.text != "" || SecondesInputField.text != "")
        {
            int hourse;
            int minutes;
            int secondes;
            int.TryParse(HouresInputField.text, out hourse);
            int.TryParse(MinutesInputField.text, out minutes);
            int.TryParse(SecondesInputField.text, out secondes);
            TimeSlider.value = hourse * 60 * 60 + minutes * 60 + secondes;
        }
    }
}
