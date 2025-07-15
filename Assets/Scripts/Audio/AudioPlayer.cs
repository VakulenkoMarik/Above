using DependencyControl;
using UnityEngine;

namespace Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip clip;
        
        [Header("Settings")]
        [SerializeField] private bool loop;
        [SerializeField, Range(-1, 1)] private float volumeOffset;
        [SerializeField] private AudioChannelType channel;

        private GlobalAudioSource Source => _source ??= Dependency.Get<GlobalAudioSource>();
        private GlobalAudioSource _source;

        private AudioSource _localSource;
    
        public void Play() => _localSource = Source.Play(channel, clip, loop, volumeOffset);
        public void PlayOneShot() => _localSource = Source.PlayOneShot(channel, clip, volumeOffset);
        public void Pause() => _localSource?.Pause();
        
        public void Stop()
        {
            if (_localSource)
            {
                _localSource.Stop();
                _localSource.enabled = false;
            }
            
            _localSource = null;
        }
    }
}
