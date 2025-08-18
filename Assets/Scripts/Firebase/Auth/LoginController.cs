using System;
using System.Collections;
using DependencyControl;
using Firebase.DataControl;
using Firebase.Frontend;
using Firebase.Storage;
using Other;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Firebase.Auth
{
    public class LoginController
    {
        private readonly FirebaseAuth _auth;
        private readonly FirebaseUser _user;
        private readonly AuthenticationUiActionsSender _uiActionsSender;

        private Action _onLogin;
        private Action _onEndAutoLogin;
        private Action _onFailAutoLogin;
        private Action _onFailVerification;

        private CoroutineRunner CoroutineRunner => _coroutineRunner ??= Dependency.Get<CoroutineRunner>();
        private CoroutineRunner _coroutineRunner;
        
        private LoadingScreenActivator LoadingScreenActivator => _loadingScreenActivator ??= Dependency.Get<LoadingScreenActivator>();
        private LoadingScreenActivator _loadingScreenActivator;
        
        public LoginController(FirebaseAuth newAuth, AuthenticationUiActionsSender uiActionsSender) {
            _auth = newAuth;
            _user = _auth.CurrentUser;
            _uiActionsSender = uiActionsSender;
        }

        public void SetActionSuccessfulLogin(Action targetAction) {
            _onLogin += targetAction;
        }
        
        public void SetActionEndAutoLoginAttempt(Action targetAction) {
            _onEndAutoLogin += targetAction;
        }
        
        public void SetActionFailAutoLogin(Action targetAction) {
            _onFailAutoLogin += targetAction;
        }
        
        public void SetActionNotVerifiedUser(Action targetAction) {
            _onFailVerification += targetAction;
        }
        
        public void TryAutoLoginProcessing() {
            CoroutineRunner.Run(CheckForAutoLogin());
        }

        private IEnumerator CheckForAutoLogin() {
            if (_user != null) {
                var reloadUserTask = _user.ReloadAsync();

                yield return new WaitUntil(() => reloadUserTask.IsCompleted);

                AutoLogin();
            }
            else {
                _onFailAutoLogin.Invoke();
            }
            
            _onEndAutoLogin.Invoke();
        }
        
        private void AutoLogin() {
            if (_user is { IsEmailVerified: true }) {
                StartLoginProcess();
            }
        }

        private void StartLoginProcess() {
            Dependency.Get<StorageData>().SetAuth(_auth);

            _onLogin.Invoke();
        }
        
        public void Login(LoginData data) {
            CoroutineRunner.Run(LoginAsync(data.Email, data.Password));
        }

        private IEnumerator LoginAsync(string email, string password) {
            var loginTask = _auth.SignInWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => loginTask.IsCompleted);

            if (loginTask.Exception != null) {
                FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Login Failed! Because ";

                switch (authError) {
                    case AuthError.InvalidEmail:
                        failedMessage += "Email is invalid";
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Wrong Password";
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email is missing";
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Password is missing";
                        break;
                    default:
                        failedMessage = "Login Failed";
                        break;
                }

                _uiActionsSender.SetErrorMessage(failedMessage);
            }
            else {
                if (_user.IsEmailVerified) {
                    LoadingScreenActivator.SetActiveLoadingScreen(true);

                    yield return new WaitForSeconds(2);

                    StartLoginProcess();
                }
                else {
                    _onFailVerification.Invoke();
                }
            }
        }
        
        public void Logout() {
            Dependency.Get<StorageData>().DeleteJson();
            
            _auth.SignOut();
            
            SceneLoader.OpenAuthenticationScene();
        }
    }
}