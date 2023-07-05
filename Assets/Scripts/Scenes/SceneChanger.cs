using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [System.Serializable]
    public enum Scene {
        main_menu = 0, game = 1
    }

    public void ChangeScene(int scene) {
        ChangeScene((Scene)scene);
    }
    public void ChangeScene(Scene scene) {
        SceneManager.LoadScene((int)scene);
    }
}
