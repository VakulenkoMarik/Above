using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Other.Buttons
{
    public class ButtonsManagerInGame : MonoBehaviour
    {
        IEnumerator RetryGameAsync()
        {
            GameManager.instance.SetActiveLoadingScreen(true);

            yield return new WaitForSeconds(3);

            SceneManager.LoadSceneAsync("Game");
        }

        IEnumerator ReturnToMainMenuAsync()
        {
            GameManager.instance.SetActiveLoadingScreen(true);

            yield return new WaitForSeconds(3);

            SceneManager.LoadSceneAsync("MainMenu");
        }
    
        public void RetryGame()
        {
            StartCoroutine(RetryGameAsync());
        }

        public void ToMainMenu()
        {
            StartCoroutine(ReturnToMainMenuAsync());
        }
    }
}
