using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[ExecuteInEditMode]

public class ToggleSetting : SettingObject
{
    bool value;
    public UnityEvent<bool> valueChangeEvents;

    [SerializeField] string onMessage, offMessage;
    [SerializeField] Sprite iconOn, iconOff;

    Image onOffImage;
    Button activateButton;
    TMP_Text buttonText;


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

        // Setting info
        activateButton.onClick.RemoveAllListeners();
        activateButton.onClick.AddListener(() => { ActivateSetting(); });

        ResetSetting();
    }

    public void ActivateSetting() {
        value = !value;
        ResetSetting();
    }
    public void ResetSetting() {
        if (value) { TurnOn(); }
        else { TurnOff(); }
    }
    void TurnOn() {
        value = true;
        buttonText.text = onMessage;
        onOffImage.sprite = iconOn;
        valueChangeEvents.Invoke(value);
    }
    void TurnOff() {
        value = false;
        buttonText.text = offMessage;
        onOffImage.sprite = iconOff;
        valueChangeEvents.Invoke(value);
    }


    // ----- GET AND SET VALUE -----

    override public string ValueToString() {
        valueString = (value ? 1 : 0).ToString();
        return valueString;
    }
    override public void SetValue(string value) {
        bool isInt = int.TryParse(value, out int temp);
        if (!isInt || (temp != 1 && temp != 0)) {
            Debug.LogWarning($"ToggleSetting: Inputted string \"{value}\" is not a 1 or a 0. Returning.");
            return;
        }
        this.value = temp == 1;
        Reset();
        return;
    }

    // ----- UTILITIES -----

    // Make sure each object is good
    bool GetObjects() {
        bool allGood = true;
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

#if UNITY_EDITOR
    void Update() {
        if (!Application.isPlaying) { Reset(); }
    }
#endif

}
