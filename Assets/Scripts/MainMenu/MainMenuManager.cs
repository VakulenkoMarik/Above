using Audio;
using DependencyControl;
using Firebase.DataControl;
using Firebase.Storage;
using Other;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header ("## Audio settings :")]
        public Slider[] sliders;
        public AudioSource[] SFX;

        private AudioSource musicMainMenu;
        private bool isReady = false;

        void Start()
        {
            musicMainMenu = Dependency.Get<LoadingScreenActivator>().gameObject.GetComponent<AudioSource>();

            LoadAudioValue();
        }

        private void LoadAudioValue()
        {
            sliders[0].value = Dependency.Get<JsonStorage>().data.audioSettings.musicMainMenu;
            sliders[1].value = Dependency.Get<JsonStorage>().data.audioSettings.sfxMainMenu;
            sliders[2].value = Dependency.Get<JsonStorage>().data.audioSettings.musicGame;
            sliders[3].value = Dependency.Get<JsonStorage>().data.audioSettings.sfxGame;

            musicMainMenu.enabled = true;

            isReady = true;
        }

        public void SaveAudioSettings()
        {
            if (isReady)
            {
                AudioControl.SetAudioSetting(sliders[0].value, "musicMainMenu");
                AudioControl.SetAudioSetting(sliders[1].value, "sfxMainMenu");
                AudioControl.SetAudioSetting(sliders[2].value, "musicGame");
                AudioControl.SetAudioSetting(sliders[3].value, "sfxGame");

                AudioControl.SetAudioValue(sliders[0], musicMainMenu);
                AudioControl.SetAudioValue(sliders[1], SFX);
            }
        }
    }
}
