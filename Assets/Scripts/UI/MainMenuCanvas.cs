using Audio;
using Managers;
using Managers.Enums;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class MainMenuCanvas : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _normalModeButton;
        [SerializeField] private Button _hardModeButton;
        [Inject] private AudioController _audioController;
        [Inject] private GameStateChanger _gameStateChanger;
        
        public void Awake()
        {
            _normalModeButton.gameObject.SetActive(false);
            _hardModeButton.gameObject.SetActive(false);
            _playButton.onClick.AddListener(() =>
            {
                _normalModeButton.gameObject.SetActive(true);
                _hardModeButton.gameObject.SetActive(true);
                _playButton.gameObject.SetActive(false);
            });
            _normalModeButton.onClick.AddListener(()=>OnModeSelected(Difficulty.Normal));
            _hardModeButton.onClick.AddListener(()=>OnModeSelected(Difficulty.Hard));
        
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Start()
        {
            _audioController.StopMainMusic();
        }

        private void OnModeSelected(Difficulty difficulty)
        {
            RemoveListeners();
            _gameStateChanger.StartGameScene(difficulty);
        }

        private void RemoveListeners()
        {
            _normalModeButton.onClick.RemoveAllListeners();
            _hardModeButton.onClick.RemoveAllListeners();
            _playButton.onClick.RemoveAllListeners();
        }
    
        private void OnDisable()
        {
            RemoveListeners();
        }
    }
}
