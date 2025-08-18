using DependencyControl;
using Encrypting;
using Firebase.DataControl;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public static class AudioControl
    {
        private static JsonStorage Storage => _jsonStorage ??= Dependency.Get<JsonStorage>();
        private static JsonStorage _jsonStorage;
        
        public static void SetAudioValue(Slider slider, AudioSource[] audioSources)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.volume = slider.value;
            }
        }

        public static void SetAudioValue(Slider slider, AudioSource audioSource)
        {
            audioSource.volume = slider.value;
        }

        public static void SetAudioValue(Slider[] sliders, AudioSource[] audioSources)
        {
            for (int i = 0; i < audioSources.Length; i++)
            {
                if (sliders[i] != null)
                {
                    audioSources[i].volume = sliders[i].value;
                }
            }
        }

        public static void SetAudioSetting(float value, string setting)
        {
            switch (setting)
            {
                case "musicMainMenu":
                    Storage.data.audioSettings.musicMainMenu = value;
                    break;
                case "sfxMainMenu":
                    Storage.data.audioSettings.sfxMainMenu = value;
                    break;
                case "musicGame":
                    Storage.data.audioSettings.musicGame = value;
                    break;
                case "sfxGame":
                    Storage.data.audioSettings.sfxGame = value;
                    break;
            }

            CryptoHelper.Encrypt(Storage.data, Storage.password);
        }
    }
}
