using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

[ExecuteInEditMode]

public class SettingsMenu : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject settingAreaPrefab;
    [SerializeField] GameObject settingTabPrefab;

    [Header("Setting Options")]
    [Range(0.001f, 1.0f)] public float settingMenuTitleSizeMultiplier;
    [Range(0.001f, 0.5f)] public float settingMenuSpacingSizeMultiplier;

    [Header("Settings Tabs")]
    [SerializeField] public Transform settingAreasHolder;
    [SerializeField] Transform settingTabsHolder;
    [SerializeField] List<SettingsArea> settingAreas;
    HorizontalLayoutEditor areaLayoutEditor, tabLayoutEditor;


    // Start is called before the first frame update
    void Awake() { StartCoroutine(Start()); }
    IEnumerator Start() {
        // Setting title size
        if (settingMenuTitleSizeMultiplier <= 0.001f) { settingMenuTitleSizeMultiplier = 0.001f; }
        if (settingMenuTitleSizeMultiplier >= 1.000f) { settingMenuTitleSizeMultiplier = 1f; }
        // Setting spacing size
        if (settingMenuSpacingSizeMultiplier <= 0.001f) { settingMenuSpacingSizeMultiplier = 0.001f; }
        if (settingMenuSpacingSizeMultiplier >= 0.500f) { settingMenuSpacingSizeMultiplier = 0.5f; }
        yield return new WaitForEndOfFrame();
        Reset();

        // TODO: Figure out saving and loading
        yield return new WaitForEndOfFrame();
        Save();

    }

#if UNITY_EDITOR
    void Update() {
        if (!Application.isPlaying) { Reset(); }
    }
#endif

    public void Reset() {
        SetupTabs();
        SetupSettingsAreas();
        foreach (SettingsArea area in settingAreas) {
            area.settingsMenuScript = this;
            area.Reset();
        }
        if (!Application.isPlaying) {
            SetupLayouts();
        } else {
            StartCoroutine(SetupLayoutsEnum());
        }
    }

    void SetupLayouts() {
        if (areaLayoutEditor == null) { areaLayoutEditor = settingAreasHolder.GetComponent<HorizontalLayoutEditor>(); }
        if (tabLayoutEditor  == null) { tabLayoutEditor = settingTabsHolder.GetComponent<HorizontalLayoutEditor>(); }
        float areaSize = areaLayoutEditor.transform.parent.GetComponent<RectTransform>().rect.width / Mathf.Min(Screen.width, Screen.height);
        float tabSize = tabLayoutEditor.transform.parent.GetComponent<RectTransform>().rect.height / Mathf.Min(Screen.width, Screen.height);
        for (int i = 0; i < areaLayoutEditor.sizes.Count; ++i) {
            areaLayoutEditor.sizes[i] = areaSize;
        }
        for (int i = 0; i < tabLayoutEditor.sizes.Count; ++i) {
            tabLayoutEditor.sizes[i] = tabSize;
        }
    }
    IEnumerator SetupLayoutsEnum() {
        yield return new WaitForEndOfFrame();
        SetupLayouts();
    }

    // Setup settings tabs
    void SetupTabs() {
        // Create all the tab objects
        if (settingTabsHolder.childCount != settingAreas.Count) {
            GetTabs();
        }
    }

    void GetTabs() {
        // Destroy extras
        for (int i = settingTabsHolder.childCount; i > settingAreas.Count; ++i) {
            if (settingTabsHolder.childCount == settingAreas.Count) { break; }
            if (Application.isPlaying) { GameObject.Destroy(settingTabsHolder.GetChild(i-1).gameObject); }
            else { GameObject.DestroyImmediate(settingTabsHolder.GetChild(i-1).gameObject); }
        }
        // Create missing ones
        for (int i = settingTabsHolder.childCount; i < settingAreas.Count; ++i) {
#if UNITY_EDITOR
            if (Application.isPlaying) {
                GameObject obj = Instantiate(settingTabPrefab, settingTabsHolder);
            } else {
                PrefabUtility.InstantiatePrefab(settingTabPrefab, settingTabsHolder);
            }
#else
            GameObject obj = Instantiate(settingTabPrefab, settingTabsHolder);
#endif
        }
        // Assign them to their respective setting areas
        for (int i = 0; i < settingAreas.Count; ++i) {
            settingAreas[i].tab = settingTabsHolder.GetChild(i);
        }
    }

    // Setup settings areas
    void SetupSettingsAreas() {
        // Create all the area objects
        if (settingAreasHolder.childCount != settingAreas.Count) {
            GetAreas();
        }
    }

    void GetAreas() {
        // Destroy extras
        for (int i = settingAreasHolder.childCount; i > settingAreas.Count; ++i) {
            if (settingAreasHolder.childCount == settingAreas.Count) { break; }
            if (Application.isPlaying) { GameObject.Destroy(settingAreasHolder.GetChild(i-1).gameObject); }
            else { GameObject.DestroyImmediate(settingAreasHolder.GetChild(i-1).gameObject); }
        }
        // Create missing ones
        for (int i = settingAreasHolder.childCount; i < settingAreas.Count; ++i) {
#if UNITY_EDITOR
            if (Application.isPlaying) {
                GameObject obj = Instantiate(settingAreaPrefab, settingAreasHolder);
            } else {
                PrefabUtility.InstantiatePrefab(settingAreaPrefab, settingAreasHolder);
            }
#else
            GameObject obj = Instantiate(settingAreaPrefab, settingAreasHolder);
#endif
        }
        // Assign them to their respective setting areas
        for (int i = 0; i < settingAreas.Count; ++i) {
            settingAreas[i].settingsHolder = settingAreasHolder.GetChild(i).Find("Padding").Find("Settings Holder");
        }
    }


    public void Save() {
        // Save it (json stuff)
        List<List<SettingObject>> settings = GetAllSettings();
        AllAreaValues jsonValues = new AllAreaValues(settings);
        string toJson = JsonUtility.ToJson(jsonValues);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/SettingsData.json", toJson);
    }

    public void Load() {
        AllAreaValues jsonValues = JsonUtility.FromJson<AllAreaValues>(Application.persistentDataPath + "/SettingsData.json");
        if (jsonValues == null) {
            Debug.LogError("No saved Settings");
            return;
        }

        // TODO: Search each setting for their respective SettingValue
        // Check if this works
        // Maybe DONT do O(n^2 search and match them directly?)
        List<SettingObject> settings = GetAllSettingsList();
        // Just a basic O(n^2) search since theres  not many objects
        foreach (SettingValue settingValue in jsonValues.ValuesToList()) {
            foreach (SettingObject setting in settings) {
                if (settingValue.SettingMatchesValue(setting)) {
                    setting.SetValue(settingValue.value);
                }
            }
        }
    }

    List<List<SettingObject>> GetAllSettings() {
        List<List<SettingObject>> settings = new List<List<SettingObject>>();
        foreach (SettingsArea area in settingAreas) {
            List<SettingObject> row = new List<SettingObject>();
            foreach (SettingObject obj in area.settingObjects) {
                row.Add(obj);   
            }
            settings.Add(row);
        }
        return settings;
    }
    List<SettingObject> GetAllSettingsList() {
        List<SettingObject> settings = new List<SettingObject>();
        foreach (SettingsArea area in settingAreas) {
            foreach (SettingObject obj in area.settingObjects) {
                settings.Add(obj);
            }
        }
        return settings;
    }

    // Classes for saving each area and shit
    [System.Serializable]
    class AllAreaValues {
        public List<AreaValues> areaValues;
        public List<SettingValue> ValuesToList() {
            List<SettingValue> allSettingValues = new List<SettingValue>();
            foreach (var area in areaValues) {
                foreach (SettingValue value in area.values) {
                    allSettingValues.Add(value);
                }
            }
            return allSettingValues;
        }
        public AllAreaValues(List<List<SettingObject>> settings) {
            areaValues = new List<AreaValues>();
            foreach (var row in settings) {
                areaValues.Add(new AreaValues(row));
            }
        }
    }
    [System.Serializable]
    class AreaValues {
        public List<SettingValue> values;
        public AreaValues(List<SettingObject> allValues) {
            values = new List<SettingValue>();
            foreach (SettingObject setting in allValues) {
                values.Add(new SettingValue(setting));
            }
        }
    }
    [System.Serializable]
    class SettingValue {
        public string value;
        public string objectName;
        public string parentName;
        public string settingName;
        // TODO: Mayve take away parent name check ??? idk if it should be necessary
        // ex. Setting gets moved to a different area
        public bool SettingMatchesValue(SettingObject settingObject) {
            if (this.value != settingObject.ValueToString()) { return false; }
            if (this.objectName != settingObject.gameObject.name) { return false; }
            if (this.parentName != settingObject.transform.parent.name) { return false; }
            if (this.settingName != settingObject.settingName) { return false; }
            return true;
        }
        public SettingValue(SettingObject settingObject) {
            this.value = settingObject.ValueToString();
            this.objectName = settingObject.gameObject.name;
            this.parentName = settingObject.transform.parent.name;
            this.settingName = settingObject.settingName;
        }
    }


    // ----- SETTINGS AREA -----
    // Holds the info of an entire settings menu
    // Want it to horizontally separate settings (ex. audio, themes, etc)
    // Then, use icons at the top of the settings area to to switch between them (or just scroll)

    [System.Serializable]
    public class SettingsArea {
        // Identifiers and looks
        [SerializeField] string name, description;
        [SerializeField] Sprite tabIcon;
        [SerializeField] ThemeColor tabColor;

        // Tab stuff
        [HideInInspector] public Transform tab;
        Button button;
        Image iconImage;
        ImageTheme iconTheme;

        // Settings objects
        Transform settingsMenu;
        [HideInInspector] public Transform settingsHolder;
        TMP_Text titleText, descriptionText;
        TextTheme titleTheme, descriptionTheme;
        RectTransform parentRect, menuRect, titleRect, paddingRect;
        [SerializeField] public List<SettingObject> settingObjects;
        int nonSettingsObjects;

        [HideInInspector] public SettingsMenu settingsMenuScript;


        public void Reset() {
            GetSettingObjects();

            // Menus
            settingsMenu.gameObject.name = name + " Menu";
            titleRect.sizeDelta = new Vector2(0f, 2.25f * settingsMenuScript.settingMenuTitleSizeMultiplier * Mathf.Min(Screen.height, Screen.width));
            titleRect.anchoredPosition = new Vector2(0f, 0f);
            paddingRect.sizeDelta = new Vector2(0f, menuRect.rect.height - titleRect.sizeDelta.y);
            paddingRect.anchoredPosition = new Vector2(0f, 0f);
            titleText.text = name;

            titleTheme.mixedColor = tabColor;

            // Setup tabs
            tab.gameObject.name = name + " Tab";
            iconImage.sprite = tabIcon;
            iconTheme.color = tabColor;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                SelectMenu();
            });
        }

        // TODO: THIS
        void SelectMenu() {
            if (!SettingObjectsAreGood()) {
                GetSettingObjects();
                if (!SettingObjectsAreGood()) { return; }
            }
            // Currently assuming the menu is at the center of the screen
            float pos = menuRect.position.x;
            float center = (float)Screen.width / 2f;
            float offset = pos - center;
            Vector3 newPos = new Vector3(parentRect.position.x - offset, parentRect.position.y, parentRect.position.z);
            parentRect.position = newPos;
        }

        // Get settingObjects for modifying all together on the SettingsMenu
        void GetSettingObjects() {
            if (settingsHolder == null) {
                Debug.Log("SettingsMenu: No settingsHolder set");
                return;
            }
            if (SettingObjectsAreGood()) { return; }

            // Get menu stuff
            settingsMenu = settingsHolder.parent.parent;
            parentRect = settingsMenu.parent.GetComponent<RectTransform>();
            menuRect = settingsMenu.GetComponent<RectTransform>();
            titleText = settingsMenu.Find("Title Text").GetComponent<TMP_Text>();
            titleTheme = titleText.gameObject.GetComponent<TextTheme>();
            titleRect = titleText.GetComponent<RectTransform>();
            paddingRect = settingsMenu.Find("Padding").GetComponent<RectTransform>();
            //descriptionText = ???

            // Get icon stuff
            iconImage = tab.Find("Icon").GetComponent<Image>();
            iconTheme = tab.GetComponent<ImageTheme>();
            button = tab.GetComponent<Button>();

            // Get each settings object
            nonSettingsObjects = 0;
            settingObjects = new List<SettingObject>();
            foreach (Transform child in settingsHolder) {
                SettingObject obj = child.GetComponent<SettingObject>();
                if (obj == null) {
                    ++nonSettingsObjects;
                } else {
                    settingObjects.Add(obj);
                }
            }
        }
        bool SettingObjectsAreGood() {
            if (settingObjects.Count != settingsHolder.childCount - nonSettingsObjects) {
                return false;
            }
            if (settingsMenu == null) { return false; }
            if (titleText == null)    { return false; }
            if (titleTheme == null)   { return false; }
            if (parentRect == null)   { return false; }
            if (menuRect == null)     { return false; }
            if (titleRect == null)    { return false; }
            if (paddingRect == null)  { return false; }
            //if (descriptionText == null) { return false; }
            //if (descriptionTheme == null) { return false; }
            if (button == null)       { return false; }
            if (iconImage == null)    { return false; }
            if (iconTheme == null)    { return false; }
            return true;
        }
    }
}
