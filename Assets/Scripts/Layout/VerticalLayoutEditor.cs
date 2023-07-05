using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI {

[ExecuteInEditMode]

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(VerticalLayoutGroup))]
public class VerticalLayoutEditor : MonoBehaviour
{
    public bool controlHeight = true;
    bool lastControlHeight = true;
    public bool expandHeight = true;
    [SerializeField] bool expandParentSizeToChildrenSize = false;

    [SerializeField] public List<GameObject> objects = new List<GameObject>();
    List<LayoutElement> layoutGroups = new List<LayoutElement>();
    public List<float> sizes = new List<float>();
    public Vector2 vertPaddingMultiplier = new Vector2(0f, 0f);
    public Vector2 horPaddingMultiplier = new Vector2(0f, 0f);
    public float spacingDivider = 0f;

    [Space(10f)]
    [SerializeField] bool dontResetCustomSizesList = false;

    RectTransform rectTransform;
    VerticalLayoutGroup vertical;
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
        vertical.childControlWidth = true;
        vertical.childForceExpandWidth = true;
        vertical.childControlHeight = controlHeight;
        vertical.childForceExpandHeight = expandHeight;

        if (objects.Count != layoutGroups.Count) {
            AddAllElements();
        }
        if (lastControlHeight != controlHeight) {
            AddAllElements();
            lastControlHeight = controlHeight;
        }

        RectTransform parentRect;
        if (vertical.transform.parent == null) { parentRect = rectTransform; }
        else { parentRect = vertical.transform.parent.GetComponent<RectTransform>(); }
        float parentSize = Mathf.Min(parentRect.rect.width, parentRect.rect.height);

        float totalSize = 0f;
        for (int i = 0; i < objects.Count; ++i) {
            if (objects[i] == null || layoutGroups[i] == null) { continue; }
            if (controlHeight) {
                layoutGroups[i].preferredHeight = sizes[i];
                totalSize += sizes[i];
            } else {
                //layoutGroups[i].preferredHeight = 0f;
                float size = sizes[i] * parentSize;
                RectTransform childRect = objects[i].GetComponent<RectTransform>();
                childRect.sizeDelta = new Vector2(childRect.sizeDelta.x, size);
                totalSize += size;
            }
        }

        totalSize += vertical.padding.top + vertical.padding.bottom;
        totalSize += (objects.Count - ignoreCount - 1) * vertical.spacing;
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        if (!vertical.childControlHeight) {
            float viewSize = parentRect.rect.height;

            if (viewSize >= totalSize) {
                totalSize = viewSize;
            }
            totalSize -= viewSize * (rectTransform.anchorMax.y - rectTransform.anchorMin.y);

            if (totalSize > 100000f || totalSize < 100000f) { totalSize = 0f; }

            if (expandParentSizeToChildrenSize && rect.sizeDelta.y != totalSize - viewSize && lastTotalSize != totalSize) {
                Debug.Log("Setting holder rect size to totalSize of " + (totalSize-viewSize) + " from rect.sizeDelta.y of " + rect.sizeDelta.y);
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, totalSize);
            }
        }
        lastTotalSize = totalSize;
    }

    // Updates side padding size, depending on screen width
    void UpdatePadding() {
        int pad = (int)(rectTransform.rect.width * horPaddingMultiplier.x);
        vertical.padding.left = pad;
        pad = (int)(rectTransform.rect.width * horPaddingMultiplier.y);
        vertical.padding.right = pad;
        
        Vector2Int horPad = new Vector2Int();
        horPad.x = (int)(rectTransform.rect.height * vertPaddingMultiplier.x);
        horPad.y = (int)(rectTransform.rect.height * vertPaddingMultiplier.y);
        vertical.padding.top = horPad.x;
        vertical.padding.bottom = horPad.y;

        vertical.spacing = (spacingDivider < 1f) ? 0 : (rectTransform.rect.height / spacingDivider);
        spacingSize = vertical.spacing;

        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponentInChildren<RectTransform>());
    }

    // Adds all children of HorizontalLayoutHolder to Objects
    public void AddAllElements() {
        // Resets Objects and Sizes
        objects = new List<GameObject>();
        layoutGroups = new List<LayoutElement>();
        ignoreCount = 0;

        RectTransform parentRect;
        if (vertical.transform.parent == null) { parentRect = rectTransform; }
        else { parentRect = vertical.transform.parent.GetComponent<RectTransform>(); }
        float parentSize = Mathf.Min(Screen.height, Screen.width);
        if (parentSize <= 0.01f) { parentSize = 9999999999f; }

        // For all children
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
            if (vertical.childControlHeight) {
                sizes[i] = layout.preferredHeight;
            }
            else if (!dontResetCustomSizesList) {
                float size = trans.GetComponent<RectTransform>().rect.height / parentSize;
                sizes[i] = size;
            }
        }
    }

    void GetObjects() {
        if (rectTransform == null) {
            rectTransform = gameObject.GetComponent<RectTransform>();
        }
        if (vertical == null) {
            vertical = gameObject.GetComponent<VerticalLayoutGroup>();
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