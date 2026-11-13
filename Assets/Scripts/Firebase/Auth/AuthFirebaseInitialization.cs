using System;
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

namespace Firebase.Auth
{
    public struct FirebaseInitializedData
    {
        public FirebaseApp App;
        public FirebaseStorage Storage;
        public FirebaseAuth Auth;
    }
    
    public class AuthFirebaseInitialization
    {
        public Action<FirebaseInitializedData> OnInitialized;
        
        private bool _isInitialized;
        public bool IsInitialized => _isInitialized;

        private FirebaseInitializedData _initializedData;
        
        public async void Initialize() {
            if (!_isInitialized) {
                await TryProcessDependencies();
            }
        }

        private async Task<bool> TryProcessDependencies() {
            try {
                await CheckAndFixDependenciesAsync();
                return true;
            }
            catch (Exception ex) {
                Debug.LogError(ex.Message);
                return false;
            }
        }

        private async Task CheckAndFixDependenciesAsync() {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
    
            if (dependencyStatus != DependencyStatus.Available) {
                throw new Exception($"Firebase dependencies not available: {dependencyStatus}");
            }

            InitializeApp();
            InitializeStorage();
            InitializeAuth();
            DoneInitialization();
        }

        private void InitializeApp() {
            AppOptions options = GetAppOptions();
            _initializedData.App = FirebaseApp.Create(options);
        }
        
        private void InitializeStorage() {
            _initializedData.Storage = FirebaseStorage.GetInstance(_initializedData.App, "gs://above-acb46.appspot.com");
        }

        private void InitializeAuth() {
            _initializedData.Auth = FirebaseAuth.DefaultInstance;
        }
        
        private void DoneInitialization() {
            OnInitialized.Invoke(_initializedData);
            OnInitialized = null;
            _isInitialized = true;
        }

        private AppOptions GetAppOptions() {
            AppOptions options = new AppOptions();

            options.ApiKey = "AIzaSyBBKdqnwmjChKTZrcJf9yluEj_vIuIOJLo";
            options.AppId = "1:222329630877:android:91e403f4a340caea3f2474";
            options.ProjectId = "above-acb46";
            options.StorageBucket = "above-acb46.appspot.com";
            options.DatabaseUrl = new Uri("https://above-acb46-default-rtdb.europe-west1.firebasedatabase.app/");

            return options;
        }
        
        public FirebaseInitializedData GetInitializedData() {
            if (!_isInitialized) {
                return new FirebaseInitializedData();
            }

            return _initializedData;
        }
    }
}