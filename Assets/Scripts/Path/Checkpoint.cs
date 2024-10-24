using System;
using Entities.Interfaces;
using UnityEngine;

namespace Path
{
    public class Checkpoint : MonoBehaviour
    {
        public event Action<Checkpoint> OnPlayerEntered;
    
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
                OnPlayerEntered?.Invoke(this);
            }
        }
    }
}
