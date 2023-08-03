using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] public List<SettingsGroup> settings;

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


    // ----- Variable Saving -----

    // TODO: 
}
