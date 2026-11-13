using DependencyControl;
using Firebase.DataControl;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.Select
{
    public class SelectAccIcon : MonoBehaviour
    {
        [SerializeField] private GameObject blockedPanel;
        public string iconName;
        public Image button;

        void Start()
        {
            iconName = GetComponent<Image>().sprite.name;

            CheckLock();

            if (iconName == Dependency.Get<JsonStorage>().data.userData.userIcon)
            {
                Change();
            }
        }

        public void CheckLock()
        {
            bool isBought = Dependency.Get<JsonStorage>().data.icons.icons.Contains(iconName);

            if (isBought == true)
            {
                blockedPanel.SetActive(false);
            }
        }

        public void Change()
        {
            EquipAccIcon.instance.Change(iconName, button);
        }
    }
}
