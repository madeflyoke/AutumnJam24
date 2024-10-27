using System;
using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    public event Action OnCountdownEnd;
    
    [SerializeField] private TMP_Text _normalControlText;
    [SerializeField] private TMP_Text _hardControlText;
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Button _pauseMenuButtonToMenu;
    private int _currentCount;
    
    private void Start()
    {
        _normalControlText.gameObject.SetActive(GameplayHandler._Difficulty==0);
        _hardControlText.gameObject.SetActive(GameplayHandler._Difficulty!=0);
        _pauseMenuButtonToMenu.onClick.AddListener(()=>
        {
            _pauseMenuButtonToMenu.onClick.RemoveAllListeners();
            Pause(false);
            GameStateChanger.Instance.CallOnMainMenu();
        });
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
