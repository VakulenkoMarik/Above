using DependencyControl;
using Firebase.Frontend;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.Events;
using WifiControl;

namespace Firebase.Auth
{
    public class FirebaseAuthSystem : DependencyMonoBehaviour
    {
        [SerializeField] private AuthenticationUiDataReader uiDataReader;
        [SerializeField] private AuthenticationUiActionsSender uiActionsSender;
        
        private LoginController _loginController;
        private RegisterController _registerController;
        private StartGameInitiator _startGameInitiator;

        private bool _wasAttemptAutoLogin = false;

        public FirebaseUser User => Auth.CurrentUser;
        public FirebaseAuth Auth { get; private set; }
        public FirebaseStorage Storage { get; private set; }

        protected override void Awake() {
            if (Dependency.Get<FirebaseAuthSystem>()) {
                Destroy(gameObject);
                return;
            }
            
            base.Awake();

            StartInitializationFirebaseAPI();
        }

        private void StartInitializationFirebaseAPI() {
            AuthFirebaseInitialization firebaseDataInitialization = new AuthFirebaseInitialization();
            firebaseDataInitialization.OnInitialized += AfterFirebaseInitProcess;
            firebaseDataInitialization.Initialize();
        }

        private void AfterFirebaseInitProcess(FirebaseInitializedData firebaseData) {
            SetLocalFirebaseReferences(firebaseData);
            
            (_loginController, _registerController, _startGameInitiator) = CreateControllers();
            SetUpControllers(_loginController, _registerController, _startGameInitiator);
            
            StartInternetChecking();
        }
        
        private void SetLocalFirebaseReferences(FirebaseInitializedData firebaseData) {
            Auth = firebaseData.Auth;
            Storage = firebaseData.Storage;
        }

        private (LoginController, RegisterController, StartGameInitiator) CreateControllers() {
            return (new LoginController(Auth, uiActionsSender),
                    new RegisterController(Auth, uiActionsSender),
                    new StartGameInitiator(Storage));
        }

        private void SetUpControllers(LoginController loginController, RegisterController registerController, StartGameInitiator startGameInitiator) {
            loginController.SetActionSuccessfulLogin(() => AddListenerToOnlineButton(startGameInitiator.OpenGameScene));
            loginController.SetActionEndAutoLoginAttempt(uiActionsSender.DeactivatePreLoginLoadingIcon);
            loginController.SetActionEndAutoLoginAttempt(uiActionsSender.ActivateButtons);
            
            loginController.SetActionFailAutoLogin(() => AddListenerToOnlineButton(uiActionsSender.OpenLoginPanel));
            loginController.SetActionNotVerifiedUser(() => AddListenerToOnlineButton(registerController.SendEmailForVerification));
        }

        public void StartInternetChecking() {
            WifiChecking wifiChecking = Dependency.Get<WifiChecking>();
            wifiChecking.StartChecking();
            wifiChecking.OnConnectionStatusChanged += HandleInternetConnection;
        }
        
        private void HandleInternetConnection(InternetData data) {
            if (data.IsInternetConnection && !_wasAttemptAutoLogin) {
                _wasAttemptAutoLogin = true;
                StartTryingAutoLoginProcess();
            }
            else if (!data.IsInternetConnection) {
                uiActionsSender.SetActiveWifiErrorScreen(true);
                data.Checker.OnConnectionStatusChanged -= HandleInternetConnection;
            }
        }

        private void AddListenerToOnlineButton(UnityAction action) {
            uiActionsSender.AddListenerToOnlineButton(action);
        }
        
        private void StartTryingAutoLoginProcess() {
            _loginController.TryAutoLoginProcessing();
        }

        public void Login() {
            LoginData data = uiDataReader.GetLoginData();
            _loginController.TryLogin(data);
        }
        
        public void Register() {
            RegistrationData data = uiDataReader.GetRegistrationData();
            _registerController.TryRegister(data);
        }

        public void ForgetPassword() {
            string email = uiDataReader.GetForgetPasswordData();
            _registerController.ForgetPassword(email);
        }

        protected override void OnDestroy() {
            _loginController.Reset();
            
            base.OnDestroy();
        }
    }
}