using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DEBUG_LogToFile : MonoBehaviour
{
    [SerializeField] string fileName;
    string path = "";
    
    static string fullLog = "";
    static int logCount = 0;


    void Awake() {
        path = Application.persistentDataPath + "/" + fileName + ".txt";
        Application.logMessageReceived += LogMessage;

        Debug.Log("Persistent data path: \"" + Application.persistentDataPath + "\"");
    }

    public void LogMessage(string condition, string stackTrace, LogType type) {
        ++logCount;
        fullLog += "( " + logCount + " ) : " + type.ToString() + "\n";
        fullLog += condition + "\n";
        fullLog += ":: " + stackTrace.Replace("\n", "\n :: ") + "\n\n";

        File.WriteAllText(path, fullLog);
    }
}
