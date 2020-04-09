using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockDisplay : MonoBehaviour 
{
    public Slider TimeSlider;
    public Text ClockText;

    public int houres;
    public int minutes;
    public int secondes;
	// Use this for initialization
	void Start () {
        TimeSlider.value = 10 * 3600;

        if (PlayerPrefs.HasKey("EnvironmentServer::TimeSliderValue"))
        {
            TimeSlider.value = PlayerPrefs.GetFloat("EnvironmentServer::TimeSliderValue");
        }

        StartCoroutine(SaveTimePeriodically());
	}

    private IEnumerator SaveTimePeriodically()
    {
        while(true)
        {
            yield return new WaitForSeconds(3);
            PlayerPrefs.SetFloat("EnvironmentServer::TimeSliderValue", TimeSlider.value);
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        SliderToTimeValues();
        ClockTextEdit();
	}

    void SliderToTimeValues()
    {
        int temp = (int)TimeSlider.value;
        secondes = temp % 60;
        temp = temp / 60;
        minutes = temp % 60;
        temp = temp / 60;
        houres = temp;
    }

    void ClockTextEdit()
    {
        ClockText.text = houres.ToString("00") + ":" + minutes.ToString("00") + ":" + secondes.ToString("00");
    }
}
