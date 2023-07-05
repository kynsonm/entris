using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI {

[ExecuteInEditMode]

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(HorizontalLayoutGroup))]
public class HorizontalLayoutEditor : MonoBehaviour
{
    public bool controlWidth = true;
    bool lastControlWidth = true;
    public bool expandWidth = true;
    [SerializeField] bool expandParentSizeToChildrenSize = false;

    [Space(10f)]
    [SerializeField] List<GameObject> objects = new List<GameObject>();
    List<LayoutElement> layoutGroups = new List<LayoutElement>();
    public List<float> sizes = new List<float>();
    public Vector2 vertPaddingMultiplier = new Vector2(0f, 0f);
    public Vector2 horPaddingMultiplier = new Vector2(0f, 0f);
    public float spacingDivider = 0f;

    [Space(10f)]
    [SerializeField] bool dontResetCustomSizesList = false;

    RectTransform rectTransform;
    HorizontalLayoutGroup horizontal;
    int ignoreCount = 0;
    float spacingSize;

    // Start is called before the first frame update
    private void OnEnable() { StartCoroutine(Start()); }
    private IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        Reset();
    }

    public void Reset() {
        if (!gameObject.activeInHierarchy) { return; }
        GetObjects();
        if (transform.childCount != objects.Count + ignoreCount || transform.childCount != sizes.Count + ignoreCount) {
            AddAllElements();
        }
        CheckAndUpdateSizes();
        UpdatePadding();
    }

    float lastTotalSize;
    void CheckAndUpdateSizes() {
        horizontal.childControlHeight = true;
        horizontal.childForceExpandHeight = true;
        horizontal.childControlWidth = controlWidth;
        horizontal.childForceExpandWidth = expandWidth;

        if (lastControlWidth != controlWidth || objects.Count != layoutGroups.Count) {
            AddAllElements();
            lastControlWidth = controlWidth;
        }

        RectTransform parentRect;
        if (horizontal.transform.parent == null) { parentRect = rectTransform; }
        else { parentRect = horizontal.transform.parent.GetComponent<RectTransform>(); }
        float parentSize = Mathf.Min(Screen.height, Screen.width);

        float totalSize = 0f;
        for (int i = 0; i < objects.Count; ++i) {
            if (objects[i] == null || layoutGroups[i] == null) { continue; }
            if (controlWidth) {
                layoutGroups[i].preferredWidth = sizes[i];
                totalSize += sizes[i];
            } else {
                float size = sizes[i] * parentSize;
                RectTransform childRect = objects[i].GetComponent<RectTransform>();
                childRect.sizeDelta = new Vector2(size, childRect.sizeDelta.y);
                totalSize += size;
            }
        }

        totalSize += horizontal.padding.left + horizontal.padding.right;
        totalSize += (objects.Count - ignoreCount - 1) * horizontal.spacing;
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        if (!horizontal.childControlWidth) {
            float viewSize = parentRect.rect.width;

            if (viewSize >= totalSize) {
                totalSize = viewSize;
            }
            totalSize -= viewSize * (rectTransform.anchorMax.x - rectTransform.anchorMin.x);

            if (totalSize > 100000f || totalSize < -100000f) {
                Debug.LogWarning("HorizontalLayoutEditor: Sizes are too " + (totalSize > 100000f ? "LARGE" : "SMALL") + "! Setting to 0");
                totalSize = 0f;
            }
            
            if (expandParentSizeToChildrenSize) {
                rect.sizeDelta = new Vector2(totalSize, rect.sizeDelta.y);
            }
        }
        lastTotalSize = totalSize;
    }

    // Updates side padding size, depending on screen width
    void UpdatePadding() {
        int pad = (int)(rectTransform.rect.width * horPaddingMultiplier.x);
        horizontal.padding.left = pad;
        pad = (int)(rectTransform.rect.width * horPaddingMultiplier.y);
        horizontal.padding.right = pad;
        
        Vector2Int horPad = new Vector2Int();
        horPad.x = (int)(rectTransform.rect.height * vertPaddingMultiplier.x);
        horPad.y = (int)(rectTransform.rect.height * vertPaddingMultiplier.y);
        horizontal.padding.top = horPad.x;
        horizontal.padding.bottom = horPad.y;

        horizontal.spacing = (spacingDivider < 1f) ? 0 : (rectTransform.rect.width / spacingDivider);
        spacingSize = horizontal.spacing;

        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponentInChildren<RectTransform>());
    }

    // Adds all children of HorizontalLayoutHolder to Objects
    public void AddAllElements() {
        // Resets Objects and Sizes
        objects = new List<GameObject>();
        layoutGroups = new List<LayoutElement>();
        ignoreCount = 0;

        RectTransform parentRect;
        if (horizontal.transform.parent == null) { parentRect = rectTransform; }
        else { parentRect = horizontal.transform.parent.GetComponent<RectTransform>(); }
        float parentSize = Mathf.Min(Screen.height, Screen.width);
        if (parentSize <= 0.01f) { parentSize = 9999999999f; }

        // Add all relevant children to objects
        foreach (Transform child in transform) {
            LayoutElement layout = child.gameObject.GetComponent<LayoutElement>();
            // If objects does not have a layout element, give it one!
            if (layout == null) {
                child.gameObject.AddComponent<LayoutElement>();
                layout = child.gameObject.GetComponent<LayoutElement>();
                layout.ignoreLayout = false;
            }
            // If ignoreLayout is off, add this object to Objects and its size to Sizes
            if (!layout.ignoreLayout) {
                objects.Add(child.gameObject);
                layoutGroups.Add(layout);
            } else {
                ++ignoreCount;
            }
        }

        if (sizes == null) { sizes = new List<float>(); }
        // Remove extras
        for (int i = sizes.Count; i > objects.Count; ++i) {
            sizes.RemoveAt(i-1);
        }
        // Add new ones
        for (int i = 0; i < objects.Count; ++i) {
            if (i == sizes.Count) { sizes.Add(0.1f); }

            LayoutElement layout = layoutGroups[i];
            Transform trans = objects[i].transform;
            if (horizontal.childControlWidth) {
                sizes[i] = layout.preferredWidth;
            }
            else if (!dontResetCustomSizesList) {
                float size = trans.GetComponent<RectTransform>().rect.width / parentSize;
                sizes[i] = size;
            }
        }
    }

    void GetObjects() {
        if (rectTransform == null) {
            rectTransform = gameObject.GetComponent<RectTransform>();
        }
        if (horizontal == null) {
            horizontal = gameObject.GetComponent<HorizontalLayoutGroup>();
        }
    }


#if UNITY_EDITOR
    private void Update() {
        if (!Application.isPlaying) {
            Reset();
        }
    }
#endif

}}