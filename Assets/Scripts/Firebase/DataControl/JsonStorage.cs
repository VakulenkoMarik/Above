using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DependencyControl;
using Encrypting;
using Firebase.Storage;
using Other;
using UnityEngine;

namespace Firebase.DataControl
{
    public class JsonStorage : DependencyMonoBehaviour
    {
        private int _seconds = 0;
        private bool _isCanCheck = true;

        [Header ("## Json file :")]
        public DataControl.Data data;
        
        [Header("## References :")]
        public string password;
        public bool isDataException = false;
        
        private LoadingScreenActivator LoadingScreenActivator => _loadingScreenActivator ??= Dependency.Get<LoadingScreenActivator>();
        private LoadingScreenActivator _loadingScreenActivator;
        
        private StorageData StorageData => _storageData ??= Dependency.Get<StorageData>();
        private StorageData _storageData;

        protected override void Awake() {
            if (Dependency.Get<JsonStorage>()) {
                Destroy(gameObject);
                return;
            }
            
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "data.json");

            if (!File.Exists(filePath) || StorageData.GetAuth() == null || isDataException || data.userData.level == 0)
            {
                Handheld.Vibrate();
                return;
            }

            SaveEncryptDataOnExit();
        }

        private void FixedUpdate()
        {
            if (isDataException)
            {
                LoadingScreenActivator.SetActiveLoadingScreen(true);
                LoadingScreenActivator.SetMessage("Data loading error.\nPlease re-enter the game");
                return;
            }

            if (_seconds == 5 && _isCanCheck && !isDataException && password != null && data.userData.level != 0)
            {
                CryptoHelper.Encrypt(data, password);
                StorageData.SaveJsonData();

                _seconds = 0;

                _isCanCheck = false;
            }
        }

        public async Task StartAction(FirebaseStorage storageReference) {
            StorageData.SetStorage(storageReference);
            password = CryptoHelper.GenerateKeyFromUid(StorageData.GetAuth().CurrentUser.UserId);
            LoadingScreenActivator.SetMessage("Checking the availability of a file");

            await CheckJson();
        }

        public async Task CheckJson()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "data.json");

            if (!File.Exists(filePath))
            {
                try
                {
                    if (StorageData.GetAuth().CurrentUser != null)
                    {
                        if (await StorageData.CheckIfJsonDataExists() == false)
                        {
                            LoadingScreenActivator.SetMessage("Creating new json data");

                            data = CreateNewJsonData();

                            SaveEncryptDataOnExit();
                            StartTimer();

                            return;
                        }
                        
                        LoadingScreenActivator.SetMessage("Loading json data");

                        data = await StorageData.LoadJsonData<DataControl.Data>();

                        while (!File.Exists(filePath))
                        {
                            await Task.Delay(100);
                        }

                        await Task.Delay(1000);

                        CryptoHelper.Encrypt(data, password);

                        return;
                    }
                    
                    LoadingScreenActivator.SetMessage("User exception");
                
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    LoadingScreenActivator.SetMessage(e.ToString());
                }
            
            }

            while (!File.Exists(filePath))
            {
                await Task.Delay(1000);
            }

            data = CryptoHelper.LoadAndDecrypt<DataControl.Data>(filePath, password);

            LoadingScreenActivator.SetMessage("Enabling add-ons");

            if (data.userData.level == 0)
            {
                isDataException = true;
                LoadingScreenActivator.SetActiveLoadingScreen(true);
                LoadingScreenActivator.SetMessage("Data loading error.\nPlease re-enter the game");
                return;
            }

            StartTimer();
        }

        private void SaveEncryptDataOnExit()
        {
            if (StorageData.GetAuth().CurrentUser != null)
            {
                string password = CryptoHelper.GenerateKeyFromUid(StorageData.GetAuth().CurrentUser.UserId);
            
                StorageData.SaveJsonData();
                CryptoHelper.Encrypt(data, password);
            }
        }

        private DataControl.Data CreateNewJsonData()
        {
            return new DataControl.Data
            {
                boolean = new DataControl.Data.Boolean
                {
                    isTutorial = false
                },

                userData = new DataControl.Data.UserData
                {
                    userIcon = "blackThrush",

                    exp = 0,
                    level = 1,
                    coinsS = 0,
                    coinsF = 0,

                    coinsFAllTime = 0,
                    coinsSAllTime = 0
                },

                purchasedItems = new DataControl.Data.PurchasedItems
                {
                    skins = new List<string>()
                    {
                        "blackThrush"
                    },

                    bgs = new List<string>()
                    {
                        "street"
                    },

                    boosts = new List<string>()
                    {
                        "none"
                    },
                },

                currentShop = new DataControl.Data.CurrentShop
                {
                    currentSkin = 0,
                    currentBg = 0,
                    currentBoost = 0
                },

                audioSettings = new DataControl.Data.AudioSettings
                {
                    musicMainMenu = 0.5f,
                    musicGame = 0.5f,
                    sfxMainMenu = 1,
                    sfxGame = 1
                },

                otherSettings = new DataControl.Data.OtherSettings
                {
                    showLevelRanks = true,
                    particles = true,
                    showSelectedBoostInGame = true,
                    cameraShake = true,
                    vibration = true
                },

                icons = new DataControl.Data.Icons
                {
                    icons = new List<string>()
                    {
                        "blackThrush",
                        "chimney"
                    }
                }
            };
        }

        private void AddSeconds()
        {
            _isCanCheck = true;
            _seconds++;
        }

        private void StartTimer()
        {
            InvokeRepeating(nameof(AddSeconds), 1, 1);
        }

        public void CancelTimer()
        {
            CancelInvoke(nameof(AddSeconds));
        }
    }
}