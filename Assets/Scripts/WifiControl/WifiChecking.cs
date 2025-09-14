using System;
using System.Collections;
using DependencyControl;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace WifiControl
{
    public struct InternetData
    {
        public bool IsInternetConnection;
        public WifiChecking Checker;
    }
    
    public class WifiChecking : DependencyMonoBehaviour
    {
        [Header("-- Settings --")]
        [SerializeField] private float checkInterval = 1f;
        [SerializeField] private int checkTimeOut = 5;
        [SerializeField] private string pingUrl = "https://www.google.com";

        public delegate void ConnectionStatusChanged(InternetData data);
        public event ConnectionStatusChanged OnConnectionStatusChanged;

        private InternetData _currentData;

        private void Start() {
            _currentData.Checker = this;
        }

        public bool IsInternetConnection() {
            CheckConnection();
            return _currentData.IsInternetConnection;
        }

        public void StartChecking() {
            InvokeRepeating(nameof(CheckConnection), 0f, checkInterval);
        }
        
        public void StopChecking() {
            CancelInvoke(nameof(CheckConnection));
        }

        private void CheckConnection() {
            StartCoroutine(CheckInternetConnection(result => {
                if (result != _currentData.IsInternetConnection) {
                    _currentData.IsInternetConnection = result;
                    OnConnectionStatusChanged?.Invoke(_currentData);
                }
            }));
        }

        private IEnumerator CheckInternetConnection(System.Action<bool> callback) {
            using UnityWebRequest request = UnityWebRequest.Head(pingUrl);
            
            request.timeout = checkTimeOut;
            
            yield return request.SendWebRequest();

            callback(request.result == UnityWebRequest.Result.Success);
        }
    }
}
