using DependencyControl;
using Encrypting;
using Firebase.DataControl;
using Firebase.Storage;
using TMPro;
using UnityEngine;

namespace Game
{
    public class LosePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        public void Death(int lastRunScore) {
            JsonStorage storage = Dependency.Get<JsonStorage>();
            
            int recordScore = storage.data.userData.record;
            var gainedExp = lastRunScore / 14;

            if (gainedExp > 100) {
                gainedExp = 100;
            }

            int doneExp = storage.data.userData.exp + gainedExp;

            storage.data.userData.exp = doneExp;

            if (lastRunScore > recordScore) {
                recordScore = lastRunScore;
                storage.data.userData.record = recordScore;
            }

            CryptoHelper.Encrypt(storage.data, storage.password);

            scoreText.text = recordScore.ToString();
        }
    }
}
