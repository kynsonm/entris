using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonFunctions : MonoBehaviour
{
    // Managers
    MenuMovement menuMovement;
    MainMenuManager mainMenuManager;

    public void BackButtonPress() {
        if (!GetObjects()) { return; }
        mainMenuManager.GoBack();
    }

    public void InfoButtonPress() {

    }

    public void ProfileButtonPress() {

    }

    bool GetObjects() {
        bool allGood = true;
        if (menuMovement == null) {
            menuMovement = GameObject.FindObjectOfType<MenuMovement>();
            if (menuMovement == null) {
                Debug.Log("No MenuMovement in the scene");
                allGood = false;
            }
        }
        if (mainMenuManager == null) {
            mainMenuManager = GameObject.FindObjectOfType<MainMenuManager>();
            if (mainMenuManager == null) {
                Debug.Log("No MainMenuManager in the scene");
                allGood = false;
            }
        }
        return allGood;
    }
}
