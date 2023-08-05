using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;

namespace PlayerInfo.Settings {


// Parent class for a setting (NO OBJECTS)
// -- name and description
// -- value string (for saving to json)
// -- color for all images in children
// -- -- control if this does anything w/ prefabs/Imagethemes
[System.Serializable]
public class SettingClass
{
    // toggle: "false" for false, "true" for true
    // slider: float.ToString(), float.TryParse()
    // select: int.ToString(), int.TryParse()
    [HideInInspector] public virtual string valueString { get; private set; }

    [SerializeField] public string name;
    [SerializeField] public string info;

    [Tooltip("Color of each image in the setting")]
    public ThemeColor color;

    [SerializeField] protected UnityEvent loadValueEvent;

    virtual public GameObject CreateObject(SettingsMenu settingsMenu, Transform parent, int index) {
        throw new System.NotImplementedException();
    }

    // For editor things
    [HideInInspector] public int editorIndex;
    public bool checkIndex(int currentSettingIndex) {
        return editorIndex == currentSettingIndex;
    }
}


// Class for a toggle setting
// -- sprites for the on/off icon
// -- text for the on/off message (on the button)
[System.Serializable]
public class ToggleSetting : SettingClass {
    public override string valueString {
        get {
            return value ? "1" : "0";
        }
    }

    public bool value;
    public UnityEvent<bool> valueChangeEvent;

    [SerializeField] public string onMessage, offMessage;
    [SerializeField] public Sprite onIcon, offIcon;

    override public GameObject CreateObject(SettingsMenu settingsMenu, Transform parent, int index) {
        GameObject obj = SettingClassUtility.Instantiate(settingsMenu.toggleSettingPrefab, parent);

        ToggleSettingObject toggle = obj.GetComponent<ToggleSettingObject>();
        if (toggle != null) {
            toggle.settingsMenu = settingsMenu;
            toggle.setting = this;
            //toggle.Reset();
        } else {
            Debug.Log($"No ToggleSettingObject on toggle setting object: \'{obj.name}\"");
        }
        editorIndex = index;

        return obj;
    }
}


// Class for a slider setting
// -- min and max values for the slider
// -- event that updates settings class, etc
[System.Serializable]
public class SliderSetting : SettingClass {
    public override string valueString {
        get {
            return value.ToString();
        }
    }

    public float value;
    public UnityEvent<float> valueChangeEvent;

    [SerializeField] public float minValue, maxValue;

    override public GameObject CreateObject(SettingsMenu settingsMenu, Transform parent, int index) {
        GameObject obj = SettingClassUtility.Instantiate(settingsMenu.sliderSettingPrefab, parent);

        SliderSettingObject slider = obj.GetComponent<SliderSettingObject>();
        if (slider != null) {
            slider.settingsMenu = settingsMenu;
            slider.setting = this;
            //slider.Reset();
        } else {
            Debug.Log($"No SliderSettingObject on slider setting object: \'{obj.name}\"");
        }
        editorIndex = index;

        return obj;
    }
}


// Class for a select setting
// -- class for an option
// -- -- icon, text, and value
// -- grid constraint type and count
// -- background sprite
[System.Serializable]
public class SelectSetting : SettingClass {
    [System.Serializable]
    public class Option {
        [SerializeField] public Sprite icon;
        [SerializeField] public string text;
        [SerializeField] public int value = -1;
    }

    public override string valueString {
        get {
            return value.ToString();
        }
    }

    public int value;
    public UnityEvent<int> valueChangeEvent;

    [Header("Option Settings")]
    [SerializeField] public List<Option> options;
    [SerializeField] public GameObject optionPrefab;

    [Header("Grid Setup")]
    [SerializeField] public GridLayoutEditor.ConstraintType gridConstraintType;
    [SerializeField] [Min(0)] public int constraintCount;

    override public GameObject CreateObject(SettingsMenu settingsMenu, Transform parent, int index) {
        GameObject obj = SettingClassUtility.Instantiate(settingsMenu.selectSettingPrefab, parent);

        SelectSettingObject select = obj.GetComponent<SelectSettingObject>();
        if (select != null) {
            select.settingsMenu = settingsMenu;
            select.setting = this;
            //select.Reset();
        } else {
            Debug.Log($"No SliderSettingObject on slider setting object: \'{obj.name}\"");
        }
        editorIndex = index;

        return obj;
    }
}

[System.Serializable]
public class SettingSpacer : SettingClass {
    GameObject prefab = null;

    override public GameObject CreateObject(SettingsMenu settingsMenu, Transform parent, int index) {
        name = "Spacing";
        info = "";
        color = ThemeColor.invalid;
        loadValueEvent = null;

        if (prefab == null) {
            GameObject temp = new GameObject();
            prefab = temp;
            SettingClassUtility.Destroy(temp);
        }
        GameObject obj = SettingClassUtility.Instantiate(prefab, parent);
        editorIndex = index;
        return obj;
    }
}

public static class SettingClassUtility {
    public static GameObject Instantiate(GameObject prefab, Transform parent) {
 #if UNITY_EDITOR
        if (Application.isPlaying) {
            return GameObject.Instantiate(prefab, parent);
        } else {
            return PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
        }
 #else
        return GameObject.Instantiate(prefab, parent);
 #endif
    }

    public static void Destroy(GameObject gameObject) {
 #if UNITY_EDITOR
        if (Application.isPlaying) {
            GameObject.Destroy(gameObject);
        } else {
            GameObject.DestroyImmediate(gameObject);
        }
 #else
        GameObject.Destroy(gameObject);
 #endif
    }
}

}