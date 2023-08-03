using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // Objects
    SettingsManager settingsManager;
    [SerializeField] ScrollRectPagesMenu scrollRectPages;

    // Prefabs
    [SerializeField] GameObject toggleSettingPrefab;
    [SerializeField] GameObject sliderSettingPrefab;
    [SerializeField] GameObject selectSettingPrefab;


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


    // 
    public void Reset() {
        DestroySettings();
        CreateSettings();
    }

    void DestroySettings() {
        if (scrollRectPages == null) {
            return;
        }
        foreach (var tab in scrollRectPages.tabs) {
            Transform holder = SettingHolder(tab);
            if (holder == null) { continue; }

            foreach (Transform child in holder) {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    void CreateSettings() {
        if (scrollRectPages == null) {
            return;
        }

        // Get the achievements data from the manager
        List<SettingsGroup> settings = new List<SettingsGroup>(); // IDK!!
        if (settings == null) {
            Debug.Log("No achievements. Returning");
            return;
        }

        var tabs = scrollRectPages.tabs;

        // Make new settings for each
        for (int i = 0; i < settings.Count; ++i) {
            if (i > tabs.Count - 1) { break; }
            Transform holder = SettingHolder(tabs[i]);

            SettingsGroup group = settings[i];
            
        }
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
}
