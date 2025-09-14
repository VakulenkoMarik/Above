using System.IO;
using DependencyControl;
using Firebase.Auth;
using Firebase.DataControl;
using Firebase.Storage;
using Other;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Firebase.Frontend
{
    public class AuthenticationUiActionsSender : MonoBehaviour
    {
        [Header("--Panels--")]
        [SerializeField] private GameObject authPanel;
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registrationPanel;
        [SerializeField] private GameObject emailVerificationPanel;
        [SerializeField] private GameObject wifiErrorScreen;
        
        [Header("--Texts--")]
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private TextMeshProUGUI emailVerificationText;
        
        [Header("--Buttons--")]
        [SerializeField] private Button playOnlineButton;
        [SerializeField] private Button logoutButton;
        
        [Header("--Other--")]
        [SerializeField] private GameObject preLoginLoadingIcon;

        public void SetActiveWifiErrorScreen(bool activate) {
            wifiErrorScreen.SetActive(activate);
        }
        
        public void OpenGmail() {
            Application.OpenURL("https://mail.google.com");
        }

        public void SetErrorMessage(string error) {
            errorText.text = error;
        }

        public void DisposeAuthenticationPanel() {
            loginPanel.SetActive(false);
            registrationPanel.SetActive(false);
            emailVerificationPanel.SetActive(false);
            errorText.text = "";
        }

        public void OpenLoginPanel() {
            string filePath = Path.Combine(Application.persistentDataPath, "data.json");
        
            Dependency.Get<StorageData>().DeletingFile(filePath);

            DisposeAuthenticationPanel();
        
            authPanel.SetActive(true);
            loginPanel.SetActive(true);
        }

        public void OpenRegistrationPanel() {
            DisposeAuthenticationPanel();
            authPanel.SetActive(true);
            registrationPanel.SetActive(true);
        }

        public void CloseAuthPanel() {
            DisposeAuthenticationPanel();
            authPanel.SetActive(false);
        }

        public void ActivateButtons() {
            DisposeAuthenticationPanel();

            if (Dependency.Get<FirebaseAuthSystem>() != null) {
                if (Dependency.Get<FirebaseAuthSystem>().User != null && Dependency.Get<FirebaseAuthSystem>().User.IsEmailVerified) {
                    logoutButton.interactable = true;
                }
            }

            playOnlineButton.interactable = true;
        }

        public void ShowVerificationResponse(bool isEmailSent, string emailId, string errorMessage = "") {
            DisposeAuthenticationPanel();
            emailVerificationPanel.SetActive(true);

            if(isEmailSent) {
                emailVerificationText.text += emailId;
            }
            else {
                emailVerificationText.text = $"Couldn't sent email : {errorMessage}";
            }
        }

        public void OpenDocumentation() {
            Application.OpenURL("https://docs.google.com/document/d/1ANJd7XmfpLubOtsssmON4Fuc_4nhoc4LQUoeockF8RY/edit?usp=sharing");
        } 
        
        public void PlayOffline() {
            SceneLoader.OpenOfflineMenuScene();
        }

        public void AddListenerToOnlineButton(UnityAction action) {
            playOnlineButton.onClick.AddListener(action);
        }

        public void ActivatePreLoginLoadingIcon() {
            preLoginLoadingIcon.SetActive(true);
        }
        
        public void DeactivatePreLoginLoadingIcon() {
            preLoginLoadingIcon.SetActive(false);
        }
    }
}