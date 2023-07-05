using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    MenuMovement menuMovement;
    MainMenuButtonFunctions buttonFunctions;

    [Header("Interaction")]
    [SerializeField] List<GameObject> inactivateOnTween;
    List<CanvasGroup> menuCanvases;

    [Header("Opening Menus")]
    [SerializeField] GameObject mainMenuButtonsMenu;
    [SerializeField] List<GameObject> openedMenus;

    void Awake() {
        openedMenus = new List<GameObject>();
        openedMenus.Add(mainMenuButtonsMenu);

        menuCanvases = new List<CanvasGroup>();
        foreach (GameObject obj in inactivateOnTween) {
            if (obj == null) { continue; }
            menuCanvases.Add(GetCanvas(obj));
        }

        StartCoroutine(TurnOffObjectsOnStart());
    }

    IEnumerator TurnOffObjectsOnStart() {
        // Turn everything on and save its "on" status
        List<bool> objectWasOn = new List<bool>();
        foreach (GameObject obj in inactivateOnTween) {
            if (obj == null) {
                objectWasOn.Add(false);
                continue;
            }
            objectWasOn.Add(obj.activeSelf);
            obj.SetActive(true);
        }
        // Wait x amount of time
        for (int i = 0; i < 5; ++i) {
            yield return new WaitForEndOfFrame();
        }
        // Turn everything back off (or to what it was on)
        for (int i = 0; i < inactivateOnTween.Count; ++i) {
            if (inactivateOnTween[i] == null) { continue; }
            inactivateOnTween[i].SetActive(objectWasOn[i]);
        }
    }

    public void ActivateObjects(GameObject objectTweening) {
        StopAllCoroutines();
        StartCoroutine(ActivateObjectsEnum(objectTweening));
    }
    IEnumerator ActivateObjectsEnum(GameObject objectTweening) {
        foreach (CanvasGroup canv in menuCanvases) {
            canv.interactable = false;
            canv.blocksRaycasts = false;
        }
        while (objectTweening.LeanIsTweening()) {
            yield return new WaitForEndOfFrame();
        }
        foreach (CanvasGroup canv in menuCanvases) {
            canv.interactable = true;
            canv.blocksRaycasts = true;
        }
    }


    // ----- OPENING and CLOSING -----

    public void GoBack() {
        if (!GetObjects()) { return; }

        GameObject toClose = NextMenuToClose();
        menuMovement.Close(toClose);

        GameObject toOpen = NextMenuToClose();
        if (toClose == null && toOpen == null) { return; }
        if (toOpen == null && !mainMenuButtonsMenu.activeInHierarchy) {
            toOpen = mainMenuButtonsMenu;
        }

        menuMovement.Open(toOpen, false);
    }

    public void OpenMenu(GameObject menu) {
        if (!GetObjects()) { return; }
        openedMenus.Add(menu);
    }

    public void CloseMenu(GameObject menu) {
        if (!GetObjects()) { return; }
        if (openedMenus.Count == 0) {
            Debug.Log("No opened menus to remove");
            return;
        }
        // Don't close the buttons
        if (menu == mainMenuButtonsMenu) {
            Debug.Log("Can't close the buttons menu");
            return;
        }
        // Remove the menu from opened menus
        for (int i = 0; i < openedMenus.Count; ++i) {
            if (openedMenus[i] == menu) {
                openedMenus.RemoveAt(i);
                return;
            }
        }
        // It wasn't found
        Debug.Log($"Menu \"{menu.name}\" could not be found in openedMenus");
    }

    public GameObject NextMenuToClose() {
        if (!GetObjects()) { return null; }
        if (NoMenusToClose()) {
            Debug.Log("No opened menus to close");
            return null;
        }
        return openedMenus[openedMenus.Count-1];
    }

    public bool NoMenusToClose() {
        if (!GetObjects()) { return true; }
        if (openedMenus.Count == 0) { return true; }
        if (openedMenus.Count == 1) {
            return true;
        }
        return false;
    }

    public bool GameObjectIsButtonsMenu(GameObject obj) {
        return obj == mainMenuButtonsMenu;
    }

    // ----- UTILITIES -----

    bool GetObjects() {
        bool allGood = true;
        if (menuMovement == null) {
            menuMovement = GameObject.FindObjectOfType<MenuMovement>();
            if (menuMovement == null) {
                Debug.Log("No MenuMovement in the scene");
                allGood = false;
            }
        }
        if (buttonFunctions == null) {
            buttonFunctions = GameObject.FindObjectOfType<MainMenuButtonFunctions>();
            if (buttonFunctions == null) {
                Debug.Log("No MainMenuManager in the scene");
                allGood = false;
            }
        }
        if (mainMenuButtonsMenu == null) {
            Debug.Log("No buttons menu set");
            allGood = false;
        }
        if (openedMenus == null) {
            openedMenus = new List<GameObject>();
        }
        return allGood;
    }

    CanvasGroup GetCanvas(GameObject obj) {
        CanvasGroup canv = obj.GetComponent<CanvasGroup>();
        if (canv == null) {
            canv = obj.AddComponent<CanvasGroup>();
        }
        return canv;
    }
}
