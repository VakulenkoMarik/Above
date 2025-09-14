using System.Collections;
using System.Collections.Generic;
using DependencyControl;
using Other;
using UnityEngine;
using UnityEngine.SceneManagement;
using WifiControl;

public class OfflineManager : MonoBehaviour
{
    private LoadingScreenActivator LoadingScreenActivator => _loadingScreenActivator ??= Dependency.Get<LoadingScreenActivator>();
    private LoadingScreenActivator _loadingScreenActivator;
    
    void Start()
    {
        LoadingScreenActivator.SetActiveLoadingScreen(false);
    }

    public void EquipHero(int index)
    {
        LevelManager.instance.equipedHero = index;
    }

    public void ToLobby()
    {
        LoadingScreenActivator.gameObject.GetComponent<AudioSource>().volume = 0.2f;
        SceneManager.LoadSceneAsync("Authentication");
    }

    public void StartGame(int index)
    {
        StartCoroutine(GameIsStarted(index));
    }

    private IEnumerator GameIsStarted(int index)
    {
        LoadingScreenActivator.SetActiveLoadingScreen(true);

        yield return new WaitForSeconds(0.5f);

        LevelManager.instance.eqipedLevel = index;
        LoadingScreenActivator.GetComponent<AudioSource>().enabled = false;

        SceneManager.LoadSceneAsync("OfflineGame");
    }
}
