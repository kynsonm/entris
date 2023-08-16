using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    // Holds each scene's integer value
    // -- Set this in PlayerPrefs btw
    [System.Serializable]
    public enum Scene {
        main_menu = 0,
        game = 1
    }

    // Use this one in the editor
    public void ChangeScene(int scene) {
        ChangeScene((Scene)scene);
    }

    // Use this one in scripts
    public void ChangeScene(Scene scene) {

        Debug.Log("CHANGING SCENE TO \"" + scene.ToString().ToUpper() + "\"");

        SceneManager.LoadScene((int)scene);
    }
}
