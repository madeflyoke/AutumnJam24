using System;
using System.Collections.Generic;
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

    [field: SerializeField] public Difficulty _Difficulty { get; private set; } = 0;
    [field:SerializeField] public SimpleAirCreatureController Player { get; private set; }
    [SerializeField] private List<NpcPathFollower> _enemies;

    public void Awake()
    {
        if (Instance!=null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        AudioController.Instance.PlayClipAsMusic(_Difficulty==0? SoundType.NORMALMUSIC: SoundType.HARDMUSIC);
        
        _enemies.ForEach(x =>
        {
            x.Initialize();
            x.FollowPath();
        });
    }
}
