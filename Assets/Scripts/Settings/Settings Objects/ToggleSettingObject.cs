using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace PlayerInfo.Settings {

[ExecuteInEditMode]

public class ToggleSettingObject : SettingObject
{
    public ToggleSetting setting;

    [SerializeField] Image onOffImage;
    [SerializeField] Button activateButton;
    [SerializeField] TMP_Text buttonText;

    // Start is called before the first frame update
    void OnEnable() { StartCoroutine(Start()); }
    IEnumerator Start() {
        yield return new WaitForSeconds(1f);
        Reset();
    }
 #if UNITY_EDITOR
    void Update() {
        if (!Application.isPlaying) { Reset(); }
    }
 #endif


    // Set each objects stuffff
    new public void Reset() {
        if (!GetObjects()) {
            Debug.LogWarning($"ToggleSettingObject: Can't reset \"{gameObject.name}\"");
            return;
        }
        base.Reset();
        gameObject.name = (setting.name == "") ? "Toggle Setting" : setting.name;
        titleText.text = setting.name;

        // Setting info
        activateButton.onClick.RemoveAllListeners();
        activateButton.onClick.AddListener(() => { ActivateSetting(); });

        ResetSetting();
    }

    // Turning on and off the setting
    public void ActivateSetting() {
        setting.value = !setting.value;
        setting.valueChangeEvent.Invoke(setting.value);
        ResetSetting();
    }
    public void ResetSetting() {
        if (setting.value) { TurnOn(); }
        else { TurnOff(); }
    }
    void TurnOn() {
        buttonText.text = setting.onMessage;
        onOffImage.sprite = setting.onIcon;
    }
    void TurnOff() {
        buttonText.text = setting.offMessage;
        onOffImage.sprite = setting.offIcon;
    }


    // ----- GET AND SET VALUE -----

    public void SetValue(string value) {
        bool isInt = int.TryParse(value, out int temp);
        if (!isInt || (temp != 1 && temp != 0)) {
            Debug.LogWarning($"ToggleSetting: Inputted string \"{value}\" is not a 1 or a 0. Returning.");
            return;
        }
        setting.value = temp == 1;
        Reset();

        global::Settings.Save();

        return;
    }


    // ----- UTILITIES -----

    // Make sure each object is good
    bool GetObjects() {
        bool allGood = true;
        if (setting == null) {
            allGood = false;
        }
        if (activateButton == null) {
            activateButton = gameObject.transform.Find("Setting").GetComponentInChildren<Button>();
            if (activateButton == null) {
                Debug.LogError("No button on activate setting: " + gameObject.name);
                allGood = false;
            } else {
                buttonText = activateButton.transform.Find("Button").GetComponentInChildren<TMP_Text>();
            }
        } else {
            if (buttonText == null) {
                buttonText = activateButton.transform.GetComponentInChildren<TMP_Text>();
            }
        }
        if (onOffImage == null) {
            onOffImage = gameObject.transform.Find("Setting").Find("Icon").GetComponent<Image>();
            if (onOffImage == null) {
                Debug.LogError("No icon image on activate setting: " + gameObject.name);
                allGood = false;
            }
        }
        return allGood;
    }

}}
