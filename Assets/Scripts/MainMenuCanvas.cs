using System;
using Main.Scripts.Audio;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _normalModeButton;
    [SerializeField] private Button _hardModeButton;

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
        _normalModeButton.onClick.AddListener(()=>OnModeSelected(GameplayHandler.Difficulty.Normal));
        _hardModeButton.onClick.AddListener(()=>OnModeSelected(GameplayHandler.Difficulty.Hard));
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        AudioController.Instance.StopMainMusic();
    }

    public void OnModeSelected(GameplayHandler.Difficulty difficulty)
    {
        RemoveListeners();
        GameplayHandler.SetDifficulty(difficulty);
        GameStateChanger.Instance.StartGameScene();
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
