using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventReports;

namespace Simulation
{
    public class SimulationTime : MonoBehaviour
    {
        public EnvironmentStateListener EnvironmentStateListener;

        [Range(0, 23)]
        public int Hours;
        [Range(0, 59)]
        public int Minutes, Seconds;

        public float TimeMultiplier = 1;

        [Range(0, 23.99f)]
        public float NightStartTime = 18;
        [Range(0, 23.99f)]
        public float NightEndTime = 6;

        public event Action OnDay;
        public event Action OnNight;

        private TimeSpan currentTime;

        private bool previousDayState = true;

        private void Start()
        {
            EnvironmentStateListener = FindObjectOfType<EnvironmentStateListener>();
        }

        private void Update()
        {
            if (EnvironmentStateListener.EnviromnentServerExists)
            {
                Hours = EnvironmentStateListener.Hour;
                Minutes = EnvironmentStateListener.Minute;
            }
            else
            {
                currentTime = new TimeSpan(0, Hours, Minutes, Seconds, currentTime.Milliseconds);

                TimeSpan deltaTime = TimeSpan.FromSeconds(Time.deltaTime * TimeMultiplier);
                currentTime = currentTime.Add(deltaTime);

                Hours = currentTime.Hours;
                Minutes = currentTime.Minutes;
                Seconds = currentTime.Seconds;
                EnvironmentStateListener.SetTime(Hours, Minutes, Seconds);
            }
            CheckDayToNightSwitch();
        }

        private void CheckDayToNightSwitch()
        {
            bool day = IsDay;

            if (previousDayState != day)
            {
                if (day)
                {
                    if (OnDay != null)
                        OnDay();
                }
                else if (OnNight != null)
                    OnNight();
            }

            previousDayState = day;
        }


        public int TimeInMinutes
        {
            get
            {
                return Hours * 60 + Minutes;
            }
        }

        public bool IsDay
        {
            get
            {
                if (NightStartTime > NightEndTime)
                {
                    if (Hours >= NightStartTime || Hours < NightEndTime)
                        return false;
                }
                else
                {
                    if (Hours >= NightStartTime && Hours < NightEndTime)
                        return false;
                }
                return true;
            }
        }
    }
}