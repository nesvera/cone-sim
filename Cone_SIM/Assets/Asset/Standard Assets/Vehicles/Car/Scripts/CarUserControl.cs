using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void Update()
        {
			float gas = 0;
			float brake = 0;
			float steering = 0;
			float handbrake = 0;

			// Keyboard
			if (GameControl.controller_type == 0) {
				steering = CrossPlatformInputManager.GetAxis("Horizontal");
				gas = brake = CrossPlatformInputManager.GetAxis("Vertical");
				handbrake = CrossPlatformInputManager.GetAxis("Jump");

			// Xbox - xinput
			} else if (GameControl.controller_type == 1) {
				// Windows
				//steering = CrossPlatformInputManager.GetAxis("Horizontal");
				//gas = brake = CrossPlatformInputManager.GetAxis("Vertical");
				//handbrake = CrossPlatformInputManager.GetAxis("Jump");

				// Linux
				steering = CrossPlatformInputManager.GetAxis("Horizontal");
				gas = CrossPlatformInputManager.GetAxis("Gas");
				brake = CrossPlatformInputManager.GetAxis("Brake");
				handbrake = CrossPlatformInputManager.GetAxis("Jump");

			} else if (GameControl.controller_type == 2) {
				steering = CrossPlatformInputManager.GetAxis("Horizontal");
				gas = CrossPlatformInputManager.GetAxis ("Gas");
				brake = (-1)*CrossPlatformInputManager.GetAxis ("Brake");
				// handbrake = CrossPlataformInputManager.GetAxis("Jump");

			}

#if !MOBILE_INPUT
			m_Car.Move(steering, gas, brake, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
