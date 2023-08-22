using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveData
{
    private static string pathPrefix = "";
    private const string userSessionCachePrefix = "/";

    // Some methods can only be called in the main thread
    // -- This sets those values on startup
    static SaveData() { Init(); }
    public static void Init() {
        pathPrefix = Application.persistentDataPath + "/";
    }

    // ---- SAVING -----

    public static void Save(string jsonData, string fileName) {
        string path = pathPrefix + fileName;

        Debug.Log("Writing to path: " + path + "\n" + jsonData);

        File.WriteAllText(path, jsonData);
    }

    public static void Save(UserSessionCache userSessionCache) {
        string jsonData = userSessionCache.ToJson();
        string fileName = userSessionCachePrefix + userSessionCache.FileNameToUseForData();

        Debug.Log("Saving to file: " + fileName + "\n" + jsonData);

        Save(jsonData, fileName);
    }


    // ----- LOADING -----

    public static void Load() {
        Debug.Log("Youre doing this wrong");
    }

    public static UserSessionCache Load(UserSessionCache userSessionCache) {
        string fileName = userSessionCachePrefix + userSessionCache.FileNameToUseForData();
        string content = File.ReadAllText(pathPrefix + fileName);
        return JsonUtility.FromJson<UserSessionCache>(content);
    }
}
