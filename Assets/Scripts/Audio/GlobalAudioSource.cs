using System;
using System.Collections.Generic;
using DependencyControl;
using UnityEngine;

namespace Audio
{
    public enum AudioChannelType
    {
        Music,
        Sfx,
        UI,
        Ambient
    }
    
    public class GlobalAudioSource : DependencyMonoBehaviour
    {
        private readonly Dictionary<AudioChannelType, AudioChannel> _channels = new Dictionary<AudioChannelType, AudioChannel>();

        protected override void OnDestroy() {}

        protected override void Awake()
        {
            if (Dependency.Get<GlobalAudioSource>())
            {
                Destroy(gameObject);
                return;
            }
            
            base.Awake();
            
            DontDestroyOnLoad(gameObject);

            CreateChannel(AudioChannelType.Music);
            CreateChannel(AudioChannelType.Sfx);
            CreateChannel(AudioChannelType.UI);
            CreateChannel(AudioChannelType.Ambient);
        }

        private void CreateChannel(AudioChannelType type)
        {
            var channelObject = new GameObject($"AudioChannel_{type}");
            channelObject.transform.SetParent(transform);
    
            var channel = new AudioChannel(channelObject.transform);
            _channels[type] = channel;
        }
        
        private T WithChannel<T>(AudioChannelType type, Func<AudioChannel, T> action)
        {
            if (!_channels.TryGetValue(type, out var channel))
            {
                Debug.LogWarning($"AudioChannel [{type}] not found!");
                return default;
            }

            return action(channel);
        }

        public AudioSource Play(AudioChannelType type, AudioClip clip, bool loop = false, float volume = 1f)
            => WithChannel(type, channel => channel.Play(clip, loop, volume));

        public AudioSource PlayOneShot(AudioChannelType type, AudioClip clip, float volume = 1f)
            => WithChannel(type, channel => channel.PlayOneShot(clip, volume));

        public void Stop(AudioChannelType type)
        {
            if (_channels.TryGetValue(type, out var channel))
            {
                channel.StopAll();
            }
        }

        public void SetChannelVolume(AudioChannelType type, float volume)
        {
            if (_channels.TryGetValue(type, out var channel))
            {
                channel.SetChannelVolume(volume);
            }
        }
    }
}