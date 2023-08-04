using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace PlayerInfo.Settings {

[ExecuteInEditMode]

public class SettingObject : MonoBehaviour
{
    [HideInInspector] public SettingsMenu settingsMenu;
    protected TMP_Text titleText;
    //InfoPopup info;

    List<ImageTheme> imageThemes;


    // Start is called before the first frame update
    void Start() {
        Debug.Log("SettingObject: Base class should no be starting >:(");
    }

    // Setup setting name, info, color
    public void Reset() {
        if (!GetObjects()) {
            //Debug.LogWarning("SettingObject: GetObjects() is false on " + gameObject.name);
            return;
        }

        // Title and setting size
        float size = gameObject.GetComponent<RectTransform>().rect.height;
        RectTransform titleRect = titleText.transform.parent.GetComponent<RectTransform>();
        float height = Screen.height * (settingsMenu.titleSizeMultiplier - settingsMenu.spacingSizeMultiplier);
        titleRect.sizeDelta = new Vector2(0f, height);
        titleRect.anchoredPosition = new Vector2(0f, 0f);

        RectTransform settingRect = gameObject.transform.Find("Setting").GetComponent<RectTransform>();
        height = size - (Screen.height * settingsMenu.titleSizeMultiplier);
        settingRect.sizeDelta = new Vector2(0f, height);
        settingRect.anchoredPosition = new Vector2(0f, 0f);
    }

    void GetImageThemes() {
        imageThemes = new List<ImageTheme>();
        findThemes(transform);
    }
    void findThemes(Transform parent) {
        ImageTheme theme = parent.GetComponent<ImageTheme>();
        if (theme != null) {
            imageThemes.Add(theme);
        }
        foreach (Transform child in parent) {
            findThemes(child);
        }
    }


    // ----- UTILITIES -----

    // Make sure each object is good
    bool GetObjects() {
        bool allGood = true;
        if (settingsMenu == null) {
            allGood = false;
        }
        if (titleText == null) {
            titleText = gameObject.GetComponentInChildren<TMP_Text>();
            if (titleText == null) {
                Debug.LogError("No title text on setting: " + gameObject.name);
                allGood = false;
            }
        }
        if (imageThemes == null) { GetImageThemes(); }
        foreach (ImageTheme theme in imageThemes) {
            if (theme == null) {
                GetImageThemes();
                return true;
            }
        }
        return allGood;
    }
}}