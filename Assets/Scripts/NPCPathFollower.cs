using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HeneGames.Airplane;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class NpcPathFollower : MonoBehaviour
{
    [SerializeField] private SplineContainer _path;
    [SerializeField] private float _speed;
    [SerializeField] private CrowAnimator _crowAnimator;
    [SerializeField] private float _playerDistanceThreshold;
    private Tween _tween;
    private SimpleAirCreatureController _player;

    private void Start()
    {
        _player = GameplayHandler.Instance.Player;
        FollowPath();
    }

    public void FollowPath()
    {
        Vector3 RandomVector(float range)
        {
            float GenerateRandomValue(float range)
            {
                int sign = Random.Range(0, 2) == 0 ? 1 : -1;

                float value = Random.Range(range-2f, range) * sign;

                return value;
            }
            
            return new Vector3(GenerateRandomValue(range),
                GenerateRandomValue(range/2), 
                GenerateRandomValue(range));
        }
        
        var path = _path.Splines[0].Knots.Select(
            x => (Vector3)x.Position+_path.transform.position + RandomVector(7f)).ToArray();
        
        transform.position = path[0];
        transform.forward = path[1] - transform.position;
        _crowAnimator.SetFlyAnimation();
        _tween=transform.DOPath(path, _speed).SetOptions(false,lockRotation: AxisConstraint.X)
            .SetSpeedBased(true).SetEase(Ease.Linear).SetLookAt(0.01f, up:Vector3.up);
    }

    private void Update() //Path along Z axis
    {
        return;
        var dot = Vector3.Dot(_player.transform.forward, (_player.transform.position - transform.position).normalized);
        
        var outOfDistance = Vector3.Distance(_player.transform.position, transform.position) > _playerDistanceThreshold;

        if (outOfDistance)
        {
            if (dot<0)
            {
                SetSpeed(.8f);
            }
            else if (dot>0)
            {
                SetSpeed(1.5f);
            }
            return;
        }
        SetSpeed(1);
    }

    private void SetSpeed(float multiplier)
    {
        _tween.timeScale = multiplier;
    }
}
