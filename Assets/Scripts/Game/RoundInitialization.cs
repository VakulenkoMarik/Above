using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using DependencyControl;
using Firebase.Storage;
using Hero;
using UnityEngine;

namespace Game
{
    public class RoundInitialization : DependencyMonoBehaviour
    {
        [SerializeField] private Transform characterStartSpawnPoint;

        [SerializeField] private List<Character> characters;
        [SerializeField] private List<GameObject> locations;
        
        private Character _currentCharacter;
        private GameObject _currentLocation;

        private JsonStorage Storage => _jsonStorage ??= Dependency.Get<JsonStorage>();
        private JsonStorage _jsonStorage;

        private void Start() {
            CharacterInitialisation();
            LocationInitialisation();
        }

        private void CharacterInitialisation() {
            AssignCharacter();
            Instantiate(_currentCharacter, characterStartSpawnPoint);
        }

        private void AssignCharacter() {
            int indexOfSelectedCharacter = Storage.data.currentShop.currentSkin;
            try {
                _currentCharacter = characters.First(c => c.Index.Equals(indexOfSelectedCharacter));
            }
            catch (Exception) {
                Debug.LogWarning("Character not found");
                throw;
            }
        }
        
        private void LocationInitialisation() {
            AssignLocation();
            _currentLocation.SetActive(true);
        }

        private void AssignLocation() {
            int indexOfSelectedLocation = Storage.data.currentShop.currentBg;
            try {
                _currentLocation = locations[indexOfSelectedLocation];
            }
            catch (Exception) {
                Debug.LogWarning("Location not found");
                throw;
            }
        }

        public Character GetCurrentCharacter() {
            return _currentCharacter;
        }
        
        public GameObject GetCurrentLocation() {
            return _currentLocation;
        }
    }
}