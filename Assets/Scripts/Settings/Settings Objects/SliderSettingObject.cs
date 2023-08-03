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
    float value;
    public UnityEvent<float> valueChangeEvents;

    [SerializeField] float minValue, maxValue;

    Slider slider;
    SliderTheme sliderTheme;


    // Start is called before the first frame update
    void OnEnable() { StartCoroutine(Start()); }
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        Reset();
    }

    // Set each objects stuffff
    new public void Reset() {
        if (!GetObjects()) { return; }
        base.Reset();

        // Slider info
        slider.minValue = minValue;
        slider.maxValue = maxValue;

        // Setting info
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener((float value) => { valueChangeEvents.Invoke(value); });

        if (sliderTheme == null) { return; }
        sliderTheme.backgroundColor = settingColor;
        sliderTheme.fillColor = settingColor;
        sliderTheme.knobColor = settingColor;
        sliderTheme.ResetColor();
    }


    // ----- GET AND SET VALUE -----

    override public string ValueToString() {
        valueString = value.ToString();
        return valueString;
    }
    override public void SetValue(string value) {
        bool isFloat = float.TryParse(value, out float temp);
        if (!isFloat) {
            Debug.LogWarning($"SliderSetting: Inputted string \"{value}\" is not a float. Returning.");
            return;
        }
        this.value = temp;
        Reset();
        return;
    }

    // ----- UTILITIES -----

    // Make sure each object is good
    bool GetObjects() {
        bool allGood = true;
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

#if UNITY_EDITOR
    void Update() {
        if (!Application.isPlaying) { Reset(); }
    }
#endif

}
}