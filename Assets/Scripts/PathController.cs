using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine;
using UnityEngine.Splines;

public class PathController : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private Checkpoint _checkpointPrefab;
    [SerializeField] private List<Checkpoint> _checkpoints;
    private Checkpoint _currentCheckpoint;
    
    private void Start()
    {
        _checkpoints.ForEach(x=>x.OnEntered+=OnCheckpointEntered);
        _checkpoints.ForEach(x => x.gameObject.SetActive(false));
        _checkpoints[0].gameObject.SetActive(true);
        _currentCheckpoint = _checkpoints[0];
    }

    private void OnDisable()
    {
        _checkpoints.ForEach(x=>x.OnEntered-=OnCheckpointEntered);
    }

    private void OnCheckpointEntered(Checkpoint checkpoint)
    {
        if (_checkpoints.IndexOf(checkpoint)==_checkpoints.Count-1)
        {
            Debug.LogWarning("finish");
            
            FindObjectOfType<GameplayCanvas>().ShowFinalText(NpcPathFollower.s_FinishedCount==0, 
                NpcPathFollower.s_FinishedCount+1);
        }
        
        var index = _checkpoints.IndexOf(checkpoint)+1;
        if (index<_checkpoints.Count)
        {
            _checkpoints[index].gameObject.SetActive(true);
        }
    }

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
}
