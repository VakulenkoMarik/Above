using DependencyControl;
using TMPro;
using UnityEngine;

namespace Other
{
    public class LoadingScreenActivator : DependencyMonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private TextMeshProUGUI info;

        protected override void Awake() {
            if (Dependency.Get<LoadingScreenActivator>() != null) {
                Destroy(gameObject);
                return;
            }
            
            base.Awake();
        }

        public void SetMessage(string text) {
            info.text = text;
        }

        public void SetActiveLoadingScreen(bool isActive) {
            loadingScreen.SetActive(isActive);
        }
    }
}
