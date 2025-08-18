using DependencyControl;
using Firebase.Frontend;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.Events;

namespace Firebase.Auth
{
    public class FirebaseAuthSystem : DependencyMonoBehaviour
    {
        [SerializeField] private AuthenticationUiDataReader uiDataReader;
        [SerializeField] private AuthenticationUiActionsSender uiActionsSender;
        
        private LoginController _loginController;
        private RegisterController _registerController;
        private StartGameInitiator _startGameInitiator;

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
            
            CreateControllers();
            SetUpControllers();
            
            StartLoginProcess();
        }

        private void SetLocalFirebaseReferences(FirebaseInitializedData firebaseData) {
            Auth = firebaseData.Auth;
            Storage = firebaseData.Storage;
        }

        private void CreateControllers() {
            _loginController = new LoginController(Auth, uiActionsSender);
            _registerController = new RegisterController(Auth, uiActionsSender);
            _startGameInitiator = new StartGameInitiator(Storage);
        }

        private void SetUpControllers() {
            _loginController.SetActionSuccessfulLogin(() => AddListenerToOnlineButton(_startGameInitiator.OpenGameScene));
            _loginController.SetActionEndAutoLoginAttempt(uiActionsSender.DeactivatePreLoginLoadingIcon);
            _loginController.SetActionEndAutoLoginAttempt(uiActionsSender.ActivateButtons);
            
            _loginController.SetActionFailAutoLogin(() => AddListenerToOnlineButton(uiActionsSender.OpenLoginPanel));
            _loginController.SetActionNotVerifiedUser(() => AddListenerToOnlineButton(_registerController.SendEmailForVerification));
        }

        private void AddListenerToOnlineButton(UnityAction action) {
            uiActionsSender.AddListenerToOnlineButton(action);
        }
        
        private void StartLoginProcess() {
            _loginController.TryAutoLoginProcessing();
        }

        public void Login() {
            LoginData data = uiDataReader.GetLoginData();
            _loginController.Login(data);
        }
        
        public void Register() {
            RegistrationData data = uiDataReader.GetRegistrationData();
            _registerController.Register(data);
        }

        public void ForgetPassword() {
            string email = uiDataReader.GetForgetPasswordData();
            _registerController.ForgetPassword(email);
        }
    }
}