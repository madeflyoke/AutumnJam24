using Cinemachine;
using Managers;
using Managers.Enums;
using UnityEngine;
using Zenject;
using static Entities.SimpleAirCreatureController;

namespace Entities
{
    public class SimpleAirCreatureCamera : MonoBehaviour
    {
        private CinemachineBrain brain;

        [Header("References")]
        [SerializeField] private SimpleAirCreatureController airCreatureController;

        [SerializeField] private CinemachineFreeLook freeLook;

        [Header("Camera values")]
        [SerializeField] private float cameraDefaultFov = 50f;

        [SerializeField] private float cameraHardDefaultFov = 60f;
        private float cameraTurboFov;
        private GameplayHandler _gameplayHandler;
        private bool _isInitialized;
        
        [Inject]
        public void Construct(GameplayHandler gameplayHandler)
        {
            _gameplayHandler = gameplayHandler;
            _gameplayHandler.Initialized += SetupCameraMode;
        }

        private void SetupCameraMode(Difficulty difficulty)
        {
            if (difficulty==0)
            {
                freeLook.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            }
            else
            {
                freeLook.m_BindingMode = CinemachineTransposer.BindingMode.LockToTarget;
                cameraDefaultFov = cameraHardDefaultFov;
            }
            cameraTurboFov = cameraDefaultFov + 10f;
            _isInitialized = true;
        }

        private void Start()
        {
            brain = GetComponent<CinemachineBrain>();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            freeLook.enabled = Application.isFocused;
            CameraFovUpdate();
        }


        private void CameraFovUpdate()
        {
            if (_isInitialized)
            {
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
        }
        

        public void ChangeCameraFov(float _fov)
        {
            float _deltatime = Time.deltaTime * 100f;
            freeLook.m_Lens.FieldOfView = Mathf.Lerp(freeLook.m_Lens.FieldOfView, _fov, 0.05f * _deltatime);
        }
    }
}