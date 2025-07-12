using UnityEngine;

namespace Audio
{
    public class AudioChannel
    {
        private readonly AudioSource _source;

        public float ChannelVolume { get; private set; } = 1;

        public AudioChannel(Transform parent)
        {
            _source = parent.gameObject.AddComponent<AudioSource>();
        }

        public AudioSource Play(AudioClip clip, bool loop = false, float volume = 1f)
        {
            _source.clip = clip;
            _source.loop = loop;
            SetSourceVolume(_source, volume);
            _source.Play();
            
            return _source;
        }
        
        public AudioSource PlayOneShot(AudioClip clip, float volume = 1f)
        {
            SetSourceVolume(_source, volume);
            _source.PlayOneShot(clip);
            
            return _source;
        }

        public void StopAll()
        {
            _source.Stop();
        }

        private void SetSourceVolume(AudioSource source, float customVolume)
        {
            source.volume = Mathf.Clamp01(customVolume + ChannelVolume);
        }

        public void SetChannelVolume(float volume)
        {
            ChannelVolume = Mathf.Clamp01(volume);
        }
    }
}