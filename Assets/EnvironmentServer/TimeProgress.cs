using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using UnityEngine.UI;

public class TimeProgress : MonoBehaviour 
{
    public Slider TimeSlider;
    public Text TimeMultiplier;

    private int multiplier;

	// Use this for initialization
	void Start () {
        InvokeRepeating("SliderUpdate", 0, 1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SliderUpdate()
    {
        if (TimeMultiplier != null && TimeMultiplier.text!="")
        {
            int.TryParse(TimeMultiplier.text, out multiplier);
            TimeSlider.value += multiplier;
        }
        else
            TimeSlider.value+=1;
        TimeSlider.value = TimeSlider.value % 86400;
    }
}
