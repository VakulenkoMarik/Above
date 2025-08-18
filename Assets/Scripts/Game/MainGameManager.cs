using System.Collections;
using Audio;
using DependencyControl;
using Firebase.DataControl;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class MainGameManager : MonoBehaviour
    {
        [Header ("## Audio :")]
        [SerializeField] private AudioSource[] SFX;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        [HideInInspector]
        public AudioSource audioSource;

        private bool _isReady = false;
    
        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            JsonStorage storage = Dependency.Get<JsonStorage>();

            musicSlider.value = storage.data.audioSettings.musicGame;
            sfxSlider.value = storage.data.audioSettings.sfxGame;

            audioSource.volume = musicSlider.value;

            float sfxVolume = storage.data.audioSettings.sfxGame;

            foreach (var source in SFX) {
                source.volume = sfxVolume;
            }

            _isReady = true;
        
            StartCoroutine(GameMusicOn());
        }

        private IEnumerator GameMusicOn()
        {
            yield return new WaitForSeconds(0.5f);

            audioSource.enabled = true;
        }

        public void SaveAudioSettings()
        {
            if (_isReady)
            {
                AudioControl.SetAudioSetting(musicSlider.value, "musicGame");
                AudioControl.SetAudioSetting(sfxSlider.value, "sfxGame");

                AudioControl.SetAudioValue(musicSlider, audioSource);
                AudioControl.SetAudioValue(sfxSlider, SFX);
            }
        }
    }
}
