using System;
using Managers;
using Managers.Enums;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class GameplayCanvas : MonoBehaviour
    {
        public event Action OnCountdownEnd;
    
        [SerializeField] private TMP_Text _normalControlText;
        [SerializeField] private TMP_Text _hardControlText;
        [SerializeField] private TMP_Text _countdownText;
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private Button _pauseMenuButtonToMenu;
        [SerializeField] private TMP_Text _finalText;
        private int _currentCount;
        private GameplayHandler _gameplayHandler;
        private GameStateChanger _gameStateChanger;
        
        [Inject]
        public void Construct(GameplayHandler gameplayHandler, GameStateChanger gameStateChanger)
        {
            _gameplayHandler = gameplayHandler;
            _gameplayHandler.Initialized += Initialize;
            _gameStateChanger = gameStateChanger;
        }
        
        private void Initialize(Difficulty difficulty)
        {
            _gameplayHandler.Initialized -= Initialize;
            
            _normalControlText.gameObject.SetActive(difficulty==0);
            _hardControlText.gameObject.SetActive(difficulty!=0);
            _pauseMenuButtonToMenu.onClick.AddListener(()=>
            {
                _pauseMenuButtonToMenu.onClick.RemoveAllListeners();
                Pause(false);
                _gameStateChanger.CallOnMainMenu();
            });
            _finalText.gameObject.SetActive(false);

            _gameplayHandler.PathController.PlayerFinished += HandlePlayerFinish;
        }

        private void HandlePlayerFinish(int place)
        {
            if (place==0)
            {
                ShowFinalWinnerText();
            }
            else
            {
                ShowFinalPlaceText(place+1);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_pauseMenu.activeSelf)
                {
                    _pauseMenu.gameObject.SetActive(false);
                    Pause(false);
                }
                else
                {
                    _pauseMenu.gameObject.SetActive(true);
                    Pause(true);
                }
            }
        }

        private void ShowFinalPlaceText(int place)
        {
            _finalText.text = $"{place}th Place! Well Done!";
            _finalText.gameObject.SetActive(true);
        }
        
        private void ShowFinalWinnerText()
        {
            _finalText.text = "You Win! Congratulations!";
            _finalText.gameObject.SetActive(true);
        }
    
        private void Pause(bool value)
        {
            if (value)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    

        public void StartCountdown()
        {
            _countdownText.gameObject.SetActive(true);
            _currentCount = 5;
            _countdownText.text = _currentCount.ToString();
            Observable.Interval(TimeSpan.FromSeconds(1)).TakeWhile(_ => _currentCount >= 0).Subscribe(x =>
            {
                _currentCount--;
                if (_currentCount==-1)
                {
                    OnCountdownEnd?.Invoke();
                    _countdownText.gameObject.SetActive(false);
                    return;
                }
                _countdownText.text = Mathf.Clamp(_currentCount, 0, 5).ToString();
            }).AddTo(this);
        }

        private void OnDisable()
        {
            _pauseMenuButtonToMenu.onClick.RemoveAllListeners();
        }
    }
}
