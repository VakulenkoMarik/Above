using System.Collections;
using DependencyControl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Other.Buttons
{
    public class ButtonsManagerInGame : MonoBehaviour
    {
        IEnumerator RetryGameAsync()
        {
            Dependency.Get<LoadingScreenActivator>().SetActiveLoadingScreen(true);

            yield return new WaitForSeconds(3);

            SceneManager.LoadSceneAsync("Game");
        }

        IEnumerator ReturnToMainMenuAsync()
        {
            Dependency.Get<LoadingScreenActivator>().SetActiveLoadingScreen(true);

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
