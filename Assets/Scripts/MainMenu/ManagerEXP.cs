using System;
using System.Collections.Generic;
using DependencyControl;
using Encrypting;
using Firebase.DataControl;
using Firebase.Storage;
using Other;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WifiControl;

namespace MainMenu
{
    [Serializable]
    public struct LevelValue
    {
        public int Level;
        public int CountToNextLevel;
        public string Title;
    }

    public class ManagerEXP : MonoBehaviour
    {
        [SerializeField] private Slider expSlider;
        [SerializeField] private TextMeshProUGUI countToNextLevelText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI titleText;

        [SerializeField] private TextMeshProUGUI hightScore;
        [SerializeField] private List<LevelValue> keyFrame = new List<LevelValue>();

        private int countToNextLevel = 0;

        private int exp;
        private int level;
    
        void Start()
        {
            if (WIFIChecking.instance != null)
            {
                WIFIChecking.instance.ChangeSceneName();
            }
        
            if (Dependency.Get<JsonStorage>().data.userData.level >= 20)
            {
                bool isBought = Dependency.Get<JsonStorage>().data.icons.icons.Contains("exp");

                if (isBought == false)
                {
                    Dependency.Get<JsonStorage>().data.icons.icons.Add("exp");
                    EquipAccIcon.instance.CheckLock();
                }
            }

            hightScore.text = "" + Dependency.Get<JsonStorage>().data.userData.record;

            GetValues();

            SetValues();
            CheckNextLevel();

            if (level == 0)
            {
                Dependency.Get<JsonStorage>().isDataException = true;
                Dependency.Get<LoadingScreenActivator>().SetActiveLoadingScreen(true);
                Dependency.Get<LoadingScreenActivator>().SetMessage("Data loading error. Please re-enter the game");
            }
        }

        public void GetValues()
        {
            exp = Dependency.Get<JsonStorage>().data.userData.exp;
            level = Dependency.Get<JsonStorage>().data.userData.level;
        }

        private void SetValues()
        {
            foreach (LevelValue valueLevel in keyFrame)
            {
                if (valueLevel.Level > level)
                {
                    countToNextLevelText.text = exp + " / " +  valueLevel.CountToNextLevel;
                    levelText.text = level + "";

                    expSlider.maxValue = valueLevel.CountToNextLevel;
                    expSlider.value = exp;

                    titleText.text = valueLevel.Title;

                    countToNextLevel = (int)expSlider.maxValue;

                    return;
                }
            }
        }

        public void CheckNextLevel()
        {
            while (exp >= countToNextLevel)
            {
                exp -= countToNextLevel;
                level++;
                Dependency.Get<JsonStorage>().data.userData.exp -= countToNextLevel;
                Dependency.Get<JsonStorage>().data.userData.level++;

                CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);

                SetValues();
            }

            SetValues();
        }
    }
}