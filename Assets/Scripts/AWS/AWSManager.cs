using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Net;
using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Amazon.DynamoDBv2;

/*
    HUGE shoutout to BatterAcid @ github.com/BatteryAcid
    His video and code REALLY helped me here!
    Specifically from his [unity-cognito-hostedui-social-client] and [unity-cognito-client] repos
    Thank you!!
*/

namespace AWS
{
    public class AWSManager : MonoBehaviour
    {
        public static RegionEndpoint Region = RegionEndpoint.USEast2;

        private const string identityPoolId = "us-east-2:d50437ee-083d-4306-85aa-07f3629e5feb";
        private const string userPoolId = "us-east-2_xrfy4dSB3";
        private const string appClientId = "1049hrqpmc55a71ig1cd8g37f7";

        private AmazonCognitoIdentityProviderClient provider;
        private CognitoAWSCredentials cognitoAWSCredentials;
        private static string userid = "";
        private CognitoUser user;

        // Monobehaviour
        void Awake() {
            SaveData.Init();

            Debug.Log("AuthenticationManager: Awake");
            provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), Region);
        }

        // Get cognito credentials from serialized vars
        private CognitoAWSCredentials _credentials;
        private CognitoAWSCredentials Credentials {
            get {
                if (_credentials == null) {
                    _credentials = new CognitoAWSCredentials(identityPoolId, Region);
                }
                return _credentials;
            }
        }

        // Get client from credentials serialized vars
        private static AmazonDynamoDBClient _ddbClient;
        public AmazonDynamoDBClient Client {
            get {
                if (_ddbClient == null) {
                    _ddbClient = new AmazonDynamoDBClient(Credentials, Region);
                }
                return _ddbClient;
            }
        }


        public async Task<bool> RefreshSession()
        {
            Debug.Log("RefreshSession");

            DateTime issued = DateTime.Now;
            UserSessionCache userSessionCache = SaveData.Load(new UserSessionCache());

            if (userSessionCache == null || userSessionCache.refreshToken == null || userSessionCache.refreshToken == "") {
                Debug.Log("User sessoin cache has an error");
                return false;
            }

            try {
                CognitoUserPool userPool = new CognitoUserPool(userPoolId, appClientId, provider);

                // apparently the username field can be left blank for a token refresh request
                CognitoUser user = new CognitoUser("", appClientId, userPool, provider);

                // The "Refresh token expiration (days)" (Cognito->UserPool->General Settings->App clients->Show Details) is the
                // amount of time since the last login that you can use the refresh token to get new tokens. After that period the refresh
                // will fail Using DateTime.Now.AddHours(1) is a workaround for https://github.com/aws/aws-sdk-net-extensions-cognito/issues/24
                user.SessionTokens = new CognitoUserSession(
                    userSessionCache.idToken,
                    userSessionCache.accessToken,
                    userSessionCache.refreshToken,
                    issued,
                    DateTime.Now.AddDays(30)  // It was my understanding that this should be set to when your refresh token expires...
                );

                // Attempt refresh token call
                AuthFlowResponse authFlowResponse = await user.StartWithRefreshTokenAuthAsync(
                    new InitiateRefreshTokenAuthRequest {
                        AuthFlowType = AuthFlowType.REFRESH_TOKEN_AUTH
                    }
                ).ConfigureAwait(false);

                Debug.Log("User Access Token after refresh: " + authFlowResponse.AuthenticationResult.AccessToken);
                Debug.Log("User refresh token successfully updated!");

                // update session cache
                UserSessionCache userSessionCacheToUpdate = new UserSessionCache(
                    authFlowResponse.AuthenticationResult.IdToken,
                    authFlowResponse.AuthenticationResult.AccessToken,
                    authFlowResponse.AuthenticationResult.RefreshToken,
                    userSessionCache.userId
                );
                SaveData.Save(userSessionCacheToUpdate);

                // update credentials with the latest access token
                cognitoAWSCredentials = user.GetCognitoAWSCredentials(identityPoolId, Region);

                this.user = user;

                return true;
            }
            catch (NotAuthorizedException ne) {
                // https://docs.aws.amazon.com/cognito/latest/developerguide/amazon-cognito-user-pools-using-tokens-with-identity-providers.html
                // refresh tokens will expire - user must login manually every x days (see user pool -> app clients -> details)
                Debug.Log("NotAuthorizedException: " + ne);
            }
            catch (WebException webEx) {
                // we get a web exception when we cant connect to aws - means we are offline
                Debug.Log("WebException: " + webEx);
            }
            catch (Exception ex) {
                Debug.Log("Exception: " + ex);
            }
            return false;
        }


        public async Task<bool> Login(string username, string password) {
            Debug.Log("Login: " + username + ", " + password);

            CognitoUserPool userPool = new CognitoUserPool(userPoolId, appClientId, provider);
            CognitoUser user = new CognitoUser(username, appClientId, userPool, provider);

            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest{
                Password = password
            };

            try {
                AuthFlowResponse authFlowResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

                userid = await GetUserIdFromProvider(authFlowResponse.AuthenticationResult.AccessToken);
                
                Debug.Log("Users unique ID from cognito: " + userid);

                UserSessionCache userSessionCache = new UserSessionCache(
                    authFlowResponse.AuthenticationResult.IdToken,
                    authFlowResponse.AuthenticationResult.AccessToken,
                    authFlowResponse.AuthenticationResult.RefreshToken,
                    userid
                );
                SaveData.Save(userSessionCache);

                // This how you get credentials to use for accessing other services.
                // This IdentityPool is your Authorization, so if you tried to access using an
                // IdentityPool that didn't have the policy to access your target AWS service, it would fail.
                cognitoAWSCredentials = user.GetCognitoAWSCredentials(identityPoolId, Region);

                this.user = user;

                return true;
            }
            catch (Exception e) {
                Debug.Log("Login failed, exception: " + e);
                return false;
            }
        }


        public async Task<bool> Signup(string username, string email, string password)
        {
            Debug.Log("SignUpRequest: " + username + ", " + email + ", " + password);

            SignUpRequest signUpRequest = new SignUpRequest{
                ClientId = appClientId,
                Username = username,
                Password = password,
            };

            // Must provide all attributes required by the User Pool that you configured
            List<AttributeType> attributes = new List<AttributeType>{
                new AttributeType{
                    Name = "email", Value = email
                },
                new AttributeType{
                    Name = "updated_at", Value = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                }
            };
            signUpRequest.UserAttributes = attributes;

            try {
                SignUpResponse signUpResponse = await provider.SignUpAsync(signUpRequest);
                Debug.Log("Sign up successful");
                bool result = await Login(username, password);
                return result;
            }
            catch (Exception e) {
                Debug.Log("Sign up failed, exception: " + e);
            }
            return false;
        }


        public async Task<bool> ConfirmSignUp(string code)
        {
            CognitoUserPool userPool = new CognitoUserPool(userPoolId, appClientId, provider);
            user = (user == null) ? new CognitoUser("kynsonm", appClientId, userPool, provider) : user;

            ConfirmSignUpRequest confirmRequest = new ConfirmSignUpRequest{
                ClientId = appClientId,
                Username = user.Username,
                ConfirmationCode = code,
            };

            try {
                ConfirmSignUpResponse confirmResponse = await provider.ConfirmSignUpAsync(confirmRequest);
                Debug.Log("Confirm sign up was successful");
                return true;
            }
            catch (Exception e) {
                Debug.Log("Authentication failed, exception: " + e);
            }
            return false;
        }


        public async Task<bool> ResendVerificationCode(string username) {
            CognitoUserPool userPool = new CognitoUserPool(userPoolId, appClientId, provider);
            user = (user == null) ? new CognitoUser(username, appClientId, userPool, provider) : user;

            ResendConfirmationCodeRequest resendRequest = new ResendConfirmationCodeRequest(){
                ClientId = appClientId,
                Username = username
            };

            try {
                ResendConfirmationCodeResponse response = await provider.ResendConfirmationCodeAsync(resendRequest);
                Debug.Log("Confirmation was successful");
                return true;
            }
            catch (Exception e) {
                Debug.Log("Authentication failed, exception: " + e);
            }
            return false;
        }


        // Make the user's unique id available for GameLift APIs, linking saved data to user, etc
        public string GetUsersId()
        {
            Debug.Log("GetUserId: [" + userid + "]");
            if (userid == null || userid == "") {
                // load userid from cached session 
                UserSessionCache userSessionCache = SaveData.Load(new UserSessionCache());
                userid = userSessionCache.userId;
            }
            return userid;
        }

        // we call this once after the user is authenticated, then cache it as part of the session for later retrieval 
        private async Task<string> GetUserIdFromProvider(string accessToken)
        {
            Debug.Log("Getting user's id...");
            string subId = "";

            Task<GetUserResponse> responseTask = provider.GetUserAsync(
                new GetUserRequest{ AccessToken = accessToken }
            );
            GetUserResponse responseObject = await responseTask;

            // set the user id
            foreach (var attribute in responseObject.UserAttributes) {
                if (attribute.Name == "sub") {
                    subId = attribute.Value;
                    break;
                }
            }

            return subId;
        }

        // Limitation note: so this GlobalSignOutAsync signs out the user from ALL devices, and not just the game.
        // So if you had other sessions for your website or app, those would also be killed.  
        // Currently, I don't think there is native support for granular session invalidation without some work arounds.
        public async Task SignOut()
        {
            await user.GlobalSignOutAsync();

            // Important! Make sure to remove the local stored tokens 
            UserSessionCache userSessionCache = new UserSessionCache("", "", "", "");
            SaveData.Save(userSessionCache);

            Debug.Log("user logged out.");
        }

        public async Task DeleteAccount() {
            await user.DeleteUserAsync();

            // Important! Make sure to remove the local stored tokens 
            UserSessionCache userSessionCache = new UserSessionCache("", "", "", "");
            SaveData.Save(userSessionCache);

            Debug.Log("deleted user.");
        }
    }
}