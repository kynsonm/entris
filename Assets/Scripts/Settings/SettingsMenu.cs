using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInfo.Settings {

public class SettingsMenu : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] ScrollRectPagesMenu scrollRectPages;

    [Header("Sizing")]
    public float titleSizeMultiplier;
    public float spacingSizeMultiplier;

    [Header("Prefabs")]
    [SerializeField] public GameObject toggleSettingPrefab;
    [SerializeField] public GameObject sliderSettingPrefab;
    [SerializeField] public GameObject selectSettingPrefab;


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


    // Destroy and create settings
    public void Reset() {
        DestroySettings();
        CreateSettings();
    }

    // Delete all the current settings in ScrollRectPages
    void DestroySettings() {
        if (!allGood())  { return; }
        foreach (var tab in scrollRectPages.tabs) {
            Transform holder = SettingHolder(tab);
            if (holder == null) { continue; }

            foreach (Transform child in holder) {
                GameObject.Destroy(child.gameObject);
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

            foreach (var setting in settingPages[i].settings) {
                // Creates the setting and also resets it
                GameObject settingObj = setting.CreateObject(this, holder);
                
                // do something? idk
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