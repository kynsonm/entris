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
    // toggle: "false" for false, "true" for true
    // slider: float.ToString(), float.TryParse()
    // select: int.ToString(), int.TryParse()
    protected string valueString;

    public string settingName;
    public string settingInfo;

    [Tooltip("Color of each image in the setting")]
    public ThemeColor settingColor;

    [SerializeField] UnityEvent loadEventBackup;

    SettingsMenu settingsMenu;
    TMP_Text titleText;

    [HideInInspector] public float titleSize { get; private set; }
    [HideInInspector] public VerticalLayoutEditor verticalEditor;


    // Start is called before the first frame update
    void Start() {
        Debug.Log("SettingObject: Base class should no be starting >:(");
    }

    // Setup setting name, info, color
    public void Reset() {
        if (!GetObjects()) {
            Debug.LogWarning("SettingObject: !GetObjects() on " + gameObject.name);
            return;
        }

        gameObject.name = settingName;
        titleText.text = settingName;

        /*
        // Title and setting size
        float size = gameObject.GetComponent<RectTransform>().rect.height;
        RectTransform titleRect = titleText.transform.parent.GetComponent<RectTransform>();
        float height = Screen.height * (settingsMenu.settingMenuTitleSizeMultiplier - settingsMenu.settingMenuSpacingSizeMultiplier);
        titleRect.sizeDelta = new Vector2(0f, height);
        titleRect.anchoredPosition = new Vector2(0f, 0f);

        titleSize = (float)Screen.height * settingsMenu.settingMenuTitleSizeMultiplier;

        RectTransform settingRect = gameObject.transform.Find("Setting").GetComponent<RectTransform>();
        height = size - (Screen.height * settingsMenu.settingMenuTitleSizeMultiplier);
        settingRect.sizeDelta = new Vector2(0f, height);
        settingRect.anchoredPosition = new Vector2(0f, 0f);
        */
    }

    List<ImageTheme> imageThemes;
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


    // ----- GET AND SET VALUE -----

    virtual public string ValueToString() {
        return "SettingObject: Base class should not have a value >:(";
    }
    virtual public void SetValue(string value) {
        Debug.Log("SettingObject: Base class should not have a value >:(");
        return;
    }


    // ----- UTILITIES -----

    // Make sure each object is good
    bool GetObjects() {
        bool allGood = true;
        /*
        if (settingsMenu == null) {
            settingsMenu = GameObject.FindObjectOfType<SettingsMenu>();
            if (settingsMenu == null) {
                Debug.LogError("No settingsMenu on setting: " + gameObject.name);
                allGood = false;
            }
        }
        */
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
        if (verticalEditor == null) {
            verticalEditor = transform.parent.GetComponent<VerticalLayoutEditor>();
            if (verticalEditor == null) {
                Debug.Log("Vertical editor is null");
            }
        }
        return allGood;
    }
}}