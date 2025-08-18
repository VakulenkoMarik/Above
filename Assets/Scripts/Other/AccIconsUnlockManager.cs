using DependencyControl;
using Encrypting;
using Firebase.DataControl;
using Firebase.Storage;
using Game;
using Hero;
using MainMenu;
using UnityEngine;

namespace Other
{
    public class AccIconsUnlockManager : MonoBehaviour
    {
        [SerializeField] private string iconName;
        [SerializeField] private bool isNeedToBuy;

        void Start()
        {
            if (isNeedToBuy)
            {
                bool isBought = Dependency.Get<JsonStorage>().data.icons.icons.Contains(iconName);

                if (isBought)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void Unlock()
        {
            Dependency.Get<JsonStorage>().data.icons.icons.Add(iconName);

            EquipAccIcon.instance.CheckLock();
        }

        public void BuyIconWithCoinsF(int price)
        {
            if (CoinsManagerInMainMenu.instance.coinsF >= price)
            {
                Dependency.Get<JsonStorage>().data.icons.icons.Add(iconName);

                gameObject.SetActive(false);

                EquipAccIcon.instance.CheckLock();
            
                CoinsManagerInMainMenu.instance.coinsF -= price;
                Dependency.Get<JsonStorage>().data.userData.coinsF = CoinsManagerInMainMenu.instance.coinsF;

                CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);

                CoinsManagerInMainMenu.instance.UpdateUI();
            }
        }

        public void BuyIconWithCoinsS(int price)
        {
            if (CoinsManagerInMainMenu.instance.coinsS >= price)
            {
                Dependency.Get<JsonStorage>().data.icons.icons.Add(iconName);

                gameObject.SetActive(false);

                EquipAccIcon.instance.CheckLock();
            
                CoinsManagerInMainMenu.instance.coinsS -= price;
                Dependency.Get<JsonStorage>().data.userData.coinsS = CoinsManagerInMainMenu.instance.coinsS;

                CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);

                CoinsManagerInMainMenu.instance.UpdateUI();
            }
        }
    }
}