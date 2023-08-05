using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI {

[ExecuteInEditMode]

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(HorizontalLayoutGroup))]
public class HorizontalLayoutEditor : MonoBehaviour
{
    [System.Serializable]
    public class HorizontalLayoutObject {
        [SerializeField] GameObject gameObject;
        [HideInInspector] LayoutElement layoutGroup;
        [SerializeField] [Min(0f)] public float size;
        public Transform transform {
            get { return gameObject.transform; }
            private set { }
        }
        public bool allGood {
            get {
                if (gameObject == null) { return false; }
                if (layoutGroup == null) {
                    layoutGroup = gameObject.GetComponent<LayoutElement>();
                }
                return gameObject != null && layoutGroup != null;
            }
        }

        public void SetPreferredWidth() {
            size = (size < -1.05f) ? -1 : size;
            layoutGroup.preferredWidth = size;
        }
        public float SetRealWidth(float parentSize) {
            layoutGroup.preferredWidth = -1;
            float size = this.size * parentSize;
            size = (size < 0f) ? 0f : size;
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(size, rect.sizeDelta.y);
            return size;
        }
        public bool Equals(GameObject gameObject) {
            return this.gameObject == gameObject;
        }

        public HorizontalLayoutObject(GameObject gameObject, LayoutElement layoutGroup) {
            this.gameObject = gameObject;
            this.layoutGroup = layoutGroup;
            size = 0.1f;
        }
        public HorizontalLayoutObject(GameObject gameObject, LayoutElement layoutGroup, float size) {
            this.gameObject = gameObject;
            this.layoutGroup = layoutGroup;
            this.size = size;
        }
    }

    public bool controlWidth = true;
    bool lastControlWidth = true;
    public bool expandWidth = true;
    [SerializeField] bool expandParentSizeToChildrenSize = false;

    [Space(10f)]
    [SerializeField] public List<HorizontalLayoutObject> objects = new List<HorizontalLayoutObject>();
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
 #if UNITY_EDITOR
    private void Update() {
        if (!Application.isPlaying) {
            Reset();
        }
    }
 #endif

    // Make sure all the <objects> are good and reset them if not
    // -- Run at a constant interval INDEPENDENT of framerate
    IEnumerator CheckObjectsEnum() {
        if (!Application.isPlaying) {
            yield break;
        }
        while (true) {
            yield return new WaitForSeconds(0.333f);
            if (needsToReset()) {
                Reset();
            }
        }
    }

    public void Reset() {
        if (!gameObject.activeInHierarchy) { return; }
        GetObjects();
        if (objects.Count != transform.childCount - ignoreCount) {
            AddAllElements();
        }
        for (int i = 0; i < objects.Count; ++i) {
            if (!objects[i].allGood) {
                AddAllElements();
                break;
            }
        }
        CheckAndUpdateSizes();
        UpdatePadding();

        StopAllCoroutines();
        StartCoroutine(CheckObjectsEnum());
    }

    float lastTotalSize;
    void CheckAndUpdateSizes() {
        horizontal.childControlHeight = true;
        horizontal.childForceExpandHeight = true;
        horizontal.childControlWidth = controlWidth;
        horizontal.childForceExpandWidth = expandWidth;

        if (lastControlWidth != controlWidth) {
            AddAllElements();
            lastControlWidth = controlWidth;
        }

        RectTransform parentRect;
        if (horizontal.transform.parent == null) { parentRect = rectTransform; }
        else { parentRect = horizontal.transform.parent.GetComponent<RectTransform>(); }
        float parentSize = Mathf.Min(Screen.height, Screen.width);

        // Set each child to their respective size
        float totalSize = 0f;
        for (int i = 0; i < objects.Count; ++i) {
            var layoutObject = objects[i];
            // If no gameObject or layoutGroup, skip it
            if (!layoutObject.allGood) {
                Debug.Log("Not all good. Continuing");
                continue;
            }

            if (controlWidth) {
                layoutObject.SetPreferredWidth();
            } else {
                float newSize = layoutObject.SetRealWidth(parentSize);
                newSize = (newSize < 0f) ? 0f : newSize;
                totalSize += newSize;
            }
        }

        // Only continue if we want the object to expand with its children sizing
        if (controlWidth) {
            horizontal.childControlWidth = true;
            return;
        }
        if (!expandParentSizeToChildrenSize) { return; }

        // Set the holder's size depending on its childrens' sizes
        totalSize += horizontal.padding.left + horizontal.padding.right;
        totalSize += (objects.Count - ignoreCount - 1) * horizontal.spacing;

        float viewportSize = parentRect.rect.width;
        if (viewportSize > totalSize) {
            totalSize = viewportSize;
        }
        totalSize -= viewportSize * (rectTransform.anchorMax.x - rectTransform.anchorMin.x);
        totalSize = (totalSize > 50000f || totalSize < 0f) ? 0f : totalSize;

        rectTransform.sizeDelta = new Vector2(totalSize, rectTransform.sizeDelta.y);
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
        objects = new List<HorizontalLayoutObject>();
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
                float newSize = 0.1f;
                if (horizontal.childControlWidth) {
                    newSize = layout.preferredHeight;
                }
                else if (!dontResetCustomSizesList) {
                    newSize = child.GetComponent<RectTransform>().rect.width / parentSize;
                }
                objects.Add(new HorizontalLayoutObject(child.gameObject, layout, newSize));
            } else {
                ++ignoreCount;
            }
        }
    }

    bool needsToReset() {
        if (!GetObjects()) { return true; }
        if (objects.Count != transform.childCount - ignoreCount) {
            return true;
        }
        for (int i = 0; i < objects.Count; ++i) {
            if (!objects[i].allGood) {
                return true;
            }
        }
        return false;
    }

    bool GetObjects() {
        bool allGood = true;
        if (rectTransform == null) {
            rectTransform = gameObject.GetComponent<RectTransform>();
            allGood = false;
        }
        if (horizontal == null) {
                horizontal = gameObject.GetComponent<HorizontalLayoutGroup>();
            allGood = false;
        }
        return allGood;
    }
}}