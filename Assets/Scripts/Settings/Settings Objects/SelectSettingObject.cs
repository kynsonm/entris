using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace PlayerInfo.Settings {

[ExecuteInEditMode]

public class SelectSettingObject : SettingObject
{
    int value;
    public UnityEvent<int> valueChangeEvents;

    [Header("Option Settings")]
    [SerializeField] List<Option> options;
    [SerializeField] GameObject optionPrefab;
    Sprite defaultIcon;
    Transform optionParent;

    [Header("Grid Setup")]
    [SerializeField] GridLayoutEditor.ConstraintType gridConstraintType;
    [SerializeField] [Min(0)] int constraintCount;
    GridLayoutEditor gridEditor;


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

        AdjustOptionObjects();
        SetupGridLayout();

        // Setting info
        for (int i = 0; i < options.Count; ++i) {
            Option option = options[i];
            GameObject optionObject = optionParent.GetChild(i).gameObject;
            option.gameObject = optionObject;
            //option.Reset(this);
        }

        for (int i = 0; i < verticalEditor.objects.Count; ++i) {
            if (!verticalEditor.objects[i].Equals(gameObject)) { continue; }
            verticalEditor.objects[i].size = 1.05f * (titleSize + gridEditor.size) / (float)Mathf.Min(Screen.height, Screen.width);
        }
    }

    // Create the option objects
    void AdjustOptionObjects() {
        Transform parent = transform.Find("Setting").Find("Grid");
        // Destroy whats already there
        for (int i = parent.childCount; i > options.Count; --i) {
#if UNITY_EDITOR
            if (Application.isPlaying) { GameObject.Destroy(parent.GetChild(i-1).gameObject); }
            else { GameObject.DestroyImmediate(parent.GetChild(i-1).gameObject); }
#else
            GameObject.Destroy(parent.GetChild(i-1).gameObject);
#endif
        }
        // Create new ones
        for (int i = parent.childCount; i < options.Count; ++i) {
#if UNITY_EDITOR
            if (Application.isPlaying) { GameObject.Instantiate(optionPrefab, parent); }
            else { PrefabUtility.InstantiatePrefab(optionPrefab, parent); }
#else
            GameObject.Instantiate(optionPrefab, parent);
#endif

            options[i].icon = defaultIcon;
            options[i].value = -1;
            options[i].showValueText = true;
        }
    }

    // Setup grid layout
    void SetupGridLayout() {
        gridEditor.constraintType = gridConstraintType;
        gridEditor.constraintCount = constraintCount;
    }


    // ----- GET AND SET VALUE -----

    override public string ValueToString() {
        valueString = value.ToString();
        return valueString;
    }
    override public void SetValue(string value) {
        bool isInt = int.TryParse(value, out int temp);
        if (!isInt) {
            Debug.LogWarning($"SelectSetting: Inputted string \"{value}\" is not an integer. Returning.");
            return;
        }
        this.value = temp;
        Reset();
        return;
    }


    // ----- UTILITIES -----

#if UNITY_EDITOR
    void Update() {
        if (!Application.isPlaying) { Reset(); }
    }
#endif

    // Make sure each object is good
    bool GetObjects() {
        bool allGood = true;
        if (optionPrefab == null) {
            Debug.Log("No option prefab on SelectSetting: " + gameObject.name);
            allGood = false;
        }
        if (defaultIcon == null) {
            Image img = optionPrefab.GetComponent<Image>();
            if (img == null) {
                img = optionPrefab.GetComponentInChildren<Image>();
            }
            if (img != null) {
                defaultIcon = img.sprite;
            }
            
            if (defaultIcon == null) {
                Debug.Log("No default icon");
                allGood = false;
            }
        }
        if (optionParent == null) {
            optionParent = transform.Find("Setting").Find("Grid");
            if (optionParent == null) {
                Debug.Log("No option parent on SelectSetting: " + gameObject.name);
                return false;
            }
        }
        if (gridEditor == null) {
            gridEditor = optionParent.GetComponent<GridLayoutEditor>();
            if (gridEditor == null) {
                Debug.Log("No grid layout editor on SelectSetting: " + gameObject.name);
                allGood = false;
            }
        }
        return allGood;
    }

    // Holds each option in this setting to change its info
    [System.Serializable]
    public class Option {
        [SerializeField] bool dontUpdateLooks = false;
        [HideInInspector] public GameObject gameObject;
        public Sprite icon;
        public int value = -1;
        public bool showValueText = true;

        Button button;
        Image image;
        TMP_Text text;

        public Option(Option toCopy) {
            icon = toCopy.icon;
            value = toCopy.value;
            showValueText = toCopy.showValueText;
            button = toCopy.button;
            image = toCopy.image;
            text = toCopy.text;
        }
/*
        public void Reset(SelectSetting selectSetting) {
            if (!dontUpdateLooks && GetLooksObjects()) {
                image.sprite = icon;
                if (showValueText) {
                    text.text = value.ToString();
                } else {
                    text.text = "";
                }
            }

            if (!GetImportantObjects()) {
                Debug.Log("Option on SelectSetting " + selectSetting.gameObject.name + " is bad. Returning");
                return;
            }
            gameObject.name = value.ToString() + " Option";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { selectSetting.valueChangeEvents.Invoke(value); });
        }
*/

        bool GetLooksObjects() {
            if (gameObject == null) {
                return false;
            }
            if (image == null) {
                image = gameObject.GetComponent<Image>();
                if (image == null) {
                    image = gameObject.GetComponentInChildren<Image>();
                }
                if (image == null) {
                    return false;
                }
            }
            if (text == null) {
                text = gameObject.GetComponent<TMP_Text>();
                if (text == null) {
                    text = gameObject.GetComponentInChildren<TMP_Text>();
                }
                if (text == null) {
                    return false;
                }
            }
            return true;
        }

        bool GetImportantObjects() {
            if (gameObject == null) {
                return false;
            }
            if (button == null) {
                button = gameObject.GetComponent<Button>();
                if (button == null) {
                    button = gameObject.GetComponentInChildren<Button>();
                }
                if (button == null) {
                    return false;
                }
            }
            return true;
        }
    }

}}
