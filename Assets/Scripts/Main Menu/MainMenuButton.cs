using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]

// Make sure that this is actually on a main menu button (ish)
[RequireComponent(typeof(HorizontalLayoutGroup))]
[RequireComponent(typeof(HorizontalLayoutEditor))]
[RequireComponent(typeof(Button))]
public class MainMenuButton : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] Sprite logoSprite;
    [SerializeField] string buttonText;
    [Header("Button Stuff")]
    [SerializeField] Button.ButtonClickedEvent onClick;
    [Header("Looks")]
    [SerializeField] ThemeColor buttonColorBase;
    [SerializeField] ThemeColor buttonColorMix;
    [SerializeField] [Range(0f, 1f)] float buttonColorMixRatio;
    Button button;
    Image logoImage;
    ImageTheme logoTheme, buttonTheme;
    TMP_Text buttonTMP;
    TextTheme textTheme;

    // Start is called before the first frame update
    void Start() {
        Reset();
    }

    // Set objects
    public void Reset() {
        if (!GetObjects()) { if (!GetObjects()) { return; }}
        logoImage.sprite = logoSprite;
        buttonTMP.text = buttonText;

        logoTheme.color = buttonColorBase;
        logoTheme.useMixedColor = true;
        logoTheme.mixRatio = buttonColorMixRatio;
        logoTheme.mixedColor = buttonColorMix;
        logoTheme.ResetColor();

        buttonTheme.color = buttonColorBase;
        buttonTheme.useMixedColor = true;
        buttonTheme.mixRatio = buttonColorMixRatio;
        buttonTheme.mixedColor = buttonColorMix;
        buttonTheme.ResetColor();

        button.onClick = onClick;
    }


    // ----- GETTING OBJECTS -----

    // Check and get objects if they are bad
    bool GetObjects() {
        if (CheckObjects()) { return true; }
        bool allGood = true;

        button = gameObject.GetComponent<Button>();
        if (button == null) {
            Debug.Log("MainMenuButton: No button component");
            allGood = false;
        }

        logoImage = gameObject.GetComponentInChildren<Image>();
        if (logoImage == null) {
            Debug.Log("MainMenuButton: No logo image in children");
            allGood = false;
        } else {
            logoTheme = logoImage.gameObject.GetComponent<ImageTheme>();
            if (logoTheme == null) { Debug.Log("MainMenuButton: No theme on the logo image"); }
        }

        buttonTMP = gameObject.transform.Find("Button Objects").GetComponentInChildren<TMP_Text>();
        if (buttonTMP == null) {
            Debug.Log("MainMenuButton: Could not find button text");
            allGood = false;
        } else {
            textTheme = buttonTMP.GetComponent<TextTheme>();
        }

        buttonTheme = gameObject.transform.Find("Button Objects").GetComponentInChildren<ImageTheme>();
        if ( buttonTheme == null) {
            Debug.Log("MainMenuButton: Could not find button image theme");
            allGood = false;
        }
        return allGood;
    }
    bool CheckObjects() {
        if (button == null) { return false; }
        if (logoImage == null) { return false; }
        if (logoTheme == null) { return false; }
        if (buttonTheme == null) { return false; }
        if (buttonTMP == null) { return false; }
        if (textTheme == null) { return false; }
        return true;
    }

#if UNITY_EDITOR
    void Update() {
        if (!Application.isPlaying) {
            Reset();
        }
    }
#endif
}
