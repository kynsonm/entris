using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace PlayerInfo.Settings {

public class SettingOption
{
    // toggle: "false" for false, "true" for true
    // slider: float.ToString(), float.TryParse()
    // select: int.ToString(), int.TryParse()
    protected string valueString;

    public string settingName;
    public string settingInfo;

    [Tooltip("Color of each image in the setting")]
    public ThemeColor settingColor;

    [SerializeField] UnityEvent loadEventBackup;
}


public class SliderSetting : SettingOption {
    float value;
    public UnityEvent<float> valueChangeEvent;

    [SerializeField] float minValue, maxValue;
}

public class ToggleSetting : SettingOption {
    bool value;
    public UnityEvent<bool> valueChangeEvent;

    [SerializeField] string onMessage, offMessage;
    [SerializeField] Sprite iconOn, iconOff;
}

public class SelectSetting : SettingOption {
    int value;
    public UnityEvent<int> valueChangeEvent;

    [Header("Option Settings")]
    [SerializeField] List<Option> options;
    [SerializeField] GameObject optionPrefab;

    [Header("Grid Setup")]
    [SerializeField] GridLayoutEditor.ConstraintType gridConstraintType;
    [SerializeField] [Min(0)] int constraintCount;


    [System.Serializable]
    class Option {
        [SerializeField] bool dontUpdateLooks = false;
        [SerializeField] public Sprite icon;
        [SerializeField] public int value = -1;
        [SerializeField] public bool showValueText = true;
    }
}

}