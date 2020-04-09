using System;
using UnityEngine;

using UnityStandardAssets.CrossPlatformInput;


namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        public float h = 0;
        public float v = 0;
        public float handbrake = 0;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        public void setGasValues(float hFromUser, float vFromUser, float handbrakeFromUser)
        {
            h = hFromUser;
            v = vFromUser;
            handbrake = handbrakeFromUser;
            
        }

        private void FixedUpdate()
        {
            // pass the input to the car!
           // float h = CrossPlatformInputManager.GetAxis("Horizontal");
         //   float v = CrossPlatformInputManager.GetAxis("Vertical");

#if !MOBILE_INPUT
           // float handbrake = CrossPlatformInputManager.GetAxis("BrakePedal");
         //   Debug.Log("Brake = " + handbrake);
          //  handbrake = CrossPlatformInputManager.GetAxis("BrakePedal");
          //  Debug.Log("BrakePedal = " + handbrake);

            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
