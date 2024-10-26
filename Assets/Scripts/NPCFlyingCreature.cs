using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFlyingCreature : MonoBehaviour
{
    #region Private variables
        
        private float maxSpeed = 0.6f;
        private float speedMultiplier;
        private float currentPitchSpeed;
        private float currentRollSpeed;
        private float currentSpeed;
        private float currentEngineLightIntensity;
        
        private Rigidbody rb;
        
        private bool _landed;

        #endregion
        
        [SerializeField] private CrowAnimator _crowAnimator;
        
        [Header("Wing trail effects")]
        [Range(0.01f, 1f)]
        [SerializeField] private float trailThickness = 0.045f;
        [SerializeField] private TrailRenderer[] wingTrailEffects;
        
        [Range(5f, 500f)]
        [SerializeField] private float pitchSpeed = 100f;

        [Range(5f, 500f)]
        [SerializeField] private float rollSpeed = 200f;
        
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
    

        private void Start()
        {
            //Setup speeds
            maxSpeed = defaultSpeed;
            currentSpeed = defaultSpeed;
            ChangeSpeedMultiplier(1f);

            //Get and set rigidbody
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        private void Update()
        {
            AudioSystem();
           
        }

        #region Flying State
        

        #endregion
        
    
        #region Audio
        private void AudioSystem()
        {
        }

        #endregion
        
        
        private void ChangeWingTrailEffectThickness(float _thickness)
        {
            for (int i = 0; i < wingTrailEffects.Length; i++)
            {
                wingTrailEffects[i].startWidth = Mathf.Lerp(wingTrailEffects[i].startWidth, _thickness, Time.deltaTime * 10f);
            }
        }
        
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
    
    
}
