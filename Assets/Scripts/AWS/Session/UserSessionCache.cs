using UnityEngine;

// Use this class to store user session data to use for refreshing tokens
[System.Serializable]
public class UserSessionCache
{
    [SerializeField] string _idToken;
    [SerializeField] string _accessToken;
    [SerializeField] string _refreshToken;
    [SerializeField] string _userId;

    public UserSessionCache() { }
    public UserSessionCache(string idToken, string accessToken, string refreshToken, string userId) {
        _idToken = idToken;
        _accessToken = accessToken;
        _refreshToken = refreshToken;
        _userId = userId;
    }

    public string idToken {
        get { return _idToken; }
    }

    public string accessToken {
        get { return _accessToken; }
    }

    public string refreshToken{
        get { return _refreshToken; }
    }

    public string userId {
        get { return _userId; }
    }

    public string ToJson() {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonToLoadFrom) {
        JsonUtility.FromJsonOverwrite(jsonToLoadFrom, this);
    }

    public string FileNameToUseForData() {
        return "bad_data_01.dat";
    }
}