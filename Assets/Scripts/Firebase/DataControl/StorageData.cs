using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using DependencyControl;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Storage;
using Other;
using UnityEngine;
using UnityEngine.Networking;

namespace Firebase.DataControl
{
    public class StorageData : DependencyMonoBehaviour
    {
        private static JsonStorage Storage => _jsonStorage ??= Dependency.Get<JsonStorage>();
        private static JsonStorage _jsonStorage;

        private StorageReference _reference;
        private FirebaseStorage _storage;

        private FirebaseAuth _authUser;

        private TaskCompletionSource<bool> _downloadTaskCompletionSource;

        private LoadingScreenActivator LoadingScreen => _loadingScreen ??= Dependency.Get<LoadingScreenActivator>();
        private LoadingScreenActivator _loadingScreen;

        protected override void Awake() {
            if (Dependency.Get<StorageData>()) {
                Destroy(gameObject);
                return;
            }
        
            base.Awake();
        }

        public void SetStorage(FirebaseStorage storage) {
            _storage = storage;
            _reference = _storage.RootReference.Child(_authUser.CurrentUser.UserId).Child("data");
        }

        public async Task DeleteUser() {
            await DeleteFolderAsync();
        }

        public async Task DeleteFolderAsync() {
            LoadingScreen.SetActiveLoadingScreen(true);

            StorageReference storageRef = _storage.GetReferenceFromUrl("gs://above-acb46.appspot.com");
            StorageReference folderRef = storageRef.Child(_authUser.CurrentUser.UserId);

            await folderRef.DeleteAsync().ContinueWith(async task => {
                if (task.IsCompleted) {
                    Debug.Log("Folder deleted successfully.");
                    await DeleteUserAsync();
                }
                else if (task.IsFaulted) {
                    Debug.LogError("Failed to delete folder: " + task.Exception);
                    LoadingScreen.SetActiveLoadingScreen(false);
                }
            });
        }

        private async Task DeleteUserAsync() {
            FirebaseUser user = _authUser.CurrentUser;
            if (user != null) {
                await user.DeleteAsync().ContinueWith(task => {
                    if (task.IsCanceled) {
                        Debug.LogError("DeleteAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted) {
                        Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                        return;
                    }

                    Debug.Log("User deleted successfully.");
                });
            }
        }

        public void SaveJsonData() {
            string local_file = Path.Combine(Application.persistentDataPath, "gameDataTemp.json");
            string local_file_uri = $"{Uri.UriSchemeFile}://{local_file}";

            string jsonDataTemp = JsonUtility.ToJson(Storage.data, true);
            File.WriteAllText(local_file, jsonDataTemp);
        
            if (File.Exists(local_file) && _authUser.CurrentUser != null) {
                try {
                    _reference.PutFileAsync(local_file_uri).ContinueWith(task => {
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;
                    });
                }
                catch (StorageException e) {
                    LoadingScreen.SetActiveLoadingScreen(true);
                    LoadingScreen.SetMessage(e.HttpResultCode.ToString());
                }
            }
        }

        public async void DeleteJson() {
            if (Storage != null) {
                Storage.CancelTimer();
            }

            string filePath = Path.Combine(Application.persistentDataPath, "gameDataTemp.json");

            await DeletingFileSignOut(filePath);
        }

        public async Task DeletingFileSignOut(string path) {
            if (File.Exists(path)) {
                try {
                    await SaveJsonAsync();
                
                    await Task.Delay(1000);

                    if (IsFileAvailableForDeletion(path)) {
                        File.Delete(path);
                        Debug.Log("File is deleted: " + path);
                    }
                    else {
                        Debug.Log("File is not available for deletion: " + path);
                    }
                }
                catch (Exception e) {
                    Debug.Log("Delete file exception: " + e.Message);
                }
            }

            Debug.Log("Signed out");
        }

        private bool IsFileAvailableForDeletion(string path) {
            try {
                using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                return true;
            }
            catch (IOException) {
                return false;
            }
        }

        private async Task SaveJsonAsync() {
            string local_file = Path.Combine(Application.persistentDataPath, "gameDataTemp.json");
            string local_file_uri = string.Format("{0}://{1}", Uri.UriSchemeFile, local_file);

            string jsonDataTemp = JsonUtility.ToJson(Storage.data, true);
            File.WriteAllText(local_file, jsonDataTemp);

            using (StreamWriter writer = new StreamWriter(local_file, false)) {
                await writer.WriteAsync(jsonDataTemp);
            }
        
            if (File.Exists(local_file) && Dependency.Get<FirebaseAuthSystem>().User != null) {
                try {
                    var task = _reference.PutFileAsync(local_file_uri);
                    await task;

                    StorageMetadata metadata = task.Result;
                    string md5Hash = metadata.Md5Hash;
                }
                catch (StorageException e) {
                    Debug.Log(e);
                }
            }
        }

        public void DeletingFile(string path) {
            if (File.Exists(path)) {
                try {
                    File.Delete(path);

                    Debug.Log("File is deleted: " + path);
                }
                catch (Exception e) {
                    Debug.Log("Delete file exeption: " + e.Message);
                }
            }
        }

        public async Task<T> LoadJsonData<T>() {
            string filePath = Path.Combine(Application.persistentDataPath, "data.json");

            if (!await CheckIfJsonDataExists()) {
                Debug.Log("File does not exist in storage.");
                return default;
            }

            _downloadTaskCompletionSource = new TaskCompletionSource<bool>();

            var downloadUrlTask = _reference.GetDownloadUrlAsync();
            await downloadUrlTask.ContinueWithOnMainThread(task => {
                if (!task.IsFaulted && !task.IsCanceled) {
                    string downloadUrl = task.Result.ToString();
                    StartCoroutine(DownloadFile(downloadUrl, filePath));
                }
                else {
                    Debug.LogError("Error getting download URL: " + task.Exception);
                }
            });

            await _downloadTaskCompletionSource.Task;

            string jsonData = File.ReadAllText(filePath);

            T loadedData = JsonUtility.FromJson<T>(jsonData);

            return loadedData;
        }

        private IEnumerator DownloadFile(string downloadUrl, string filePath) {
            using UnityWebRequest www = UnityWebRequest.Get(downloadUrl);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) {
                File.WriteAllBytes(filePath, www.downloadHandler.data);

                _downloadTaskCompletionSource.TrySetResult(true);
            }
            else {
                Debug.LogError("Failed to download file: " + www.error);
            }
        }

        public async Task<bool> CheckIfJsonDataExists() {
            try {
                await _reference.GetMetadataAsync();
                return true;
            }
            catch (StorageException) {
                return false;
            }
        }

        public void SetAuth(FirebaseAuth authUser) {
            _authUser = authUser;
        }
        
        public FirebaseAuth GetAuth() {
            return _authUser;
        }
    }
}