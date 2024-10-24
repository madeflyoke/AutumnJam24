using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using EasyButtons;
using Entities;
using Entities.Interfaces;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.Splines;
using Zenject;

namespace Path
{
    public class PathController : MonoBehaviour
    {
        public event Action<int> PlayerFinished;

        [Inject] private AudioController _audioController;
        [Inject] private GameplayHandler _gameplayHandler;
        
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private Checkpoint _checkpointPrefab;
        [SerializeField] private List<Checkpoint> _checkpoints;
        private Checkpoint _currentCheckpoint;
        private List<RaceParticipantFinisher> _finishers = new List<RaceParticipantFinisher>();

        private void Start()
        {
            _checkpoints.ForEach(x=>x.OnPlayerEntered+=OnPlayerCheckpointEntered);
            _checkpoints.ForEach(x => x.gameObject.SetActive(false));
            _checkpoints[0].gameObject.SetActive(true);
            _currentCheckpoint = _checkpoints[0];
        }

        private void OnDisable()
        {
            _checkpoints.ForEach(x=>x.OnPlayerEntered-=OnPlayerCheckpointEntered);
        }

        private void OnPlayerCheckpointEntered(Checkpoint checkpoint)
        {
            if (_checkpoints.IndexOf(checkpoint)==_checkpoints.Count-1)
            {
                RegisterFinisher(_gameplayHandler.Player);
                PlayerFinished?.Invoke(_finishers.FirstOrDefault(x=>x.Participant==_gameplayHandler.Player).Place);
            }
        
            _audioController.PlayClip(SoundType.RINGTOUCH, 0.2f);
        
            var index = _checkpoints.IndexOf(checkpoint)+1;
            if (index<_checkpoints.Count)
            {
                _checkpoints[index].gameObject.SetActive(true);
            }
        }

        public void RegisterFinisher(IRaceParticipant participant)
        {
            if (_finishers.Any(x=>x.Participant==participant))
            {
                return;
            }
            var place = _finishers.Count();
            _finishers.Add(new RaceParticipantFinisher()
            {
                Participant = participant,
                Place = place
            });
        }
        
        private class RaceParticipantFinisher
        {
            public IRaceParticipant Participant;
            public int Place;
        }
        
#if UNITY_EDITOR

        [Button]
        public void SetupCheckpoints()
        {
            _checkpoints.Clear();
            foreach (var knot in _splineContainer.Splines[0].Knots.Select(x=>x))
            {
                var pos = (Vector3)knot.Position;
                var rot = knot.Rotation;
                var instance = Instantiate(_checkpointPrefab, pos, rot);
                instance.transform.SetParent(transform);
                _checkpoints.Add(instance);
            }
        }
        
#endif
    }
}
