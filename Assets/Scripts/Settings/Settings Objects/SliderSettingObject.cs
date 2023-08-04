using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace PlayerInfo.Settings {

[ExecuteInEditMode]

public class SliderSettingObject : SettingObject
{
    public SliderSetting setting;

    Slider slider;
    SliderTheme sliderTheme;


    // Start is called before the first frame update
    void OnEnable() { StartCoroutine(Start()); }
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        Reset();
    }
 #if UNITY_EDITOR
    void Update() {
        if (!Application.isPlaying) { Reset(); }
    }
 #endif


    // Set each objects stuffff
    new public void Reset() {
        if (!GetObjects()) { return; }
        base.Reset();
        gameObject.name = (setting.name == "") ? "Slider Setting" : setting.name;
        titleText.text = setting.name;

        // Slider info
        slider.minValue = setting.minValue;
        slider.maxValue = setting.maxValue;

        // Setting info
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener((float value) => { setting.valueChangeEvent.Invoke(value); });

        if (sliderTheme == null) { return; }
        sliderTheme.backgroundColor = setting.color;
        sliderTheme.fillColor = setting.color;
        sliderTheme.knobColor = setting.color;
        sliderTheme.ResetColor();
    }


    // ----- GET AND SET VALUE -----

    public void SetValue(string value) {
        bool isFloat = float.TryParse(value, out float temp);
        if (!isFloat) {
            Debug.LogWarning($"SliderSetting: Inputted string \"{value}\" is not a float. Returning.");
            return;
        }
        setting.value = temp;
        Reset();
        return;
    }

    // ----- UTILITIES -----

    // Make sure each object is good
    bool GetObjects() {
        bool allGood = true;
        if (setting == null) {
            allGood = false;
        }
        if (slider == null) {
            slider = gameObject.transform.GetComponentInChildren<Slider>();
            if (slider == null) {
                Debug.Log("No slider found on slider setting: " + gameObject.name);
                allGood = false;
            }
        }
        if (allGood && sliderTheme == null) {
            sliderTheme = slider.gameObject.GetComponent<SliderTheme>();
        }
        return allGood;
    }

}}