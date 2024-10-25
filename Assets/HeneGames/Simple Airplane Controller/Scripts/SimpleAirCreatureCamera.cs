using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static HeneGames.Airplane.SimpleAirCreatureController;

namespace HeneGames.Airplane
{
    public class SimpleAirCreatureCamera : MonoBehaviour
    {
        private CinemachineBrain brain;

        [Header("References")]
        [SerializeField] private SimpleAirCreatureController airCreatureController;
        [SerializeField] private CinemachineFreeLook freeLook;

        [Header("Camera values")]
        [SerializeField] private float cameraDefaultFov = 60f;
        [SerializeField] private float cameraTurboFov = 40f;
        
        private void OnEnable()
        {
            airCreatureController.crashAction += Crash;
        }

        private void OnDisable()
        {
            airCreatureController.crashAction -= Crash;
        }

        private void Start()
        {
            brain = GetComponent<CinemachineBrain>();

            //Lock and hide mouse
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            CameraFovUpdate();
        }

        private void CameraFovUpdate()
        {
            //Turbo
            if(airCreatureController.airplaneState == AirplaneState.Flying)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    ChangeCameraFov(cameraTurboFov);
                }
                else
                {
                    ChangeCameraFov(cameraDefaultFov);
                }
            }
            else
            {
                ChangeCameraFov(cameraDefaultFov);
            }
        }
        

        public void ChangeCameraFov(float _fov)
        {
            float _deltatime = Time.deltaTime * 100f;
            freeLook.m_Lens.FieldOfView = Mathf.Lerp(freeLook.m_Lens.FieldOfView, _fov, 0.05f * _deltatime);
        }

        private void Crash()
        {
            //Change update method after crash
            brain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
        }
    }
}