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

    [Header("Sizing")]
    [SerializeField] [Min(0f)] float titleSizeMiltiplier = 1f;
    [SerializeField] [Min(0f)] float tabSizeMultiplier = 1f;
    [Space(5f)]
    [SerializeField] [Min(0f)] float tabSpacingMultiplier = 0f;
    [SerializeField] Vector2 tabVertPaddingMult = new Vector2(0f, 0f);
    [SerializeField] Vector2 tabHorPaddingMult = new Vector2(0f, 0f);
    [Space(5f)]
    [SerializeField] [Min(0f)] float menuSpacingMultiplier = 0f;
    [SerializeField] Vector2 menuVertPaddingMult = new Vector2(0f, 0f);
    [SerializeField] Vector2 menuHorPaddingMult = new Vector2(0f, 0f);
    [Space(5f)]
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
    ScrollRectPages scrollRectPages;
    VerticalLayoutEditor verticalLayoutEditor;
    Transform tabsTransform, searchTransform;


    // Monobehaviour methods
    void Awake() { Start(); }
    void Start() {
        Reset();
    }
    // Constantly update the layout and looks
    // But only in the editor
    // In builds, just need the awake/start methods
#if UNITY_EDITOR
    void Update() {
        if (Application.isPlaying) { return; }
        Reset();
    }
#endif


    // Fully resets the scrollrect pages menu
    // Consists of:
    // -- Making sure all tab/menu objects are made
    // -- Turn on/off things that arent wanted
    // -- Orient them horizontally or vertically
    // -- Set the sizes of each tab/menu
    public void Reset() {
        ResetTabAndMenuObjects();
        StartCoroutine(ResetTabAndMenuSizes());
    }


    // Set the sizes of everything idk
    IEnumerator ResetTabAndMenuSizes() {
        // Only do the waiting when the game is playing
        // AKA in the editor, do everything right away
        if (Application.isPlaying) {
            yield return new WaitForEndOfFrame();
        }

        float tabParentHeight = tabsHolder.parent.GetComponent<RectTransform>().rect.height;
        RectTransform tabMenusHolderParent = tabMenusHolder.parent.GetComponent<RectTransform>();
        float menuParentWidth = tabMenusHolderParent.rect.width;
        float baseTitleSize = 0.15f * menuParentWidth;

        // Set the size of the tabs and menus and titles and stuff
        foreach (TabClass tab in tabs) {
            tab.SetTabWidth(tabParentHeight, tabSizeMultiplier);
            tab.SetMenuSize(menuParentWidth, onePage);
            tab.SetTitleHeight(baseTitleSize, titleSizeMiltiplier);
        }

        // Set the size of paddings
        ResetLayoutGroups(tabParentHeight, menuParentWidth);

        if (Application.isPlaying) {
            yield return new WaitForEndOfFrame();
        }

        // Set the size of the parent
        RectTransformSizing.SetWidthToWidthOfChildren(tabsHolder);
        RectTransformSizing.SetWidthToWidthOfChildren(tabMenusHolder);

        if (Application.isPlaying) {
            yield return new WaitForEndOfFrame();
        }

        // Set the onClick functions to their respective distances
        float selectionTime = (scrollRectPages == null) ? 0.25f : scrollRectPages.selectionTime;
        foreach (TabClass tab in tabs) {
            tab.SetButtonClick(tabMenusHolder.GetComponent<RectTransform>(), tabMenusHolderParent, selectionTime);
        }
    }

    // Setup the size of paddings and each layout group
    void ResetLayoutGroups(float tabParentHeight, float menuParentWidth) {
        HorizontalLayoutGroup tabLayout = tabsHolder.GetComponent<HorizontalLayoutGroup>();
        if (tabLayout != null) {
            tabLayout.padding.top = (int)(tabParentHeight * tabVertPaddingMult.x);
            tabLayout.padding.bottom = (int)(tabParentHeight * tabVertPaddingMult.y);
            tabLayout.padding.left = (int)(tabParentHeight * tabHorPaddingMult.x);
            tabLayout.padding.right = (int)(tabParentHeight * tabHorPaddingMult.y);
            tabLayout.spacing = (int)(tabParentHeight * tabSpacingMultiplier);
        }

        HorizontalOrVerticalLayoutGroup menuLayout = tabMenusHolder.GetComponent<HorizontalOrVerticalLayoutGroup>();
        if (menuLayout != null) {
            menuLayout.padding.top = (int)(menuParentWidth * menuVertPaddingMult.x);
            menuLayout.padding.bottom = (int)(menuParentWidth * menuVertPaddingMult.y);
            menuLayout.spacing = (int)(menuSpacingMultiplier * menuParentWidth);
        }

        float leftPadding = menuParentWidth * menuHorPaddingMult.x;
        if (!onePage && tabs.Count > 0 && tabs[0].menuSizeMultiplier < 1f) {
            float newPadding = 0.5f * menuParentWidth * (1f - tabs[0].menuSizeMultiplier);
            if (leftPadding < newPadding) {
                leftPadding = newPadding;
            }
        }

        float rightPadding = menuParentWidth * menuHorPaddingMult.y;
        if (!onePage && tabs.Count > 0 && tabs[tabs.Count-1].menuSizeMultiplier < 1f) {
            float newPadding = 0.5f * menuParentWidth * (1f - tabs[tabs.Count-1].menuSizeMultiplier);
            if (rightPadding < newPadding) {
                rightPadding = newPadding;
            }
        }

        menuLayout.padding.left = (int)leftPadding;
        menuLayout.padding.right = (int)rightPadding;
    }


    // Makes sure all tabs and menu objects are created and assigned to their <TabClass>
    void ResetTabAndMenuObjects() {
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
                TabClass tab = tabs[tabs.Count-1];
                tab.tabObject = newTab;
                tab.iconImage = newTab.GetComponentInChildren<Image>();
                tab.imageTheme = newTab.GetComponent<ImageTheme>();
                tab.tabWidthMultiplier = 1f;
                tab.icon = tab.iconImage.sprite;
                tab.color = tab.imageTheme.color;
                tab.tabRect = newTab.GetComponent<RectTransform>();
            }
            else { GameObject.DestroyImmediate(tabsHolder.GetChild(tabs.Count).gameObject); }
        }
        if (tabs.Count != tabMenusHolder.childCount) {
            if (tabs.Count > tabMenusHolder.childCount) {
                GameObject newMenu = PrefabUtility.InstantiatePrefab(tabMenuPrefab, tabMenusHolder) as GameObject;
                TabClass tab = tabs[tabs.Count-1];
                tab.menuObject = newMenu;
                tab.titleRect = newMenu.transform.Find("Title Area").GetComponent<RectTransform>();
                tab.tabNameTMP = newMenu.transform.Find("Title Area").Find("Title Text").GetComponent<TMP_Text>();
                tab.tabDescriptionTMP = newMenu.transform.Find("Title Area").Find("Description Text").GetComponent<TMP_Text>();
                tab.name = tab.tabNameTMP.text;
                tab.menuSizeMultiplier = 1f;
                tab.titleHeightMultiplier = 1f;
                tab.titleTheme = tab.tabNameTMP.gameObject.GetComponent<TextTheme>();
                tab.description = tab.tabDescriptionTMP.text;
                tab.menuRect = newMenu.GetComponent<RectTransform>();
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
            HorizontalLayoutGroup horLayout = tabMenusHolder.GetComponent<HorizontalLayoutGroup>();
#if UNITY_EDITOR
            Object.DestroyImmediate(horLayout);
#else
            if (Application.isPlaying) {
                Object.Destroy(horLayout);
            } else {
                Object.DestroyImmediate(horLayout);
            }
#endif
            VerticalLayoutGroup vertLayout = tabMenusHolder.gameObject.AddComponent<VerticalLayoutGroup>();
            lastOnePage = onePage;
        }
        if (!onePage && lastOnePage) {
            VerticalLayoutGroup vertLayout = tabMenusHolder.GetComponent<VerticalLayoutGroup>();
#if UNITY_EDITOR
            Object.DestroyImmediate(vertLayout);
#else
            if (Application.isPlaying) {
                Object.Destroy(vertLayout);
            } else {
                Object.DestroyImmediate(vertLayout);
            }
#endif
            HorizontalLayoutGroup horLayout = tabMenusHolder.gameObject.AddComponent<HorizontalLayoutGroup>();
            lastOnePage = onePage;
        }
    }

    bool CheckObjects() {
        bool allGood = true;
        if (scrollRectPages == null) {
            scrollRectPages = transform.Find("Menus").GetComponent<ScrollRectPages>();
            if (scrollRectPages == null) {
                allGood = false;
            }
        }
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

        // Size options
        [Range(0f, 1f)] public float menuSizeMultiplier = 1f;
        [Min(0f)] public float tabWidthMultiplier = 1f;
        [Min(0f)] public float titleHeightMultiplier = 1f;

        // Equality checks for updating the above stuff ^^^
        string lastName, lastDescription;
        Sprite lastIcon;
        ThemeColor lastColor;

        // Objects
        [HideInInspector] public GameObject tabObject, menuObject;
        [HideInInspector] public TMP_Text tabNameTMP, tabDescriptionTMP;
        [HideInInspector] public Image iconImage;
        [HideInInspector] public ImageTheme imageTheme;
        [HideInInspector] public TextTheme titleTheme;
        [HideInInspector] public Button iconButton;

        [HideInInspector] public RectTransform tabRect, menuRect, titleRect;

        public void Reset() { Reset(false); }
        public void Reset(bool overrideEqualityChecks) {
            // Just set everything w/out checking if things are the same
            if (overrideEqualityChecks) {
                if (tabObject != null) { tabObject.name = name + " Tab"; }
                if (menuObject != null) { menuObject.name = name + " Menu"; }
                if (tabNameTMP != null) { tabNameTMP.text = name; }
                if (titleTheme != null) { titleTheme.mixedColor = color; }
                if (tabDescriptionTMP != null) { tabDescriptionTMP.text = description; }
                if (iconImage != null) { iconImage.sprite = icon; }
                if (imageTheme != null) { imageTheme.color = color; }
                return;
            }
            // Otherwise, check if things have changed, THEN set everything
            // Also set 'last' variables
            if (name != lastName && tabNameTMP != null) {
                if (tabNameTMP != null) { tabNameTMP.text = name; }
                if (tabObject != null) { tabObject.name = name + " Tab"; }
                if (menuObject != null) { menuObject.name = name + " Menu"; }
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
            if (color != lastColor) {
                if (imageTheme != null) { imageTheme.color = color; }
                if (titleTheme != null) { titleTheme.mixedColor = color; }
                lastColor = color;
            }
        }

        public void SetTabWidth(float tabAreaHeight, float tabSizeMultiplier) {
            float size = tabSizeMultiplier * tabWidthMultiplier * tabAreaHeight;
            tabRect.sizeDelta = new Vector2(size, tabRect.sizeDelta.y);
        }
        public void SetMenuSize(float menuAreaSize, bool isVertical) {
            float size = menuSizeMultiplier * menuAreaSize;
            if (!isVertical) {
                menuRect.sizeDelta = new Vector2(size, menuRect.sizeDelta.y);
                return;
            }
            RectTransformSizing.SetHeightToHeightOfChildren(menuRect.transform, false);
        }
        public void SetTitleHeight(float baseTitleSize, float titleSizeMultiplier) {
            float size = baseTitleSize * titleSizeMultiplier * titleHeightMultiplier;
            titleRect.sizeDelta = new Vector2(titleRect.sizeDelta.x, size);
        }

        public void SetButtonClick(RectTransform menuHolder, RectTransform menuHolderParent, float selectionTime) {
            if (iconButton == null) {
                iconButton = tabObject.GetComponent<Button>();
                if (iconButton == null) {
                    iconButton = tabObject.GetComponentInChildren<Button>();
                }
                if (iconButton == null) {
                    return;
                }
            }

            iconButton.onClick.RemoveAllListeners();
            iconButton.onClick.AddListener(() => {
                Vector3 menuHolderParentCenter = menuHolderParent.TransformPoint(menuHolderParent.rect.center);
                Vector3 menuCenter = menuRect.TransformPoint(menuRect.rect.center);
                Vector3 offset = menuCenter - menuHolderParentCenter;

                if (selectionTime <= 0.01f) {
                    menuHolder.position -= offset;
                    return;
                }

                Vector3 startPos = menuHolder.position;
                Vector3 endPos = menuHolder.position - offset;

                LeanTween.cancel(menuHolder.gameObject);
                LeanTween.value(menuHolder.gameObject, 0f, 1f, selectionTime)
                .setEase(LeanTweenType.easeOutCubic)
                .setOnUpdate((float value) => {
                    menuHolder.position = startPos + value * (endPos - startPos);
                })
                .setOnComplete(() => {
                    menuHolder.position = endPos;
                });
            });
        }
    }
}

}