using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;

/*
    HUGE shoutout to BatterAcid @ github.com/BatteryAcid
    His video and code REALLY helped me here!
    Specifically from his [unity-cognito-hostedui-social-client] and [unity-cognito-client] repos
    Thank you!!
*/

namespace AWS
{
    public class UserAWS : MonoBehaviour
    {
        AWSManager awsManager;

        [SerializeField] TMP_Text resultText;
        [SerializeField] Button refreshSessionButton;
        [SerializeField] Button logInButton;
        [SerializeField] Button signUpButton;
        [SerializeField] Button confirmSignUpButton;
        [SerializeField] Button signOutButton;
        [SerializeField] Button deleteAccountButton;
        [SerializeField] Button resendVerifyCodeButton;
        [SerializeField] TMP_InputField codeInputField;

        void Awake() {
            refreshSessionButton.onClick.RemoveAllListeners();
            refreshSessionButton.onClick.AddListener(async () => {
                await RefreshSessionButtonClick();
            });

            logInButton.onClick.RemoveAllListeners();
            logInButton.onClick.AddListener(async () => {
                await LoginButtonClick();
            });

            signUpButton.onClick.RemoveAllListeners();
            signUpButton.onClick.AddListener(async () => {
                await SignUpButtonClick();
            });

            confirmSignUpButton.onClick.RemoveAllListeners();
            confirmSignUpButton.onClick.AddListener(async () => {
                await ConfirmSignUpButtonClick();
            });

            deleteAccountButton.onClick.RemoveAllListeners();
            deleteAccountButton.onClick.AddListener(async () => {
                await DeleteAccountButtonClick();
            });

            signOutButton.onClick.RemoveAllListeners();
            signOutButton.onClick.AddListener(async () => {
                await SignOutButtonClick();
            });

            resendVerifyCodeButton.onClick.RemoveAllListeners();
            resendVerifyCodeButton.onClick.AddListener(async () => {
                await ResendVerifyCodeButtonClick();
            });

            if (!checkManager) {
                Debug.Log("DB Manager is bad :(");
                return;
            }
        }

        async Task<bool> LoginButtonClick() {
            if (!checkManager) { return false; }

            resultText.text = "Attempting to login...\n";
            bool result = await awsManager.Login("kynsonm", "Torva0192!");

            resultText.text += "Login " + (result ? "WAS" : "was NOT") + " successful";
            return result;
        }

        async Task SignOutButtonClick() {
            if (!checkManager) { return; }

            resultText.text = "Attempting to sign out...\n";
            await awsManager.SignOut();

            resultText.text += "User signed out";
        }

        async Task<bool> SignUpButtonClick() {
            if (!checkManager) { return false; }

            resultText.text = "Attempting to sign up...\n";
            bool result = await awsManager.Signup("kynsonm", "kynsonmitchell@gmail.com", "Torva0192!");

            resultText.text += "Sign up " + (result ? "WAS" : "was NOT") + " successful";
            return result;
        }

        async Task<bool> ConfirmSignUpButtonClick() {
            if (!checkManager) { return false; }

            resultText.text = "Attempting to confirm sign up...\n";
            bool result = await awsManager.ConfirmSignUp(codeInputField.text);

            resultText.text += "Confirm sign up " + (result ? "WAS" : "was NOT") + " successful";
            return result;
        }

        async Task<bool> ResendVerifyCodeButtonClick() {
            if (!checkManager) { return false; }

            resultText.text = "Attempting to resend confirmation code...\n";
            bool result = await awsManager.ResendVerificationCode("kynsonm");

            resultText.text += "Resending confirmation code " + (result ? "WAS" : "was NOT") + " successful";
            return result;
        }

        async Task DeleteAccountButtonClick() {
            if (!checkManager) { return; }

            resultText.text = "Attempting to delete account...\n";
            await awsManager.DeleteAccount();

            resultText.text += "Deleted account";
        }

        async Task<bool> RefreshSessionButtonClick() {
            if (!checkManager) { return false; }

            resultText.text = "Attempting to refresh session...\n";
            bool result = await awsManager.RefreshSession();

            resultText.text += "Refresh session " + (result ? "WAS" : "was NOT") + " successful";
            return result;
        }

        bool checkManager {
            get {
                if (awsManager == null) {
                    awsManager = GameObject.FindObjectOfType<AWSManager>();
                    if (awsManager == null) {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}