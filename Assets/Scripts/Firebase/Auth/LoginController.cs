using System;
using System.Collections;
using DependencyControl;
using Extensions;
using Firebase.DataControl;
using Firebase.Frontend;
using Other;
using Tools;
using UnityEngine;

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
        
        public void TryLogin(LoginData data) {
            CoroutineRunner.Run(TryLoginAsync(data.Email, data.Password));
        }

        private IEnumerator TryLoginAsync(string email, string password) {
            var loginTask = _auth.SignInWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => loginTask.IsCompleted);

            if (loginTask.Exception != null) {
                FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Login Failed! Because " + authError.GetErrorText("Unknown error");

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

        public void Reset() {
            _onLogin = null;
            _onFailVerification = null;
            _onEndAutoLogin = null;
            _onFailAutoLogin = null;
        }
    }
}