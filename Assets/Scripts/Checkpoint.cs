using System;
using Lean.Pool;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public event Action<Checkpoint> OnEntered;
    
    private const string PlayerTag = "Player";

    [SerializeField] private ParticleSystem _disableEffect;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(PlayerTag))
        {
            var scale = _disableEffect.transform.localScale;
            _disableEffect.transform.SetParent(null);
            _disableEffect.transform.localScale = scale;
            _disableEffect.Play();
            
            gameObject.SetActive(false);
            OnEntered?.Invoke(this);
        }
    }
}
