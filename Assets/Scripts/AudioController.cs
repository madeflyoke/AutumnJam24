using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;

namespace Main.Scripts.Audio
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance { get; private set; }
        public float SoundsVolume => _soundsVolume;
        
        [SerializeField] private AudioSource _audioSourcePrefab;
        [SerializeField, Range(0.01f,1f)] private float _soundsVolume;
        [SerializeField] private List<ClipBySoundType> _clips;
        [SerializeField] private AudioSource _mainMusicSource;
        [SerializeField] private AudioSource _externalNoiseSource;
        
        private float _defaultMusicVolume;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                return;
            }
            Destroy(gameObject);
        }
        
        public void PlayClipAsMusic(SoundType soundType)
        {
            var audioSource = _mainMusicSource;
            audioSource.clip = _clips.FirstOrDefault(x=>x.SoundType==soundType).Clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        public void PlayClip(SoundType soundType, float customVolume = 0f, float customPitch = 1f)
        {
            var audioSource = LeanPool.Spawn(_audioSourcePrefab);
            audioSource.clip = _clips.FirstOrDefault(x=>x.SoundType==soundType).Clip;
            audioSource.volume = customVolume!=0f? customVolume: _soundsVolume;
            audioSource.pitch = customPitch;
            LeanPool.Despawn(audioSource, audioSource.clip.length);
            audioSource.Play();
        }

        [Serializable]
        private struct ClipBySoundType
        {
            public SoundType SoundType;
            public AudioClip Clip;
        }
    }
    
}
