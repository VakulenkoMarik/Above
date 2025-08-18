using DependencyControl;
using Encrypting;
using Firebase.DataControl;
using Firebase.Storage;
using UnityEngine;

namespace MainMenu.Select
{
    public class SelectBoost : MonoBehaviour
    {
        private int currentBoost;

        [SerializeField] private GameObject[] AllBoosts;
        [SerializeField] private GameObject[] Heroes;

        [SerializeField] private GameObject[] EquipButtons;
        [SerializeField] private GameObject[] EquipedButtons;

        [SerializeField] private Color shieldColor;
        [SerializeField] private Color standartColor;

        void Start()
        {
            if (Dependency.Get<JsonStorage>().data.currentShop.currentBoost == 3)
            {
                foreach (GameObject hero in Heroes)
                {
                    hero.GetComponent<SpriteRenderer>().color = shieldColor;
                }
            }
            else
            {
                foreach (GameObject hero in Heroes)
                {
                    hero.GetComponent<SpriteRenderer>().color = standartColor;
                }
            }

            currentBoost = Dependency.Get<JsonStorage>().data.currentShop.currentBoost;

            EquipedButtons[currentBoost].SetActive(true);
            EquipButtons[currentBoost].SetActive(false);

            AllBoosts[currentBoost].SetActive(true);
        }

        public void Change(int thisBoost)
        {
            for (int i = 0; i < AllBoosts.Length; i++)
            {
                AllBoosts[i].SetActive(false);
                EquipedButtons[i].SetActive(false);
                EquipButtons[i].SetActive(true);
            }

            if (thisBoost == 3)
            {
                foreach (GameObject hero in Heroes)
                {
                    hero.GetComponent<SpriteRenderer>().color = shieldColor;
                }
            }
            else
            {
                foreach (GameObject hero in Heroes)
                {
                    hero.GetComponent<SpriteRenderer>().color = standartColor;
                }
            }

            Dependency.Get<JsonStorage>().data.currentShop.currentBoost = thisBoost;

            CryptoHelper.Encrypt(Dependency.Get<JsonStorage>().data, Dependency.Get<JsonStorage>().password);

            AllBoosts[thisBoost].SetActive(true);

            EquipedButtons[thisBoost].SetActive(true);
            EquipButtons[thisBoost].SetActive(false);
        }
    }
}
