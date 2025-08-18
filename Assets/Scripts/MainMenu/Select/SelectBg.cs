using System.Collections;
using DependencyControl;
using Encrypting;
using Firebase.DataControl;
using Firebase.Storage;
using Other;
using UnityEngine;

namespace MainMenu.Select
{
    public class SelectBg : MonoBehaviour
    {
        private int i;

        [SerializeField] private GameObject[] AllBg;
        [SerializeField] private GameObject[] AllRailings;

        [SerializeField] private GameObject[] EquipButtons;
        [SerializeField] private GameObject[] EquipedButtons;

        void Start()
        {
            i = Dependency.Get<JsonStorage>().data.currentShop.currentBg;

            EquipedButtons[i].SetActive(true);
            EquipButtons[i].SetActive(false);

            AllBg[i].SetActive(true);
            AllRailings[i].SetActive(true);

            StartCoroutine(StartGame());
        }

        private IEnumerator StartGame()
        {
            yield return new WaitForSeconds(1);

            Dependency.Get<LoadingScreenActivator>().SetActiveLoadingScreen(false);
        }

        public void Change(int thisBg)
        {
            for (int i = 0; i < AllBg.Length; i++)
            {
                AllBg[i].SetActive(false);
                AllRailings[i].SetActive(false);

                EquipedButtons[i].SetActive(false);
                EquipButtons[i].SetActive(true);
            }

            Dependency.Get<JsonStorage>().data.currentShop.currentBg = thisBg;
        
            CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);

            AllBg[thisBg].SetActive(true);
            AllRailings[thisBg].SetActive(true);

            EquipedButtons[thisBg].SetActive(true);
            EquipButtons[thisBg].SetActive(false);
        }
    }
}
