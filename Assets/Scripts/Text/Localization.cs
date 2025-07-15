using UnityEngine;
using UnityEngine.SceneManagement;

namespace Text
{
    public class Localization : MonoBehaviour
    {
        public void ChangeLanguage(string language)
        {
            PlayerPrefs.SetString("Language", language);

            SceneManager.LoadSceneAsync("Authentication");
        }
    }
}
