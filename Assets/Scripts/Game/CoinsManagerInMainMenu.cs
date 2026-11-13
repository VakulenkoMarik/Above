using DependencyControl;
using Firebase.DataControl;
using Firebase.Storage;
using TMPro;
using UnityEngine;

namespace Game
{
    public class CoinsManagerInMainMenu : MonoBehaviour
    {
        private static JsonStorage Storage => _jsonStorage ??= Dependency.Get<JsonStorage>();
        private static JsonStorage _jsonStorage;
        
        public static CoinsManagerInMainMenu instance;

        public int coinsF;
        public int coinsS;

        [SerializeField] private TextMeshProUGUI SuperCoinsText;
        [SerializeField] private TextMeshProUGUI FlyCoinsText;
        [SerializeField] private TextMeshProUGUI SuperCoinsInShopText;
        [SerializeField] private TextMeshProUGUI FlyCoinsInShopText;
        [SerializeField] private TextMeshProUGUI AdsForSupercoinsCount;

        private void Start()
        {
            instance = this;

            coinsF = Storage.data.userData.coinsF;
            coinsS = Storage.data.userData.coinsS;

            UpdateUI();
        }

        public void UpdateUI()
        {
            coinsF = Storage.data.userData.coinsF;
            coinsS = Storage.data.userData.coinsS;
        
            SuperCoinsText.text = coinsS + "";
            FlyCoinsText.text = coinsF + "";
            SuperCoinsInShopText.text = coinsS + "";
            FlyCoinsInShopText.text = coinsF + "";
        }
    }
}