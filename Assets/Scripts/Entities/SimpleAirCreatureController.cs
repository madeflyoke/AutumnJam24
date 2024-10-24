using System;
using System.Collections.Generic;
using Entities.Interfaces;
using Managers;
using Managers.Enums;
using UnityEngine;
using Zenject;

namespace Entities
{
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleAirCreatureController : MonoBehaviour, IRaceParticipant
    {
        public enum AirplaneState
        {
            Flying,
            Landing,
            Takeoff,
        }

        #region Private variables
        
        private float maxSpeed = 0.6f;
        private float speedMultiplier;
        private float currentYawSpeed;
        private float currentPitchSpeed;
        private float currentRollSpeed;
        private float currentSpeed;
        private float currentEngineLightIntensity;
        
        private Rigidbody rb;
        private LandingArea _currentLandingArea;

        //Input variables
        private float inputH;
        private float inputV;
        private bool inputTurbo;
        private bool inputSlower;
        private bool inputYawLeft;
        private bool inputYawRight;

        private bool _landed;

        #endregion

        public AirplaneState airplaneState;

        [SerializeField] private CrowAnimator _crowAnimator;
        [SerializeField] private SimpleAirCreatureCamera _cam;
        
        [Header("Wing trail effects")]
        [Range(0.01f, 1f)]
        [SerializeField] private float trailThickness = 0.045f;
        [SerializeField] private TrailRenderer[] wingTrailEffects;

        [Header("Rotating speeds")]
        [Range(5f, 500f)]
        [SerializeField] private float yawSpeed = 50f;

        [Range(5f, 500f)]
        [SerializeField] private float pitchSpeed = 100f;

        [Range(5f, 500f)]
        [SerializeField] private float rollSpeed = 200f;

        [Header("Rotating speeds multiplers when turbo is used")]
        [Range(0.1f, 5f)]
        [SerializeField] private float yawTurboMultiplier = 0.3f;

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

        private bool _canControl;
        private GameplayHandler _gameplayHandler;
        private Difficulty _difficulty;
        
        [Inject]
        public void Construct(GameplayHandler gameplayHandler)
        {
            _gameplayHandler = gameplayHandler;
            _gameplayHandler.GameplayStart += Launch;
            _gameplayHandler.Initialized += Initialize;
        }
        
        private void Initialize(Difficulty difficulty)
        {
            _gameplayHandler.Initialized -= Initialize;

            maxSpeed = defaultSpeed;
            currentSpeed = defaultSpeed;
            ChangeSpeedMultiplier(1f);

            rb = GetComponent<Rigidbody>();
            _difficulty = difficulty;
        }

        private void Launch()
        {
            _gameplayHandler.GameplayStart -= Launch;
            _canControl = true;
        }

        private void Update()
        {
            if (_canControl)
            {
                HandleInputs();
            }
        }

        private void FixedUpdate()
        {
            if (_canControl)
            {
                FlyingUpdate();
            }
        }

        #region Flying State

        private void FlyingUpdate()
        {
            Movement();
            SidewaysForceCalculation();
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
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
            rb.velocity += transform.forward * currentSpeed;
            
            transform.Rotate(Vector3.forward * -inputH * currentRollSpeed * Time.deltaTime);
            transform.Rotate(Vector3.right * inputV * currentPitchSpeed * Time.deltaTime);

            if (inputYawRight)
            {
                transform.Rotate(Vector3.up * currentYawSpeed * Time.deltaTime);
            }
            else if (inputYawLeft)
            {
                transform.Rotate(-Vector3.up * currentYawSpeed * Time.deltaTime);
            }

            if (currentSpeed < maxSpeed)
            {
                currentSpeed += accelerating * Time.deltaTime;
            }
            else
            {
                currentSpeed -= deaccelerating * Time.deltaTime;
            }

            if (inputTurbo)
            {
                maxSpeed = turboSpeed;

                currentYawSpeed = yawSpeed * yawTurboMultiplier;
                currentPitchSpeed = pitchSpeed * pitchTurboMultiplier;
                currentRollSpeed = rollSpeed * rollTurboMultiplier;
                
                if (wingTrailEffects[0].gameObject.activeSelf==false)
                {
                    foreach (var tr in wingTrailEffects)
                    {
                        tr.gameObject.SetActive(true);
                    }
                }
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

                currentYawSpeed = yawSpeed;
                currentPitchSpeed = pitchSpeed;
                currentRollSpeed = rollSpeed;

                ChangeWingTrailEffectThickness(0f);
                _crowAnimator.SetFlyAnimation();

            }
        }

        #endregion
        
        public void AddLandingZone(LandingArea landingArea)
        {
            _currentLandingArea = landingArea;
        }
        
        private void ChangeWingTrailEffectThickness(float _thickness)
        {
            for (int i = 0; i < wingTrailEffects.Length; i++)
            {
                wingTrailEffects[i].startWidth = Mathf.Lerp(wingTrailEffects[i].startWidth, _thickness, Time.deltaTime * 10f);
            }
        }
        
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
        
        private void HandleInputs()
        {
            inputV = Input.GetAxis("Vertical");
            inputSlower = Input.GetKey(KeyCode.Space);

            if (_difficulty==0)
            {
                inputH = 0;
                inputYawLeft = Input.GetKey(KeyCode.A);
                inputYawRight = Input.GetKey(KeyCode.D);
            }
            else
            {
                inputH = Input.GetAxis("Horizontal");
                inputYawLeft =false;
                inputYawRight = false;
            }
            
            inputTurbo = Input.GetKey(KeyCode.LeftShift);
        }
    }
}