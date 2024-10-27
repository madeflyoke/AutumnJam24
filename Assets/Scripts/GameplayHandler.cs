using System;
using System.Collections.Generic;
using EasyButtons;
using HeneGames.Airplane;
using Main.Scripts.Audio;
using UnityEngine;

public class GameplayHandler : MonoBehaviour
{
    public enum Difficulty
    {
        Normal =0,
        Hard = 1,
    }
    
    public static GameplayHandler Instance { get; private set; }
    public static Difficulty _Difficulty { get; private set; } = 0;

    [field:SerializeField] public SimpleAirCreatureController Player { get; private set; }
    [SerializeField] private List<NpcPathFollower> _enemies;
    [SerializeField] private GameplayCanvas gameplayCanvas;

    public void Awake()
    {
        if (Instance!=null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        _enemies.ForEach(x =>
        {
            x.Initialize();
        });
        gameplayCanvas.OnCountdownEnd += Launch;
        gameplayCanvas.StartCountdown();
    }

    public static void SetDifficulty(Difficulty d)
    {
        _Difficulty = d;
    }

    [Button]
    private void Launch()
    {
        gameplayCanvas.OnCountdownEnd -= Launch;

        AudioController.Instance.PlayClipAsMusic(_Difficulty==0? SoundType.NORMALMUSIC: SoundType.HARDMUSIC);
        
        _enemies.ForEach(x =>
        {
            x.FollowPath();
        });
        
        Player.Initialize();
    }
}
