using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInfo.Settings;

public class SettingsManager : MonoBehaviour
{
    // ----- CONTAINER/GROUP CLASSES -----

    [System.Serializable]
    public class SettingGroup {
        [SerializeReference]
        public List<SettingClass> settings;
    }

    [System.Serializable]
    public class SettingGroup_Container {
        [SerializeField]
        public SettingGroup settingGroup;
    }

    [SubclassList.SubclassList(typeof(SettingClass)), SerializeField]
    public SettingGroup allSettings;
    

    // ----- MONOBEHAVIOUR -----
    void Awake() {
        Settings.Load();
        CheckSettings();
        Start();
    }
    void OnEnable() {
        Start();
    }
    void Start() {
        StopAllCoroutines();
        StartCoroutine(SaveSettingsEnum());
    }


    // ----- Checking, Saving, and Loading -----

    void CheckSettings() {
        Settings.animationTime = (Settings.animationTime <= 0.05f) ? 1f : Settings.animationTime;
    }

    IEnumerator SaveSettingsEnum() {
        yield return new WaitForEndOfFrame();
        CheckSettings();
        Settings.Save();
        while (true) {
            yield return new WaitForSeconds(3f);
            Settings.Save();
        }
    }


    // ----- STATIC METHODS -----

    // Yeah idk, pretty self explanatory
    public static SettingsManager settingsManager;
    static bool ManagerIsGood() {
        if (settingsManager == null) {
            settingsManager = GameObject.FindObjectOfType<SettingsManager>();
            if (settingsManager == null) {
                return false;
            }
        }
        return true;
    }

    // Get all the achievements
    public static List<SettingGroup> settings() {
        if (!ManagerIsGood()) { return null;

    }
        List<SettingGroup> settings = new List<SettingGroup>{
            settingsManager.allSettings
        };
        return settings;
        //return settingsManager.allSettings;
    }


    // ----- SETTING OBJECT Loading/OnUpdate -----
    // Very redundant but it works for now

    // int themeIndex
    public void LoadThemeIndex(SelectSetting selectSetting) {
        selectSetting.value = Settings.themeIndex;
    }
    public void SetThemeIndex(int index) {
        Settings.themeIndex = index;
        Theme.SetTheme(index);
        Settings.Save();
    }

    // int fontIndex
    public void LoadFontIndex(SelectSetting selectSetting) {
        selectSetting.value = Settings.fontIndex;
    }
    public void SetFontIndex(int index) {
        Settings.fontIndex = index;
        Theme.SetFont(index);
        Settings.Save();
    }

    // int blocksIndex
    public void LoadBlocksIndex(SelectSetting selectSetting) {
        selectSetting.value = Settings.blocksIndex;
    }
    public void SetBlocksIndex(int index) {
        Settings.blocksIndex = index;
        Theme.SetBlocks(index);
        Settings.Save();
    }

    // int backgroundIndex
    public void LoadBackgroundIndex(SelectSetting selectSetting) {
        selectSetting.value = Settings.backgroundIndex;
    }
    public void SetBackgroundIndex(int index) {
        // TODO: idk
    }

    // bool backgroundIsActive
    public void LoadBackgroundActive(ToggleSetting toggleSetting) {
        toggleSetting.value = Settings.backgroundIsActive;
    }
    public void SetBackgroundActive(bool isActive) {
        // TODO: idk
    }

    // float animationTime
    public void LoadAnimationTime(SliderSetting sliderSetting) {
        sliderSetting.value = Settings.animationTime;
    }
    public void SetAnimationTime(float time) {
        Settings.animationTime = (time < 0.05f) ? 0.05f : time;
    }

    // bool dyanamicControls
    public void LoadDynamiControls(ToggleSetting toggleSetting) {
        toggleSetting.value = Settings.dynamicControls;
    }
    public void SetDynamicControls(bool isActive) {
        Settings.dynamicControls = isActive;
    }
}