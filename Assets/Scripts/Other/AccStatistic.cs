using System;
using DependencyControl;
using Firebase.DataControl;
using Firebase.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Other
{
    public class AccStatistic : MonoBehaviour
    {
        [Header ("## Panel parameters : ")]
        [SerializeField] private Image userIcon;
        [SerializeField] private TextMeshProUGUI flyCoinsAllTime;
        [SerializeField] private TextMeshProUGUI superCoinsAllTime;
        [SerializeField] private TextMeshProUGUI hightScore;
        [SerializeField] private TextMeshProUGUI level;
        [SerializeField] private TextMeshProUGUI createdAccount;

        [SerializeField] private TextMeshProUGUI userName;
        [SerializeField] private TextMeshProUGUI userEmail;

        [SerializeField] private GameObject panel;

        [Header ("## Other resources : ")]
        [SerializeField] private Image icon;
        
        private StorageData StorageData => _storageData ??= Dependency.Get<StorageData>();
        private StorageData _storageData;

        public void OpenStatistic()
        {
            userIcon.sprite = icon.sprite;

            flyCoinsAllTime.text = Dependency.Get<JsonStorage>().data.userData.coinsFAllTime + "";
            superCoinsAllTime.text = Dependency.Get<JsonStorage>().data.userData.coinsSAllTime + "";
            hightScore.text = Dependency.Get<JsonStorage>().data.userData.record + "";
            level.text = Dependency.Get<JsonStorage>().data.userData.level + "";

            ulong creationTimestamp = StorageData.GetAuth().CurrentUser.Metadata.CreationTimestamp;
            long signedTimestamp = (long)creationTimestamp;

            DateTime creationDateTime = DateTimeOffset.FromUnixTimeMilliseconds(signedTimestamp).DateTime;
            string formattedDate = creationDateTime.ToString("yyyy-MM-dd");

            createdAccount.text = formattedDate;

            userName.text = StorageData.GetAuth().CurrentUser.DisplayName;
            userEmail.text = StorageData.GetAuth().CurrentUser.Email;

            panel.SetActive(true);
        }
    }
}
