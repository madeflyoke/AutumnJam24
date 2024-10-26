using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HeneGames.Airplane;
using UnityEngine;

public class NpcPathFollower : MonoBehaviour
{
    private static SimpleAirCreatureController _player;
    
    [SerializeField] private List<Transform> _path;
    [SerializeField] private float _speed;
    [SerializeField] private CrowAnimator _crowAnimator;
    [SerializeField] private float _playerDistanceThreshold;
    private Tween _tween;

    private void Start()
    {
        if (_player==null)
        {
            _player = FindObjectOfType<SimpleAirCreatureController>();
        }
        FollowPath();
    }

    public void FollowPath()
    {
        transform.position = _path[0].position;
        transform.forward = _path[1].position - transform.position;
        _crowAnimator.SetFlyAnimation();
        var path = _path.Select(x=>x.position).ToArray();
        _tween=transform.DOPath(path, _speed, PathType.CatmullRom).SetOptions(false,lockRotation: AxisConstraint.X)
            .SetSpeedBased(true).SetEase(Ease.Linear).SetLookAt(0.01f, up:Vector3.up);
    }

    private void Update() //Path along Z axis
    {
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

    public void SetSpeed(float multiplier)
    {
        _tween.timeScale = multiplier;
    }
}
