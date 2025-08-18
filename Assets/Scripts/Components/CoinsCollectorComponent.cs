using System;
using DependencyControl;
using Encrypting;
using Firebase.DataControl;
using Firebase.Storage;
using Game;
using UnityEngine;

namespace Components
{
    public class CoinsCollectorComponent : MonoBehaviour
    {
        public Action OnCoinCollected;
        
        private JsonStorage Storage => _jsonStorage ??= Dependency.Get<JsonStorage>();
        private JsonStorage _jsonStorage;

        private int _multiplier = 1;

        private void Start() {
            _multiplier = Storage.data.currentShop.currentBoost == 1 ? 2 : 1;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.TryGetComponent<Coin>(out var coin)) {
                CollectCoin(coin);
            }
        }

        private void CollectCoin(Coin coin) {
            coin.PickUp();
            coin.ApplyToUser(ref Storage.data.userData, _multiplier);
            
            OnCoinCollected?.Invoke();
            CryptoHelper.Encrypt(Storage.data, Storage.password);
        }
    }
}