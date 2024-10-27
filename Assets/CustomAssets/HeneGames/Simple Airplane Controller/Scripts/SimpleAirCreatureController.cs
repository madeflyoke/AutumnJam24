using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using System;

namespace HeneGames.Airplane
{
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleAirCreatureController : MonoBehaviour
    {
        public enum AirplaneState
        {
            Flying,
            Landing,
            Takeoff,
        }

        public Action crashAction;

        #region Private variables

        private List<SimpleAirPlaneCollider> airPlaneColliders = new List<SimpleAirPlaneCollider>();

        private float maxSpeed = 0.6f;
        private float speedMultiplier;
      //  private float currentYawSpeed;
        private float currentPitchSpeed;
        private float currentRollSpeed;
        private float currentSpeed;
        private float currentEngineLightIntensity;
        private float lastEngineSpeed;
        
        private Rigidbody rb;
        private LandingArea _currentLandingArea;

        //Input variables
        private float inputH;
        private float inputV;
        private bool inputTurbo;
        private bool inputSlower;
        // private bool inputYawLeft;
        // private bool inputYawRight;

        private bool _landed;

        #endregion

        public AirplaneState airplaneState;

        [SerializeField] private CrowAnimator _crowAnimator;
        [SerializeField] private SimpleAirCreatureCamera _cam;
        
        [Header("Wing trail effects")]
        [Range(0.01f, 1f)]
        [SerializeField] private float trailThickness = 0.045f;
        [SerializeField] private TrailRenderer[] wingTrailEffects;

        // [Header("Rotating speeds")]
        // [Range(5f, 500f)]
        // [SerializeField] private float yawSpeed = 50f;

        [Range(5f, 500f)]
        [SerializeField] private float pitchSpeed = 100f;

        [Range(5f, 500f)]
        [SerializeField] private float rollSpeed = 200f;

        // [Header("Rotating speeds multiplers when turbo is used")]
        // [Range(0.1f, 5f)]
        // [SerializeField] private float yawTurboMultiplier = 0.3f;

        [Range(0.1f, 5f)]
        [SerializeField] private float pitchTurboMultiplier = 0.5f;

        [Range(0.1f, 5f)]
        [SerializeField] private float rollTurboMultiplier = 1f;

        [Header("Moving speed")]
        [Range(5f, 100f)]
        [SerializeField] private float defaultSpeed = 10f;

        [Range(10f, 200f)]
        [SerializeField] private float turboSpeed = 20f;

        [Range(0.1f, 50f)]
        [SerializeField] private float accelerating = 10f;

        [Range(0.1f, 50f)]
        [SerializeField] private float deaccelerating = 5f;

        [Header("Sideway force")]
        [Range(0.1f, 15f)]
        [SerializeField] private float sidewaysMovement = 15f;

        [Range(0.001f, 0.05f)]
        [SerializeField] private float sidewaysMovementXRot = 0.012f;

        [Range(0.1f, 5f)]
        [SerializeField] private float sidewaysMovementYRot = 1.5f;

        [Range(-1, 1f)]
        [SerializeField] private float sidewaysMovementYPos = 0.1f;
        
        [Header("Colliders")]
        [SerializeField] private Transform crashCollidersRoot;

        [Header("Takeoff settings")]
        [Tooltip("How far must the plane be from the runway before it can be controlled again")]
        [SerializeField] private float takeoffLenght = 30f;

        private void Start()
        {
            //Setup speeds
            maxSpeed = defaultSpeed;
            currentSpeed = defaultSpeed;
            ChangeSpeedMultiplier(1f);

            //Get and set rigidbody
            rb = GetComponent<Rigidbody>();

//            SetupColliders(crashCollidersRoot);
        }

        private void Update()
        {
            AudioSystem();
            HandleInputs();

            // switch (airplaneState)
            // {
            //     case AirplaneState.Flying:
            //         FlyingUpdate();
            //         break;
            //
            //     case AirplaneState.Landing:
            //         LandingUpdate();
            //         break;
            //
            //     case AirplaneState.Takeoff:
            //         TakeoffUpdate();
            //         break;
            // }
        }

        private void FixedUpdate()
        {
            FlyingUpdate();
        }

        #region Flying State

        private void FlyingUpdate()
        {
            //Airplane move only if not dead
            if (true)
            {
                Movement();
                SidewaysForceCalculation();
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
            }
            else
            {
                ChangeWingTrailEffectThickness(0f);
            }

            //Crash
            // if (HitSometing())
            // {
            //     Crash();
            // }
        }

        private void SidewaysForceCalculation()
        {
            float _mutiplierXRot = sidewaysMovement * sidewaysMovementXRot;
            float _mutiplierYRot = sidewaysMovement * sidewaysMovementYRot;

            float _mutiplierYPos = sidewaysMovement * sidewaysMovementYPos;

            //Right side 
            if (transform.localEulerAngles.z > 270f && transform.localEulerAngles.z < 360f)
            {
                float _angle = (transform.localEulerAngles.z - 270f) / (360f - 270f);
                float _invert = 1f - _angle;

                transform.Rotate(Vector3.up * (_invert * _mutiplierYRot) * Time.deltaTime);
                transform.Rotate(Vector3.right * (-_invert * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);
                
                rb.velocity += transform.up * (_invert * _mutiplierYPos);
            }

            //Left side
            if (transform.localEulerAngles.z > 0f && transform.localEulerAngles.z < 90f)
            {
                float _angle = transform.localEulerAngles.z / 90f;

                transform.Rotate(-Vector3.up * (_angle * _mutiplierYRot) * Time.deltaTime);
                transform.Rotate(Vector3.right * (-_angle * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);

                rb.velocity += transform.up * (_angle * _mutiplierYPos);
            }

            //Right side down
            if (transform.localEulerAngles.z > 90f && transform.localEulerAngles.z < 180f)
            {
                float _angle = (transform.localEulerAngles.z - 90f) / (180f - 90f);
                float _invert = 1f - _angle;

                rb.velocity += transform.up * (_invert * _mutiplierYPos);
                transform.Rotate(Vector3.right * (-_invert * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);
            }

            //Left side down
            if (transform.localEulerAngles.z > 180f && transform.localEulerAngles.z < 270f)
            {
                float _angle = (transform.localEulerAngles.z - 180f) / (270f - 180f);

                rb.velocity += transform.up * (_angle * _mutiplierYPos);
                transform.Rotate(Vector3.right * (-_angle * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);
            }
        }

        private void Movement()
        {
            // Move the Rigidbody using MovePosition
            rb.velocity += transform.forward * currentSpeed;

            //Store last speed
            lastEngineSpeed = currentSpeed;

            //Rotate airplane by inputs
            transform.Rotate(Vector3.forward * -inputH * currentRollSpeed * Time.deltaTime);
            transform.Rotate(Vector3.right * inputV * currentPitchSpeed * Time.deltaTime);

            // //Rotate yaw
            // if (inputYawRight)
            // {
            //     transform.Rotate(Vector3.up * currentYawSpeed * Time.deltaTime);
            // }
            // else if (inputYawLeft)
            // {
            //     transform.Rotate(-Vector3.up * currentYawSpeed * Time.deltaTime);
            // }

            //Accelerate and deacclerate
            if (currentSpeed < maxSpeed)
            {
                currentSpeed += accelerating * Time.deltaTime;
            }
            else
            {
                currentSpeed -= deaccelerating * Time.deltaTime;
            }

            //Turbo
            if (inputTurbo)
            {
                //Set speed to turbo speed and rotation to turbo values
                maxSpeed = turboSpeed;

               // currentYawSpeed = yawSpeed * yawTurboMultiplier;
                currentPitchSpeed = pitchSpeed * pitchTurboMultiplier;
                currentRollSpeed = rollSpeed * rollTurboMultiplier;

                //Effects
                ChangeWingTrailEffectThickness(trailThickness);
                _crowAnimator.SetGlideAnimation();
            }
            else
            {
                if (inputSlower)
                {
                    maxSpeed = defaultSpeed/4f;
                }
                else
                {
                    maxSpeed = defaultSpeed * speedMultiplier;
                }

             //   currentYawSpeed = yawSpeed;
                currentPitchSpeed = pitchSpeed;
                currentRollSpeed = rollSpeed;

                //Effects
                ChangeWingTrailEffectThickness(0f);
                _crowAnimator.SetFlyAnimation();

            }
        }

        #endregion

        #region Landing State

        public void AddLandingZone(LandingArea landingArea)
        {
            _currentLandingArea = landingArea;
        }

        //My trasform is runway landing adjuster child
        private void LandingUpdate()
        {
            if (_landed==false)
            {
                ChangeWingTrailEffectThickness(0f);
                
                currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime);
            
                transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, 
                    Vector3.up),Vector3.up);
                _crowAnimator.SetIdleAnimation();
                _landed = true;
            }
            else
            {
               // OnGroundInputHandle(); ground control to major tom
            }
        }

        private void OnGroundInputHandle()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _crowAnimator.SetWalkAnimation();
            }
            else if (Input.GetKey(KeyCode.W))
            {
                _currentLandingArea.OverrideContactPoint(transform.position);
                
                var rawDir = (_cam.transform.forward * inputV);
                rawDir = Vector3.ProjectOnPlane(rawDir, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(rawDir, Vector3.up),
                    Time.deltaTime * 50f);
                transform.position+=(transform.forward * Time.deltaTime * defaultSpeed);
            }
            else if(Input.GetKeyUp(KeyCode.W))
            {
                _crowAnimator.SetIdleAnimation();
            }
        }

        #endregion

        #region Takeoff State

        private void TakeoffUpdate()
        {
            _crowAnimator.SetFlyAnimation();
    
            // Reset colliders
            foreach (SimpleAirPlaneCollider _airPlaneCollider in airPlaneColliders)
            {
                _airPlaneCollider.collideSometing = false;
            }

            // Accelerate
            if (currentSpeed < turboSpeed)
            {
                currentSpeed += (accelerating * 2f) * Time.deltaTime;
            }
            
            Vector3 upwardDirection = Quaternion.Euler(-45, 0, 0) * Vector3.forward;
            Vector3 direction = transform.rotation * upwardDirection;

            // Move the crow
            transform.position += direction * currentSpeed * Time.deltaTime;

            // Check distance from landing area
            float distance = transform.position.y - _currentLandingArea.ContactPoint.y;
            Debug.LogWarning(distance);
            if (distance > takeoffLenght)
            {
                _currentLandingArea = null;
                airplaneState = AirplaneState.Flying;
                _landed = false;
            }
        }

        #endregion

        #region Audio
        private void AudioSystem()
        {
            if (airplaneState == AirplaneState.Flying)
            {
                
            }
            else if (airplaneState == AirplaneState.Landing)
            {
                
            }
            else if (airplaneState == AirplaneState.Takeoff)
            {
               
            }
        }

        #endregion

        #region Private methods
        
        private void SetupColliders(Transform _root)
        {
            if (_root == null)
                return;

            //Get colliders from root transform
            Collider[] colliders = _root.GetComponentsInChildren<Collider>();

            //If there are colliders put components in them
            for (int i = 0; i < colliders.Length; i++)
            {
                //Change collider to trigger
                colliders[i].isTrigger = true;

                GameObject _currentObject = colliders[i].gameObject;

                //Add airplane collider to it and put it on the list
                SimpleAirPlaneCollider _airplaneCollider = _currentObject.AddComponent<SimpleAirPlaneCollider>();
                airPlaneColliders.Add(_airplaneCollider);

                //Add airplane conroller reference to collider
                _airplaneCollider.controller = this;

                //Add rigid body to it
                Rigidbody _rb = _currentObject.AddComponent<Rigidbody>();
                _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
        }
        
        private void ChangeWingTrailEffectThickness(float _thickness)
        {
            for (int i = 0; i < wingTrailEffects.Length; i++)
            {
                wingTrailEffects[i].startWidth = Mathf.Lerp(wingTrailEffects[i].startWidth, _thickness, Time.deltaTime * 10f);
            }
        }

        private bool HitSometing()
        {
            for (int i = 0; i < airPlaneColliders.Count; i++)
            {
                if (airPlaneColliders[i].collideSometing)
                {
                    //Reset colliders
                    foreach(SimpleAirPlaneCollider _airPlaneCollider in airPlaneColliders)
                    {
                        _airPlaneCollider.collideSometing = false;
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Public methods

        public virtual void Crash()
        {
            //Invoke action
            crashAction?.Invoke();

            //Set rigidbody to non cinematic
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            //Add last speed to rb
            rb.AddForce(transform.forward * lastEngineSpeed, ForceMode.VelocityChange);

            //Change every collider trigger state and remove rigidbodys
            for (int i = 0; i < airPlaneColliders.Count; i++)
            {
                airPlaneColliders[i].GetComponent<Collider>().isTrigger = false;
                Destroy(airPlaneColliders[i].GetComponent<Rigidbody>());
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Returns a percentage of how fast the current speed is from the maximum speed between 0 and 1
        /// </summary>
        /// <returns></returns>
        public float PercentToMaxSpeed()
        {
            float _percentToMax = (currentSpeed * speedMultiplier) / turboSpeed;

            return _percentToMax;
        }

        public bool UsingTurbo()
        {
            if(maxSpeed == turboSpeed)
            {
                return true;
            }

            return false;
        }

        public float CurrentSpeed()
        {
            return currentSpeed * speedMultiplier;
        }
        
        /// <summary>
        /// With this you can adjust the default speed between one and zero
        /// </summary>
        /// <param name="_speedMultiplier"></param>
        public void ChangeSpeedMultiplier(float _speedMultiplier)
        {
            if(_speedMultiplier < 0f)
            {
                _speedMultiplier = 0f;
            }

            if(_speedMultiplier > 1f)
            {
                _speedMultiplier = 1f;
            }

            speedMultiplier = _speedMultiplier;
        }

        #endregion

        #region Inputs

        private void HandleInputs()
        {
            //Rotate inputs
            inputH = Input.GetAxis("Horizontal");
            inputV = Input.GetAxis("Vertical");
            inputSlower = Input.GetKey(KeyCode.Space);
            //Yaw axis inputs
            // inputYawLeft = Input.GetKey(KeyCode.Q);
            // inputYawRight = Input.GetKey(KeyCode.E);

            //Turbo
            inputTurbo = Input.GetKey(KeyCode.LeftShift);
        }

        #endregion
    }
}