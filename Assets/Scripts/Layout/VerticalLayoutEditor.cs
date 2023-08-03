using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI {

[ExecuteInEditMode]

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(VerticalLayoutGroup))]
public class VerticalLayoutEditor : MonoBehaviour
{
    [System.Serializable]
    public class VerticalLayoutObject {
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

        public void SetPreferredHeight() {
            size = (size < -1.05f) ? -1 : size;
            layoutGroup.preferredHeight = size;
        }
        public float SetRealHeight(float parentSize) {
            layoutGroup.preferredHeight = -1;
            float size = this.size * parentSize;
            size = (size < 0f) ? 0f : size;
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, size);
            return size;
        }
        public bool Equals(GameObject gameObject) {
            return this.gameObject == gameObject;
        }

        public VerticalLayoutObject(GameObject gameObject, LayoutElement layoutGroup) {
            this.gameObject = gameObject;
            this.layoutGroup = layoutGroup;
            size = 0.1f;
        }
        public VerticalLayoutObject(GameObject gameObject, LayoutElement layoutGroup, float size) {
            this.gameObject = gameObject;
            this.layoutGroup = layoutGroup;
            this.size = size;
        }
    }

    public bool controlHeight = true;
    bool lastControlHeight = true;
    public bool expandHeight = true;
    [SerializeField] bool expandParentSizeToChildrenSize = false;

    [SerializeField] public List<VerticalLayoutObject> objects = new List<VerticalLayoutObject>();
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
 #if UNITY_EDITOR
    private void Update() {
        if (!Application.isPlaying) {
            Reset();
        }
    }
 #endif

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
    }

    void CheckAndUpdateSizes() {
        vertical.childControlWidth = true;
        vertical.childForceExpandWidth = true;
        vertical.childControlHeight = controlHeight;
        vertical.childForceExpandHeight = expandHeight;

        if (lastControlHeight != controlHeight) {
            AddAllElements();
            lastControlHeight = controlHeight;
        }

        RectTransform parentRect;
        if (vertical.transform.parent == null) { parentRect = rectTransform; }
        else { parentRect = vertical.transform.parent.GetComponent<RectTransform>(); }
        float parentSize = Mathf.Min(parentRect.rect.width, parentRect.rect.height);

        // Set each child to their respective size
        float totalSize = 0f;
        for (int i = 0; i < objects.Count; ++i) {
            var layoutObject = objects[i];
            // If no gameObject or layoutGroup, skip it
            if (!layoutObject.allGood) {
                Debug.Log("Not all good. Continuing");
                continue;
            }

            if (controlHeight) {
                layoutObject.SetPreferredHeight();
            } else {
                float newSize = layoutObject.SetRealHeight(parentSize);
                newSize = (newSize < 0f) ? 0f : newSize;
                totalSize += newSize;
            }
        }

        // Only continue if we want the object to expand with its children sizing
        if (controlHeight) {
            vertical.childControlHeight = true;
            return;
        }
        if (!expandParentSizeToChildrenSize) { return; }

        // Set the holder's size depending on its childrens' sizes
        totalSize += vertical.padding.top + vertical.padding.bottom;
        totalSize += (objects.Count - ignoreCount - 1) * vertical.spacing;

        float viewportSize = parentRect.rect.height;
        if (viewportSize > totalSize) {
            totalSize = viewportSize;
        }
        totalSize -= viewportSize * (rectTransform.anchorMax.y - rectTransform.anchorMin.y);
        totalSize = (totalSize > 50000f || totalSize < 0f) ? 0f : totalSize;

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, totalSize);
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
        objects = new List<VerticalLayoutObject>();
        ignoreCount = 0;

        RectTransform parentRect;
        if (vertical.transform.parent == null) { parentRect = rectTransform; }
        else { parentRect = vertical.transform.parent.GetComponent<RectTransform>(); }
        float parentSize = Mathf.Min(parentRect.rect.width, parentRect.rect.height);
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
                float newSize = 0.1f;
                if (vertical.childControlHeight) {
                    newSize = layout.preferredHeight;
                }
                else if (!dontResetCustomSizesList) {
                    newSize = child.GetComponent<RectTransform>().rect.height / parentSize;
                }
                objects.Add(new VerticalLayoutObject(child.gameObject, layout, newSize));
            } else {
                ++ignoreCount;
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
}}