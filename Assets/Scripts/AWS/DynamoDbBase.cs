using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;


namespace AWS
{
    public class DynamoDbBase : MonoBehaviour
    {
        protected AWSManager awsManager;
        //protected IAmazonCognitoIdentityProvider cognitoService;

        // Start is called before the first frame update
        void Awake() {
            //UnityInitializer.AttachToGameObject(this.gameObject);
            if (!CheckManager()) { return; }

            //cognitoService = new AmazonCognitoIdentityProviderClient(
            //    new AnonymousAWSCredentials(),
            //    awsManager.CognitoPoolRegion
            //);

            //Debug.Log("Getting cognito service:\n" + cognitoService.ToString());
        }

        private AmazonDynamoDBClient _client;
        protected AmazonDynamoDBClient Client {
            get {
                if (!CheckManager()) { return null; }
                if (_client == null) {
                    _client = awsManager.Client;
                }
                return _client;
            }
        }

        private DynamoDBContext _context;
        protected DynamoDBContext Context {
            get {
                if (!CheckManager()) { return null; }
                if (_context == null) {
                    _context = new DynamoDBContext(Client);
                }
                return _context;
            }
        }


        protected bool CheckManager() {
            if (awsManager == null) {
                awsManager = GameObject.FindObjectOfType<AWSManager>();
                if (awsManager == null) {
                    Debug.LogError("DynamoDbBase: No AWSManager script in the scene!");
                    return false;
                }
            }
            return true;
        }
    }
}