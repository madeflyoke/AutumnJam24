using HeneGames.Airplane;
using UnityEngine;

public class GameplayHandler : MonoBehaviour
{
    public static GameplayHandler Instance { get; private set; }
    
    [field:SerializeField] public SimpleAirCreatureController Player { get; private set; }
    [SerializeField] private NpcPathFollower _enemies;

    public void Awake()
    {
        if (Instance!=null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
}
