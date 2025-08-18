using System.Collections.Generic;
using DependencyControl;
using Encrypting;
using Firebase.DataControl;
using Firebase.Storage;
using MainMenu.Select;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class EquipAccIcon : MonoBehaviour
    {
        public static EquipAccIcon instance;

        [SerializeField] private List<SelectAccIcon> equipButtonsList;
        [SerializeField] private AccountManagerMainMenu accountManagerMainMenu;

        void Start()
        {
            instance = this;
        }

        public void CheckLock()
        {
            foreach (SelectAccIcon select in equipButtonsList)
            {
                select.CheckLock();
            }
        }

        public void Change(string name, Image button)
        {
            Dependency.Get<JsonStorage>().data.userData.userIcon = name;

            foreach (SelectAccIcon select in equipButtonsList)
            {
                Color currentColor = select.button.color;
                currentColor.a = 0f;
                select.button.color = currentColor;
            }

            Color color = button.color;
            color.a = 0.5f;
            button.color = color;

            accountManagerMainMenu.ChangeIcon(name);

            CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);
        }
    }
}
