using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInfo.Settings;

public class SettingsManager : MonoBehaviour
{
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

    [BF_SubclassList.SubclassList(typeof(SettingClass)), SerializeField]
    public SettingGroup allSettings;
    

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
        if (!ManagerIsGood()) { return null; }
        List<SettingGroup> settings = new List<SettingGroup>{
            settingsManager.allSettings
        };
        return settings;
        //return settingsManager.allSettings;
    }
}