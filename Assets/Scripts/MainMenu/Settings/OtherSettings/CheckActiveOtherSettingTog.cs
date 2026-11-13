using DependencyControl;
using Firebase.DataControl;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.Settings.OtherSettings
{
    public class CheckActiveOtherSettingTog : MonoBehaviour
    {
        [SerializeField] private Toggle tog;
        [SerializeField] private OtherSetting setting;

        enum OtherSetting
        {
            ShowLevelRank,
            Particles,
            ShowTheSelectedBoost,
            CameraShake,
            Vibration
        }


        void Awake()
        {
            switch (setting)
            {
                case OtherSetting.ShowLevelRank:
                    tog.isOn = Dependency.Get<JsonStorage>().data.otherSettings.showLevelRanks;
                    break; 
                case OtherSetting.Particles:
                    tog.isOn = Dependency.Get<JsonStorage>().data.otherSettings.particles;
                    break;
                case OtherSetting.ShowTheSelectedBoost:
                    tog.isOn = Dependency.Get<JsonStorage>().data.otherSettings.showSelectedBoostInGame;
                    break;
                case OtherSetting.CameraShake:
                    tog.isOn = Dependency.Get<JsonStorage>().data.otherSettings.cameraShake;
                    break;
                case OtherSetting.Vibration:
                    tog.isOn = Dependency.Get<JsonStorage>().data.otherSettings.vibration;
                    break;
            }
        }
    }
}
