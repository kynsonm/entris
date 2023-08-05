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

    [HideInInspector] public int childIndex;
    List<ImageTheme> imageThemes;

    VerticalLayoutEditor verticalLayoutEditor;
    int verticalLayoutObjectIndex;


    // Start is called before the first frame update
    void Start() {
        Debug.Log("SettingObject: Base class should no be starting >:(");
    }

    // Setup setting name, info, color
    public void Reset() {
        if (!GetObjects()) {
            Debug.LogWarning("SettingObject: GetObjects() is false on " + gameObject.name);
            return;
        }

        // title size = screen.height * settingsMenu.titleSizeMultiplier
        // setting size = object size - title size - (screen * settingsMenu.spacing mult)
        float parentHeight = findViewport(transform).rect.height;
        float size = parentHeight * settingsMenu.baseSettingSizeMultiplier;

        verticalLayoutEditor.objects[verticalLayoutObjectIndex].size = size / parentHeight;

        // Title size
        //float size = gameObject.GetComponent<RectTransform>().rect.height;
        RectTransform titleRect = titleText.transform.parent.GetComponent<RectTransform>();
        //float height = parentHeight * settingsMenu.titleSizeMultiplier;
        float height = size * settingsMenu.titleSizeMultiplier;
        titleRect.sizeDelta = new Vector2(0f, height);
        titleRect.anchoredPosition = new Vector2(0f, 0f);

        // Setting size
        RectTransform settingRect = gameObject.transform.Find("Setting").GetComponent<RectTransform>();
        height = size - height * (1f + settingsMenu.spacingSizeMultiplier);
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

    RectTransform findViewport(Transform transform) {
        if (transform == null) { return null; }
        ScrollRect scroll = transform.GetComponent<ScrollRect>();
        if (scroll == null) {
            return findViewport(transform.parent);
        }
        return scroll.viewport;
    }

    // ----- UTILITIES -----

    // Make sure each object is good
    bool GetObjects() {
        bool allGood = true;
        if (settingsMenu == null) {
            allGood = false;
        }
        if (verticalLayoutEditor == null) {
            verticalLayoutEditor = transform.parent.GetComponent<VerticalLayoutEditor>();
            if (verticalLayoutEditor == null) {
                allGood = false;
            }
        }
        if (verticalLayoutEditor != null) {
            for (int i = 0; i < verticalLayoutEditor.objects.Count; ++i) {
                if (verticalLayoutEditor.objects[i].gameObject == gameObject) {
                    verticalLayoutObjectIndex = i;
                }
            }
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