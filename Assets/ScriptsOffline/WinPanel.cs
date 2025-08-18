using System.Collections;
using System.Collections.Generic;
using DependencyControl;
using Other;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private Button NextLevelButton;
    
    private LoadingScreenActivator LoadingScreenActivator => _loadingScreenActivator ??= Dependency.Get<LoadingScreenActivator>();
    private LoadingScreenActivator _loadingScreenActivator;

    void Start()
    {
        if (PlayerPrefs.GetInt("Levels") <= LevelManager.instance.eqipedLevel)
        {
            PlayerPrefs.SetInt("Levels", PlayerPrefs.GetInt("Levels") + 1);
        }

        if (LevelManager.instance.eqipedLevel == 32)
        {
            NextLevelButton.interactable = false;
        }
    }

    public void GoToMenu()
    {
        LoadingScreenActivator.SetActiveLoadingScreen(true);

        LoadingScreenActivator.GetComponent<AudioSource>().enabled = true;
        SceneManager.LoadSceneAsync("OfflineMenu");
    }

    public void NextLevel()
    {
        LoadingScreenActivator.SetActiveLoadingScreen(true);

        LevelManager.instance.eqipedLevel++;
        SceneManager.LoadSceneAsync("OfflineGame");
    }

    public void RetryLevel()
    {
        LoadingScreenActivator.SetActiveLoadingScreen(true);
        
        SceneManager.LoadSceneAsync("OfflineGame");
    }
}
