using DependencyControl;
using Encrypting;
using Firebase.DataControl;
using Firebase.Storage;
using Game;
using Hero;
using Other;
using UnityEngine;

namespace Shop.OnlineShop
{
    internal enum Coin
    {
        FlyCoins,
        SuperCoins
    }

    enum Think
    {
        Skin,
        Bg,
        Boost
    }

    public class DataBuySystem : MonoBehaviour
    {
        [SerializeField] private Coin coinType;
        [SerializeField] private Think purchaseType;
        [SerializeField] private int price;
        [SerializeField] private string purchaseName;

        void Start()
        {
            if (purchaseType == Think.Skin)
            {
                bool isBought = Dependency.Get<JsonStorage>().data.purchasedItems.skins.Contains(purchaseName);

                if (isBought == true)
                {
                    gameObject.SetActive(false);
                }
            }
            if (purchaseType == Think.Bg)
            {
                bool isBought = Dependency.Get<JsonStorage>().data.purchasedItems.bgs.Contains(purchaseName);

                if (isBought == true)
                {
                    gameObject.SetActive(false);
                }
            }
            if (purchaseType == Think.Boost)
            {
                bool isBought = Dependency.Get<JsonStorage>().data.purchasedItems.boosts.Contains(purchaseName);

                if (isBought == true)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void Buy()
        {
            if (coinType == Coin.FlyCoins)
            {
                if (CoinsManagerInMainMenu.instance.coinsF >= price)
                {
                    if (purchaseType == Think.Skin)
                    {
                        Dependency.Get<JsonStorage>().data.purchasedItems.skins.Add(purchaseName);
                    }
                    else if (purchaseType == Think.Bg)
                    {
                        Dependency.Get<JsonStorage>().data.purchasedItems.bgs.Add(purchaseName);
                    }
                    else if (purchaseType == Think.Boost)
                    {
                        Dependency.Get<JsonStorage>().data.purchasedItems.boosts.Add(purchaseName);
                    }

                    if (GetComponent<AccIconsUnlockManager>())
                    {
                        GetComponent<AccIconsUnlockManager>().Unlock();
                    }

                    gameObject.SetActive(false);

                    CoinsManagerInMainMenu.instance.coinsF -= price;
                    Dependency.Get<JsonStorage>().data.userData.coinsF = CoinsManagerInMainMenu.instance.coinsF;

                    CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);

                    CoinsManagerInMainMenu.instance.UpdateUI();
                }
            }
            else if (coinType == Coin.SuperCoins)
            {
                if (CoinsManagerInMainMenu.instance.coinsS >= price)
                {
                    if (purchaseType == Think.Skin)
                    {
                        Dependency.Get<JsonStorage>().data.purchasedItems.skins.Add(purchaseName);
                    }
                    else if (purchaseType == Think.Bg)
                    {
                        Dependency.Get<JsonStorage>().data.purchasedItems.bgs.Add(purchaseName);
                    }
                    else if (purchaseType == Think.Boost)
                    {
                        Dependency.Get<JsonStorage>().data.purchasedItems.boosts.Add(purchaseName);
                    }

                    if (GetComponent<AccIconsUnlockManager>())
                    {
                        GetComponent<AccIconsUnlockManager>().Unlock();
                    }

                    gameObject.SetActive(false);

                    CoinsManagerInMainMenu.instance.coinsS -= price;
                    Dependency.Get<JsonStorage>().data.userData.coinsS = CoinsManagerInMainMenu.instance.coinsS;

                    CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);
                
                    CoinsManagerInMainMenu.instance.UpdateUI();
                }
            }
        }
    }
}