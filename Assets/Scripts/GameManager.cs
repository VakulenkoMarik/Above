using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI info;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    public void SetMessage(string text)
    {
        info.text = text;
    }
    
    void Start()
    {
        instance = this;
    }

    public void SetActiveLoadingScreen(bool isActive)
    {
        loadingScreen.SetActive(isActive);
    }
}
