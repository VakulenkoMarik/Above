using UnityEngine.SceneManagement;

namespace Tools
{
    public static class SceneLoader
    {
        private const string AuthenticationScene = "Authentication";
        private const string TutorialScene = "Tutorial";
        
        private const string RoundScene = "RoundOnline";
        private const string OfflineRoundScene = "RoundOffline";
        
        private const string MainMenuScene = "MainMenu";
        private const string OfflineMenuScene = "OfflineMenu";

        public static void OpenTutorialScene() {
            SceneManager.LoadScene(TutorialScene);
        }

        public static void OpenMainMenu() {
            SceneManager.LoadScene(MainMenuScene);
        }

        public static void OpenOfflineMenuScene() {
            SceneManager.LoadScene(OfflineMenuScene);
        }
        
        public static void OpenRoundScene() {
            SceneManager.LoadScene(RoundScene);
        }
        
        public static void OpenOfflineRoundScene() {
            SceneManager.LoadScene(OfflineRoundScene);
        }
        
        public static void OpenAuthenticationScene() {
            SceneManager.LoadScene(AuthenticationScene);
        }
    }
}