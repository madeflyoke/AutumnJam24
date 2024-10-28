using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Managers.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameStateChanger : MonoBehaviour
    {
        public event Action<Difficulty> GameplaySceneLoaded;
    
        private CancellationTokenSource _cts;

        public async void StartGameScene(Difficulty difficulty)
        {
            _cts = new CancellationTokenSource();
            await SceneManager.LoadSceneAsync(1).ToUniTask().AttachExternalCancellation(_cts.Token);
            GameplaySceneLoaded?.Invoke(difficulty);
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
}
