using System;
using System.Collections;
using DependencyControl;
using Firebase.Extensions;
using Firebase.Frontend;
using Other;
using Tools;
using UnityEngine;

namespace Firebase.Auth
{
    public class RegisterController
    {
        private readonly FirebaseAuth _auth;
        private readonly FirebaseUser _user;
        private readonly AuthenticationUiActionsSender _uiActionsSender;
        
        private LoadingScreenActivator LoadingScreenActivator => _loadingScreenActivator ??= Dependency.Get<LoadingScreenActivator>();
        private LoadingScreenActivator _loadingScreenActivator;
            
        private CoroutineRunner CoroutineRunner => _coroutineRunner ??= Dependency.Get<CoroutineRunner>();
        private CoroutineRunner _coroutineRunner;

        public RegisterController(FirebaseAuth newAuth, AuthenticationUiActionsSender uiActionsSender) {
            _auth = newAuth;
            _user = _auth.CurrentUser;
            _uiActionsSender = uiActionsSender;
        }
        
        public void Register(RegistrationData data) {
            LoadingScreenActivator.SetActiveLoadingScreen(true);
            CoroutineRunner.Run(RegisterAsync(data.UserName, data.Email, data.Password, data.ConfirmPassword));
        }

        private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword) {
            yield return new WaitForSeconds(0.5f);

            if (name == "") {
                _uiActionsSender.SetErrorMessage("User Name is empty");
                LoadingScreenActivator.SetActiveLoadingScreen(false);
            }
            else if (email == "") {
                _uiActionsSender.SetErrorMessage("email field is empty");
                LoadingScreenActivator.SetActiveLoadingScreen(false);
            }
            else if (!string.Equals(password, confirmPassword, StringComparison.Ordinal)) {
                _uiActionsSender.SetErrorMessage("Password does not match");
                LoadingScreenActivator.SetActiveLoadingScreen(false);
            }
            else {
                var registerTask = _auth.CreateUserWithEmailAndPasswordAsync(email, password);

                yield return new WaitUntil(() => registerTask.IsCompleted);

                if (registerTask.Exception != null) {
                    LoadingScreenActivator.SetActiveLoadingScreen(false);

                    FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;

                    string failedMessage = "Registration Failed! Because ";
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
                            failedMessage = "Registration Failed";
                            break;
                    }

                    _uiActionsSender.SetErrorMessage(failedMessage);
                }
                else {
                    UserProfile userProfile = new UserProfile { DisplayName = name };

                    var updateProfileTask = _user.UpdateUserProfileAsync(userProfile);

                    yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                    if (updateProfileTask.Exception != null) {
                        _user.DeleteAsync();

                        FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError authError = (AuthError)firebaseException.ErrorCode;
                        
                        string failedMessage = "Profile update Failed! Because ";
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
                                failedMessage = "Profile update Failed";
                                break;
                        }

                        _uiActionsSender.SetErrorMessage(failedMessage);

                        LoadingScreenActivator.SetActiveLoadingScreen(false);
                    }
                    else {
                        if (_user.IsEmailVerified) {
                            _uiActionsSender.OpenLoginPanel();

                            LoadingScreenActivator.SetActiveLoadingScreen(false);
                        }
                        else {
                            SendEmailForVerification();

                            LoadingScreenActivator.SetActiveLoadingScreen(false);
                        }
                    }
                }
            }
        }

        public void SendEmailForVerification() {
            CoroutineRunner.Run(SendEmailForVerificationAsync());
        }

        private IEnumerator SendEmailForVerificationAsync() {
            if (_user != null) {
                var sendEmailTask = _user.SendEmailVerificationAsync();

                yield return new WaitUntil(() => sendEmailTask.IsCompleted);

                if (sendEmailTask.Exception != null) {
                    FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
                    AuthError error = (AuthError) firebaseException.ErrorCode;

                    string errorMessage = "Unknown Error : Please try again later";

                    switch (error) {
                        case AuthError.Cancelled:
                            errorMessage = "Email verification was cancelled";
                            break;
                        case AuthError.TooManyRequests:
                            errorMessage = "Too many request";
                            break;
                        case AuthError.InvalidRecipientEmail:
                            errorMessage = "The email you entered is invalid";
                            break;
                    }

                    _uiActionsSender.ShowVerificationResponse(false, _user.Email, errorMessage);
                }
                else {
                    _uiActionsSender.ShowVerificationResponse(true, _user.Email, null);
                }
            }
        }
        
        public void ForgetPassword(string email) {
            _uiActionsSender.DisposeAuthenticationPanel();
        
            if (string.IsNullOrEmpty(email)) {
                _uiActionsSender.SetErrorMessage("The email field cannot be empty");
                return;
            }

            ForgetPassSubmit(email);
        }

        private void ForgetPassSubmit(string email) {
            _auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task => {
                if (task.Exception != null) {
                    FirebaseException firebaseException = task.Exception.GetBaseException() as FirebaseException;
                    AuthError error = (AuthError)firebaseException.ErrorCode;

                    string failedMessage = "";

                    switch (error) {
                        case AuthError.InvalidEmail:
                            failedMessage += "Email is invalid";
                            break;
                        case AuthError.MissingEmail:
                            failedMessage += "Email is missing";
                            break;
                        default:
                            failedMessage = "Login Failed";
                            break;
                    }

                    _uiActionsSender.SetErrorMessage("Error: " + failedMessage);
                }
            });
        }
    }
}