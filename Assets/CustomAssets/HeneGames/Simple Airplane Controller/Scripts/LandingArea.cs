using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HeneGames.Airplane.SimpleAirCreatureController;

namespace HeneGames.Airplane
{
    public class LandingArea : MonoBehaviour
    {
        [SerializeField] private Collider _contactCollider;
        public Vector3 ContactPoint { get; private set; }

        private SimpleAirCreatureController landingAirCreatureController;
        private bool landingCompleted;

        private void OnTriggerEnter(Collider other)
        {
            //Check if colliding object has airplane collider component
            if (other.transform.TryGetComponent(out SimpleAirPlaneCollider _airPlaneCollider))
            {
                SimpleAirCreatureController _controller = _airPlaneCollider.controller;
                if (_controller.airplaneState==AirplaneState.Takeoff)
                {
                    return;
                }
                    
                _controller.airplaneState = AirplaneState.Landing;
                _controller.AddLandingZone(this);
                landingAirCreatureController = _controller;
                ContactPoint = _contactCollider.ClosestPoint(_airPlaneCollider.transform.position);
            }
        }

        public void OverrideContactPoint(Vector3 newPos)
        {
            ContactPoint = newPos;
        }
        
        private void Update()
        {
            if(landingAirCreatureController != null)
            {
                //Move landing adjuster to landing final pos position
                if(!landingCompleted)
                {
                    landingCompleted = true;
                }
                else
                {
                    //Launch airplane
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        landingAirCreatureController.airplaneState = SimpleAirCreatureController.AirplaneState.Takeoff;
                    }

                    //Reset runway if landing airplane is taking off
                    if (landingAirCreatureController.airplaneState == SimpleAirCreatureController.AirplaneState.Flying)
                    {
                        landingAirCreatureController = null;
                        landingCompleted = false;
                    }
                }
            }
        }
    }
}