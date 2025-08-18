using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Firebase.Frontend
{
    public struct LoginData
    {
        public string Email;
        public string Password;
    }
    
    public struct RegistrationData
    {
        public string UserName;
        public string Email;
        public string Password;
        public string ConfirmPassword;
    }
    
    public class AuthenticationUiDataReader : MonoBehaviour
    {
        [Header("--Login--")]
        [SerializeField] private TMP_InputField emailLoginField;
        [SerializeField] private TMP_InputField passwordLoginField;
        
        [Header("--Registration--")]
        [SerializeField] private TMP_InputField nameRegisterField;
        [SerializeField] private TMP_InputField emailRegisterField;
        [SerializeField] private TMP_InputField passwordRegisterField;
        [SerializeField] private TMP_InputField confirmPasswordRegisterField;
        
        [Header("--Forget password--")]
        [SerializeField] private TMP_InputField forgetPassEmailField;
        [SerializeField] private GameObject sendResetPassText;
        
        public void ClearLoginInputFieldText() {
            emailLoginField.text = "";
            passwordLoginField.text = "";
        }

        public void ClearRegisterInputFieldText() {
            nameRegisterField.text = "";
            emailRegisterField.text = "";
            passwordRegisterField.text = "";
            confirmPasswordRegisterField.text = "";
        }

        public void ResetForgetPasswordPage() {
            forgetPassEmailField.text = "";
            sendResetPassText.SetActive(false);
        }
        
        public string GetForgetPasswordData() {
            return forgetPassEmailField.text;
        }

        public LoginData GetLoginData() {
            return new LoginData {
                Email = emailLoginField.text,
                Password = passwordLoginField.text
            };
        }
        
        public RegistrationData GetRegistrationData() {
            return new RegistrationData {
                UserName = nameRegisterField.text,
                Email = emailRegisterField.text,
                Password = passwordRegisterField.text,
                ConfirmPassword = confirmPasswordRegisterField.text
            };
        }
    }
}