using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using EasyButtons;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateChanger : MonoBehaviour
{
    public static event Action GameplayStarted;
    
    public static GameStateChanger Instance { get; private set; }
    private CancellationTokenSource _cts;
    
    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            return;
        }
        Destroy(this);
    }

    [Button]
    public async void StartGameScene()
    {
        _cts = new CancellationTokenSource();
        await SceneManager.LoadSceneAsync(1).ToUniTask().AttachExternalCancellation(_cts.Token);
        GameplayStarted?.Invoke();
    }

    public void CallOnMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
    }
}
