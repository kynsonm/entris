using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UnityEngine.UI {

[ExecuteInEditMode]
public class ScrollRectPagesMenu : MonoBehaviour
{
    // General Options
    [Header("General Options")]
    [SerializeField] bool showTabs;
    [SerializeField] bool showSearch;
    [Tooltip("Replaces the separate pages (stacked horizontally) with one page stacked vertically")]
    [SerializeField] bool onePage;
    bool lastShowTabs, lastShowSearch, lastOnePage;
    [SerializeField] List<float> sizesOfEachArea;

    [Header("Prefabs")]
    [SerializeField] GameObject tabPrefab;
    [SerializeField] GameObject tabMenuPrefab;

    [Header("Tabs")]
    [SerializeField] List<TabClass> tabs;

    [Header("Objects")]
    [SerializeField] Transform tabsHolder;
    [SerializeField] Transform tabMenusHolder;


    // Objects
    VerticalLayoutEditor verticalLayoutEditor;
    Transform tabsTransform, searchTransform;


    // Start is called before the first frame update
    void Awake() { Start(); }
    void Start() {
        
    }

    // Constantly update the layout and looks
    // But only in the editor
    // In builds, just need the awake/start methods
#if UNITY_EDITOR
    void Update() {
        if (!CheckObjects()) { return; }

        // Check and set size of <sizesOfEachArea>
        if (sizesOfEachArea.Count != 3) {
            if (sizesOfEachArea.Count < 3) { sizesOfEachArea.Add(0f); }
            else { sizesOfEachArea.RemoveAt(sizesOfEachArea.Count - 1); }
        }
        for (int i = 0; i < sizesOfEachArea.Count; ++i) {
            verticalLayoutEditor.sizes[i] = sizesOfEachArea[i];
        }

        // Turn things on/off
        // Change layout to pages/one pages
        CheckOptions();

        // Check size of <tabs>
        // Create/destroy tabs/menus if necessary
        if (tabs.Count != tabsHolder.childCount) {
            if (tabs.Count > tabsHolder.childCount) {
                GameObject newTab = PrefabUtility.InstantiatePrefab(tabPrefab, tabsHolder) as GameObject;
                tabs[tabs.Count-1].iconImage = newTab.GetComponentInChildren<Image>();
                tabs[tabs.Count-1].imageTheme = newTab.GetComponent<ImageTheme>();
                tabs[tabs.Count-1].icon = tabs[tabs.Count-1].iconImage.sprite;
                tabs[tabs.Count-1].color = tabs[tabs.Count-1].imageTheme.color;
            }
            else { GameObject.DestroyImmediate(tabsHolder.GetChild(tabs.Count).gameObject); }
        }
        if (tabs.Count != tabMenusHolder.childCount) {
            if (tabs.Count > tabMenusHolder.childCount) {
                GameObject newMenu = PrefabUtility.InstantiatePrefab(tabMenuPrefab, tabMenusHolder) as GameObject;
                tabs[tabs.Count-1].tabNameTMP = newMenu.transform.Find("Title Area").Find("Title Text").GetComponent<TMP_Text>();
                tabs[tabs.Count-1].tabDescriptionTMP = newMenu.transform.Find("Title Area").Find("Description Text").GetComponent<TMP_Text>();
                tabs[tabs.Count-1].name = tabs[tabs.Count-1].tabNameTMP.text;
                tabs[tabs.Count-1].description = tabs[tabs.Count-1].tabDescriptionTMP.text;
            }
            else { GameObject.DestroyImmediate(tabMenusHolder.GetChild(tabs.Count).gameObject); }
        }

        // Reset each tab
        foreach (TabClass tab in tabs) {
            tab.Reset();
        }
    }

    // Check if tabs/search/pages are on
    // Set them to the correct values if not
    void CheckOptions() {
        // Switch tabs on/off
        if (!showTabs && lastShowTabs) {
            tabsTransform.gameObject.SetActive(false);
            lastShowTabs = false;
        }
        if (showTabs && !lastShowTabs) {
            tabsTransform.gameObject.SetActive(true);
            lastShowTabs = true;
        }

        // Switch search on/off
        if (!showSearch && lastShowSearch) {
            searchTransform.gameObject.SetActive(false);
            lastShowSearch = false;
        }
        if (showSearch && !lastShowSearch) {
            searchTransform.gameObject.SetActive(true);
            lastShowSearch = true;
        }

        // Switch layout to one page or not
        // TODO: This
        if (onePage && !lastOnePage) {

        }
        if (!onePage && lastOnePage) {

        }
    }

    bool CheckObjects() {
        bool allGood = true;
        if (verticalLayoutEditor == null) {
            verticalLayoutEditor = gameObject.GetComponent<VerticalLayoutEditor>();
            if (verticalLayoutEditor == null) {
                allGood = false;
            }
        }
        if (tabsTransform == null) {
            tabsTransform = transform.Find("Tabs");
            if (tabsTransform == null) {
                allGood = false;
            }
        }
        if (searchTransform == null) {
            searchTransform = transform.Find("Search Bar");
            if (searchTransform == null) {
                allGood = false;
            }
        }
        return allGood;
    }
#endif

    // Private class that holds info on:
    // -- Tab name, description
    // -- Tab icon, color
    // And the objects for each thing
    [System.Serializable]
    class TabClass {
        // Options
        public string name;
        public string description;
        public Sprite icon;
        public ThemeColor color;

        // Equality checks for updating the above stuff ^^^
        string lastName, lastDescription;
        Sprite lastIcon;
        ThemeColor lastColor;

        // Objects
        [HideInInspector] public TMP_Text tabNameTMP, tabDescriptionTMP;
        [HideInInspector] public Image iconImage;
        [HideInInspector] public ImageTheme imageTheme;

        public void Reset() { Reset(false); }
        public void Reset(bool overrideEqualityChecks) {
            // Just set everything w/out checking if things are the same
            if (overrideEqualityChecks) {
                if (tabNameTMP != null) { tabNameTMP.text = name; }
                if (tabDescriptionTMP != null) { tabDescriptionTMP.text = description; }
                if (iconImage != null) { iconImage.sprite = icon; }
                if (imageTheme != null) { imageTheme.color = color; }
                return;
            }
            // Otherwise, check if things have changed, THEN set everything
            // Also set 'last' variables
            if (name != lastName && tabNameTMP != null) {
                tabNameTMP.text = name;
                lastName = name;
            }
            if (description != lastDescription && tabDescriptionTMP != null) {
                tabDescriptionTMP.text = description;
                lastDescription = description;
            }
            if (icon != lastIcon && iconImage != null) {
                iconImage.sprite = icon;
                lastIcon = icon;
            }
            if (color != lastColor && imageTheme != null) {
                imageTheme.color = color;
                lastColor = color;
            }
        }
    }
}

}