using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Entities.Interfaces;
using Managers;
using Managers.Enums;
using Path;
using UnityEngine;
using UnityEngine.Splines;
using Zenject;
using Random = UnityEngine.Random;

namespace Entities
{
    public class NpcPathFollower : MonoBehaviour, IRaceParticipant
    {
        private struct SpeedModes
        {
            public float StartSpeedMultiplier;
            public float MidSpeedMultiplier;
            public float EndSpeedMultiplier;
        }
    
        public Color MarkerColor { get; private set; }
        
        [SerializeField] private SplineContainer _path;
        [SerializeField] private float _speed;
        [SerializeField] private CrowAnimator _crowAnimator;
        [SerializeField] private List<MeshRenderer> _markersRenderers;
        private Tween _tween;
        private float _currentSpeed;
        private SpeedModes _speedModes;
        private int _currentPathIndex;
        private Vector3[] _concretePath;
        private GameplayHandler _gameplayHandler;

        [Inject]
        public void Construct(GameplayHandler gameplayHandler)
        {
            _gameplayHandler = gameplayHandler;
            _gameplayHandler.Initialized += Initialize;
            _gameplayHandler.GameplayStart += FollowPath;
        }
    
        private void Initialize(Difficulty _)
        {
            _gameplayHandler.Initialized -= Initialize;

            MarkerColor = new Color(Random.value, Random.value, Random.value);
        
            _markersRenderers.ForEach(x=>x.material.color = MarkerColor * 2f);
            SetupPath();
            SetupSpeedModes();
        }

        private void SetupSpeedModes()
        {
            _speedModes = new SpeedModes();
            _speedModes.StartSpeedMultiplier = Random.Range(0.8f, 1.2f);
            _speedModes.MidSpeedMultiplier = Random.Range(0.7f, 0.9f);
            _speedModes.EndSpeedMultiplier = Random.Range(0.7f, 1.1f);
        }

        private void SetupPath()
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
        
            _concretePath = _path.Splines[0].Knots.Select(
                x => (Vector3)x.Position+_path.transform.position + RandomVector(7f)).ToArray();
            transform.position = _concretePath[0];
            transform.forward = _concretePath[1] - transform.position;
        }
    
        private void FollowPath()
        {
            _gameplayHandler.GameplayStart -= FollowPath;

            _crowAnimator.SetFlyAnimation();
            _tween=transform.DOPath(_concretePath, _speed)
                .SetOptions(false,lockRotation: AxisConstraint.X)
                .OnWaypointChange(OnWaypointChanged)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .SetLookAt(0.01f, up:Vector3.up)
                .OnComplete(()=>_gameplayHandler.PathController.RegisterFinisher(this));
        }

        private void OnWaypointChanged(int index)
        {
            _currentPathIndex = index;
            if (_currentPathIndex<(int)(_concretePath.Length*0.3f))
            {
                SetSpeed(_speedModes.StartSpeedMultiplier);
            }
            else if (_currentPathIndex<(int)(_concretePath.Length*0.6f))
            {
                SetSpeed(_speedModes.MidSpeedMultiplier);
            }
            else
            {
                SetSpeed(_speedModes.EndSpeedMultiplier);
            }
        }

        private void SetSpeed(float multiplier)
        {
            _tween.timeScale = multiplier;
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }
    }
}
