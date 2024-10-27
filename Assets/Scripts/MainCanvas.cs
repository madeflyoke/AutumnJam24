using System;
using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using TMPro;
using UniRx;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public event Action OnCountdownEnd;
    
    [SerializeField] private TMP_Text _normalControlText;
    [SerializeField] private TMP_Text _hardControlText;
    [SerializeField] private TMP_Text _countdownText;
    private int _currentCount;
    
    private void Start()
    {
        _normalControlText.gameObject.SetActive(GameplayHandler._Difficulty==0);
        _hardControlText.gameObject.SetActive(GameplayHandler._Difficulty!=0);
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
}
