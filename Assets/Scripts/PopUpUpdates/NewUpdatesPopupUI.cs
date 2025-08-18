using System.Collections;
using DependencyControl;
using Firebase;
using Firebase.Auth;
using Other;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace PopUpUpdates
{
    struct GameData
    {
        public string Description;
        public string Version;
        public string Url;
        public string TechnicalBreak;
    }

    public class NewUpdatesPopupUI : MonoBehaviour
    {
        public static NewUpdatesPopupUI instance;

        [Header ("## UI References :")]
        [SerializeField] GameObject updateCanvas;
        [SerializeField] GameObject techCanvas;
        [SerializeField] Button uiUpdateButton;
        [SerializeField] TextMeshProUGUI uiDescriptionText;

        [Space (20f)]
        [Header ("## Settings :")]
        [SerializeField] [TextArea (1, 5)] string jsonDataURL;

        bool isAlreadyCheckedForUpdates = false;

        GameData latestGameData;
        
        private LoadingScreenActivator LoadingScreenActivator => _loadingScreenActivator ??= Dependency.Get<LoadingScreenActivator>();
        private LoadingScreenActivator _loadingScreenActivator;

        void Awake()
        {
            instance = this;
        }

        public void StartAction()
        {
            if (!isAlreadyCheckedForUpdates)
            {
                StopAllCoroutines();
                StartCoroutine(CheckForUpdates());
            }
        }

        
        private IEnumerator CheckForUpdates()
        {
            UnityWebRequest request = UnityWebRequest.Get(jsonDataURL);
            request.disposeDownloadHandlerOnDispose = true;
            request.timeout = 60;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                LoadingScreenActivator.SetActiveLoadingScreen(true);
                LoadingScreenActivator.SetMessage("Error");
            }
            else
            {
                isAlreadyCheckedForUpdates = true;

                latestGameData = JsonUtility.FromJson<GameData>(request.downloadHandler.text);

                if (!string.IsNullOrEmpty(latestGameData.Version) && !Application.version.Equals(latestGameData.Version))
                {
                    Debug.Log("Latest game version: " + latestGameData.Version + " and latest game version on device: " + Application.version);
                    uiDescriptionText.text = latestGameData.Description;
                    ShowPopup();
                }
                else if (!string.IsNullOrEmpty(latestGameData.TechnicalBreak) && latestGameData.TechnicalBreak == "true")
                {
                    ShowTechPopup();
                }
                else
                {
                    //Dependency.Get<FirebaseAuthManager>().StartAction();
                }
            }

            request.Dispose();
        }

        void ShowTechPopup()
        {
            techCanvas.SetActive(true);
        }

        void ShowPopup()
        {
            uiUpdateButton.onClick.AddListener (() => { Application.OpenURL(latestGameData.Url); });

            updateCanvas.SetActive (true);
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        } 
    }
}
