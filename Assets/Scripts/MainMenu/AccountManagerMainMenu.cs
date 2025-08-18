using System;
using System.Collections.Generic;
using DependencyControl;
using Firebase.DataControl;
using Firebase.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    [Serializable]
    public struct Icons
    {
        public Sprite profileIcon;
    }

    public class AccountManagerMainMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI userName;
        [SerializeField] private Image profileImage;

        [SerializeField] private List<Icons> icons = new List<Icons>();
    
        void Start()
        {
            userName.text = Dependency.Get<StorageData>().GetAuth().CurrentUser.DisplayName;

            string equipedIcon = Dependency.Get<JsonStorage>().data.userData.userIcon;

            foreach (Icons i in icons)
            {
                if (i.profileIcon.name == equipedIcon)
                {
                    profileImage.sprite = i.profileIcon;

                    return;
                }
            }
        }

        public void ChangeIcon(string iconName)
        {
            foreach (Icons i in icons)
            {
                if (i.profileIcon.name == iconName)
                {
                    profileImage.sprite = i.profileIcon;

                    return;
                }
            }
        }
    }
}