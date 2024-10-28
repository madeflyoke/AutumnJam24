using System;
using System.Collections.Generic;
using Audio;
using Entities;
using Managers.Enums;
using Path;
using UI;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class GameplayHandler : MonoBehaviour
    {
        [Inject] private AudioController _audioController;

        public event Action<Difficulty> Initialized;
        public event Action GameplayStart;
        
        [field:SerializeField] public SimpleAirCreatureController Player { get; private set; }
        [field: SerializeField] public PathController PathController { get; private set; } 
        [SerializeField] private GameplayCanvas _gameplayCanvas;
        private Difficulty _currentDifficulty;
        private GameStateChanger _gameStateChanger;

        [Inject]
        public void Construct(GameStateChanger gameStateChanger)
        {
            _gameStateChanger = gameStateChanger;
            _gameStateChanger.GameplaySceneLoaded += Initialize;
        }
        
        private void Initialize(Difficulty difficulty)
        {
            _gameStateChanger.GameplaySceneLoaded -= Initialize;

            _currentDifficulty = difficulty;
            Initialized?.Invoke(_currentDifficulty);
            
            _gameplayCanvas.OnCountdownEnd += Launch;
            _gameplayCanvas.StartCountdown();
        }

        private void Launch()
        {
            _gameplayCanvas.OnCountdownEnd -= Launch;

            _audioController.PlayClipAsMusic(_currentDifficulty==0? SoundType.NORMALMUSIC: SoundType.HARDMUSIC);
            GameplayStart?.Invoke();
        }
    }
}
