using System.Threading.Tasks;
using DependencyControl;
using Firebase.DataControl;
using Firebase.Storage;
using Other;
using Tools;

namespace Firebase.Auth
{
    public class StartGameInitiator
    {
        private readonly FirebaseStorage _storageReference;
        
        private LoadingScreenActivator LoadingScreenActivator => _loadingScreenActivator ??= Dependency.Get<LoadingScreenActivator>();
        private LoadingScreenActivator _loadingScreenActivator;

        private JsonStorage _jsonStorage;
        
        public StartGameInitiator(FirebaseStorage storage) {
            _storageReference = storage;
        }
        
        public async void OpenGameScene() {
            await StartGame();
        }

        private async Task StartGame() {
            VisualProcessing();

            await JsonStorageStartAction();

            await HandleTutorialFlow();
        }

        private void VisualProcessing() {
            LoadingScreenActivator.SetActiveLoadingScreen(true);
        }

        private async Task JsonStorageStartAction() {
            _jsonStorage = Dependency.Get<JsonStorage>();
            await _jsonStorage.StartAction(_storageReference);
        }

        private async Task HandleTutorialFlow() {
            if (IsTutorialCompleted()) {
                _jsonStorage.data.boolean.isTutorial = true;
                
                LoadingScreenActivator.SetActiveLoadingScreen(false);

                await Task.Delay(500);

                SceneLoader.OpenTutorialScene();
            }
            else {
                SceneLoader.OpenMainMenu();
            }
        }

        private bool IsTutorialCompleted() {
            return _jsonStorage.data.boolean.isTutorial;
        }
    }
}