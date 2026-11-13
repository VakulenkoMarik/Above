using System.Collections;
using System.Threading.Tasks;
using DependencyControl;
using Encrypting;
using Firebase;
using Firebase.Auth;
using Firebase.DataControl;
using Firebase.Storage;
using Other;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu.Settings.OtherSettings
{
    public class OtherSettings : MonoBehaviour
    {
        [SerializeField] private ControllerOtherSettings controller;

        [Header ("## Rename nickname : ")]
        [SerializeField] private TMP_InputField renameNickField;
        [SerializeField] private Button renameNickButton;
        [SerializeField] private GameObject renameNickPanel;
        [SerializeField] private TextMeshProUGUI renameNickCoinsCountText;
        
        private StorageData StorageData => _storageData ??= Dependency.Get<StorageData>();
        private StorageData _storageData;

        public void ShowLevelRanks(bool tog)
        {
            if (tog == true)
            {
                Dependency.Get<JsonStorage>().data.otherSettings.showLevelRanks = true;
            }
            else
            {
                Dependency.Get<JsonStorage>().data.otherSettings.showLevelRanks = false;
            }

            controller.ShowLevelRanks(tog);

            CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);
        }

        public void OpenDocumentation()
        {
            Application.OpenURL("https://docs.google.com/document/d/1ANJd7XmfpLubOtsssmON4Fuc_4nhoc4LQUoeockF8RY/edit?usp=sharing");
        }

        public void PlayTutorial()
        {
            SceneManager.LoadSceneAsync("Tutorial");
        }

        public void Logout()
        {
            SceneManager.LoadSceneAsync("Authentication");
        }

        public async void DeleteAccount()
        {
            await AccountDeleting();
        }

        async Task AccountDeleting()
        {
            await StorageData.DeleteUser();

            Debug.Log("Data is deleted");

            Dependency.Get<LoadingScreenActivator>().SetActiveLoadingScreen(false);
            SceneManager.LoadSceneAsync("Authentication");
        }

        public void OpenRanameNicknamePanel()
        {
            renameNickPanel.SetActive(true);

            if (Dependency.Get<JsonStorage>().data.userData.coinsS >= 5)
            {
                renameNickButton.interactable = true;
                renameNickCoinsCountText.color = Color.white;
            }
            else
            {
                renameNickButton.interactable = false;
                renameNickCoinsCountText.color = Color.red;
            }
        }

        public void RenameNick()
        {
            StartCoroutine(TryRenameNickname());
        }

        private IEnumerator TryRenameNickname()
        {
            if (Dependency.Get<JsonStorage>().data.userData.coinsS >= 5)
            {
                UserProfile profile = new UserProfile { DisplayName = renameNickField.text };

                var updateProfileTask = StorageData.GetAuth().CurrentUser.UpdateUserProfileAsync(profile);

                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if (updateProfileTask.Exception != null)
                {
                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;
                }
                else
                {
                    Dependency.Get<JsonStorage>().data.userData.coinsS -= 5;

                    StorageData.SaveJsonData();

                    SceneManager.LoadSceneAsync(1);
                }
            }
        }

        public void ActivateParticles(bool tog)
        {
            if (tog == true)
            {
                Dependency.Get<JsonStorage>().data.otherSettings.particles = true;
            }
            else
            {
                Dependency.Get<JsonStorage>().data.otherSettings.particles = false;
            }

            controller.ActivateParticles(tog);

            CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);
        }

        public void ShowTheSelectedBoost(bool tog)
        {
            if (tog == true)
            {
                Dependency.Get<JsonStorage>().data.otherSettings.showSelectedBoostInGame = true;
            }
            else
            {
                Dependency.Get<JsonStorage>().data.otherSettings.showSelectedBoostInGame = false;
            }

            controller.ShowTheSelectedBoost(tog);

            CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);
        }

        public void CameraShake(bool tog)
        {
            if (tog == true)
            {
                Dependency.Get<JsonStorage>().data.otherSettings.cameraShake = true;
            }
            else
            {
                Dependency.Get<JsonStorage>().data.otherSettings.cameraShake = false;
            }

            CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);
        }

        public void Vibration(bool tog)
        {
            if (tog == true)
            {
                Dependency.Get<JsonStorage>().data.otherSettings.vibration = true;
            }
            else
            {
                Dependency.Get<JsonStorage>().data.otherSettings.vibration = false;
            }

            CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);
        }
    }
}
