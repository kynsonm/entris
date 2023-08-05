using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInfo.Settings {


[ExecuteInEditMode]
public class SettingsMenu : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] ScrollRectPagesMenu scrollRectPages;

    [Header("Sizing")]
    [Range(0f, 0.5f)] public float baseSettingSizeMultiplier;
    [Range(0f, 1.0f)] public float titleSizeMultiplier;
    [Range(0f, 1.0f)] public float spacingSizeMultiplier;
    [SerializeField] [Min(2f)] float achievementSpacingDivier = 50f;

    [Header("Prefabs")]
    [SerializeField] public GameObject toggleSettingPrefab;
    [SerializeField] public GameObject sliderSettingPrefab;
    [SerializeField] public GameObject selectSettingPrefab;

    [Space(10f)]
    [SerializeField] bool DEBUG_HardReset = false;


    // Monobehaviour stuff
    void Awake() { OnEnable(); }
    void OnEnable() {
        StopAllCoroutines();
        StartCoroutine(Start());
    }
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Reset();
    }
 #if UNITY_EDITOR
    int lastNumberOfTabs;
    List<int> lastNumberOfElements;
    void resetCheckingVariables() {
        lastNumberOfTabs = 0;
        lastNumberOfElements = new List<int>();
        int currentElementCount = 0;
        var settingPages = SettingsManager.settings();
        var tabs = scrollRectPages.tabs;
        for (int i = 0; i < settingPages.Count; ++i) {
            if (i > tabs.Count - 1) { break; }
            for (int j = 0; j < settingPages[i].settings.Count; ++j) {
                ++currentElementCount;
            }
            ++lastNumberOfTabs;
            lastNumberOfElements.Add(currentElementCount);
            currentElementCount = 0;
        }
    }
    void Update() {
        if (DEBUG_HardReset) {
            DEBUG_HardReset = false;
            Reset();
            resetCheckingVariables();
        }
        if (Application.isPlaying) { return; }

        int numberOfTabs = 0;
        List<int> numberOfElements = new List<int>();
        int currentElementCount = 0;

        // Redundant from <Createsettings>
        var settingPages = SettingsManager.settings();
        if (settingPages == null) {
            Debug.Log("No settings found. Returning");
            return;
        }
        var tabs = scrollRectPages.tabs;
        for (int i = 0; i < settingPages.Count; ++i) {
            if (i > tabs.Count - 1) { break; }

            for (int j = 0; j < settingPages[i].settings.Count; ++j) {
                var setting = settingPages[i].settings[j];
                if (!setting.checkIndex(j)) {

                    Debug.Log("Resetting settings from iterative check");

                    Reset();
                    resetCheckingVariables();
                    return;
                }

                ++currentElementCount;
            }

            ++numberOfTabs;
            numberOfElements.Add(currentElementCount);
            currentElementCount = 0;
        }
        // Check sizes of each object/variable
        if (numberOfTabs != lastNumberOfTabs || numberOfElements.Count != lastNumberOfElements.Count) {
            Reset();
            resetCheckingVariables();
            return;
        }
        for (int i = 0; i < lastNumberOfElements.Count; ++i) {
            if (numberOfElements[i] != lastNumberOfElements[i]) {

                Debug.Log("Resetting settings from number of elements check");

                Reset();
                resetCheckingVariables();
                return;
            }
        }
    }
 #endif


    // Destroy and create settings
    public void Reset() {
        Debug.Log("Resetting settings");

        DestroySettings();
        CreateSettings();
    }

    // Delete all the current settings in ScrollRectPages
    void DestroySettings() {
        if (!allGood())  { return; }
        foreach (var tab in scrollRectPages.tabs) {
            Transform holder = SettingHolder(tab);
            if (holder == null) { continue; }

            for (int i = holder.childCount - 1; i >= 0; --i) {
                Transform child = holder.GetChild(i);
 #if UNITY_EDITOR
                if (Application.isPlaying) {
                    GameObject.Destroy(child.gameObject);
                } else {
                    GameObject.DestroyImmediate(child.gameObject);
                }
 #else
                GameObject.Destroy(child.gameObject);
 #endif
            }
        }
    }

    // Create all the current settings in ScrollRectPages
    void CreateSettings() {
        if (!allGood()) { return; }

        var settingPages = SettingsManager.settings();
        if (settingPages == null) {
            Debug.Log("No settings found. Returning");
            return;
        }

        var tabs = scrollRectPages.tabs;

        for (int i = 0; i < settingPages.Count; ++i) {
            if (i > tabs.Count - 1) { break; }
            Transform holder = SettingHolder(tabs[i]);

            for (int j = 0; j < settingPages[i].settings.Count; ++j) {
                var setting = settingPages[i].settings[j];
                // Creates the setting and also resets it
                GameObject settingObj = setting.CreateObject(this, holder, j);
            }

            VerticalLayoutEditor vertEditor = holder.GetComponent<VerticalLayoutEditor>();
            if (vertEditor != null) {
                vertEditor.spacingDivider = achievementSpacingDivier;
            }
        }
    }
    

    bool allGood() {
        if (scrollRectPages == null) {
            return false;
        }
        return true;
    }

    Transform SettingHolder(ScrollRectPagesMenu.TabClass tab) {
        return SettingHolder(tab.paddingRect);
    }
    Transform SettingHolder(Transform padding) {
        if (padding == null || padding.childCount == 0) { return null; }
        
        Transform holder = padding.GetChild(0);
        if (holder == null) { return null; }

        return holder;
    }
}}