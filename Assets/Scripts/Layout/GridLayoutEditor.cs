using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI {

[ExecuteInEditMode]

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutEditor : MonoBehaviour
{
    public enum ConstraintType {
        columnCount, rowCount, none = -1
    }
    public static GridLayoutGroup.Constraint ConstraintToGridLayoutConstraint(ConstraintType constraintType) {
        if      (constraintType == ConstraintType.columnCount) { return GridLayoutGroup.Constraint.FixedColumnCount; }
        else if (constraintType == ConstraintType.rowCount)    { return GridLayoutGroup.Constraint.FixedRowCount;    }
        return GridLayoutGroup.Constraint.Flexible;
    }

    [Header("Constraints")]
    public ConstraintType constraintType = ConstraintType.none;
    public  int constraintCount = 0;

    [Header("Layout Options")]
    [SerializeField] bool expandChildrenToWidth = true;
    [SerializeField] bool expandChildrenToHeight = true;
    bool lastExpandToWidth, lastExpandToHeight;
    [SerializeField] bool expandParentSizeToChildrenSize = false;
    public bool dontUpdateConstraintCount = true;

    [Range(0.1f, 1f)] public float cellSizeXMultiplier = 1f;
    [Range(0.1f, 1f)] public float cellSizeYMultiplier = 1f;

    [Space(5f)]
    [Header("Padding and Spacing")]
    [SerializeField] Vector2 vertPaddingMultiplier = new Vector2(0f, 0f);
    [SerializeField] Vector2 horPaddingMultiplier = new Vector2(0f, 0f);
    [SerializeField] Vector2 spacingDividers = new Vector2(0f, 0f);

    [HideInInspector] public float size { get; private set; }

    // --- Private variables
    List<GameObject> objects = new List<GameObject>();
    RectTransform rectTransform;
    GridLayoutGroup grid;
    int ignoreCount = 0;

    // Start is called before the first frame update
    private void OnEnable() { StartCoroutine(Start()); }
    private IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        Reset();
    }
    public void Reset() {
        if (!gameObject.activeInHierarchy) { return; }
        GetObjects();
        if (transform.childCount != objects.Count + ignoreCount) {
            AddAllElements();
        }
        UpdatePadding();
        CheckAndUpdateSizes();
    }

    void CheckAndUpdateSizes() {
        if (expandChildrenToWidth != lastExpandToWidth || expandChildrenToHeight != lastExpandToHeight) {
            AddAllElements();
            lastExpandToWidth = expandChildrenToWidth;
            lastExpandToHeight = expandChildrenToHeight;
        }
        if (dontUpdateConstraintCount) {
            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            grid.constraintCount = 0;
        } else {
            grid.constraint = ConstraintToGridLayoutConstraint(constraintType);
            grid.constraintCount = constraintCount;
        }

        float numElements = objects.Count;
        float horSizeAvailable = rectTransform.rect.width - grid.padding.left - grid.padding.right;
        float vertSizeAvailable = rectTransform.rect.height - grid.padding.top - grid.padding.bottom;

        Vector2 cellSize = new Vector2(horSizeAvailable, vertSizeAvailable);
        if (constraintType == ConstraintType.columnCount && constraintCount > 0) {
            cellSize.x -= (float)(constraintCount - 1f) * grid.spacing.x;
            cellSize.x /= (float)constraintCount;
            cellSize.y = cellSize.x;

            if (expandParentSizeToChildrenSize) {
                RectTransform parentRect = rectTransform.parent.GetComponent<RectTransform>();
                float rows = Mathf.Ceil(numElements/constraintCount);
                float height = grid.padding.top + grid.padding.bottom + (rows-1) * grid.spacing.y + rows * (cellSizeYMultiplier * cellSize.y);
                size = height;
                height -= parentRect.rect.height * (rectTransform.anchorMax.y - rectTransform.anchorMin.y);
                if (height < parentRect.rect.height) {
                    height = parentRect.rect.height * (1f - (rectTransform.anchorMax.y - rectTransform.anchorMin.y));
                }
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
            }
        }
        else if (constraintType == ConstraintType.rowCount && constraintCount > 0) {
            cellSize.y -= (float)(constraintCount - 1f) * grid.spacing.y;
            cellSize.y /= (float)constraintCount;
            cellSize.x = cellSize.y;

            if (expandParentSizeToChildrenSize) {
                RectTransform parentRect = rectTransform.parent.GetComponent<RectTransform>();
                float cols = Mathf.Ceil(numElements/constraintCount);
                float width = grid.padding.left + grid.padding.right + (cols-1) * grid.spacing.x + cols * (cellSizeXMultiplier * cellSize.x);
                size = width;
                width -= parentRect.rect.width * (rectTransform.anchorMax.x - rectTransform.anchorMin.x);
                if (width < parentRect.rect.width) {
                    width = parentRect.rect.width * (1f - (rectTransform.anchorMax.x - rectTransform.anchorMin.x));
                }
                rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
            }
        }
        else {
            float max = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height);
            cellSize = new Vector2(max, max);
        }

        if (cellSize.x > Screen.width || cellSize.y > Screen.height) {
            Debug.Log("Cell size of " + cellSize.ToString() + " is greater than screen size of (" + Screen.width + ", " + Screen.height + ")");
            return;
        }

        grid.cellSize = new Vector2(cellSizeXMultiplier * cellSize.x, cellSizeYMultiplier * cellSize.y);
    }

    // Updates side padding size, depending on screen width
    void UpdatePadding() {
        int pad = (int)(rectTransform.rect.width * horPaddingMultiplier.x);
        grid.padding.left = pad;
        pad = (int)(rectTransform.rect.width * horPaddingMultiplier.y);
        grid.padding.right = pad;
        
        Vector2Int horPad = new Vector2Int();
        horPad.x = (int)(rectTransform.rect.height * vertPaddingMultiplier.x);
        horPad.y = (int)(rectTransform.rect.height * vertPaddingMultiplier.y);
        grid.padding.top = horPad.x;
        grid.padding.bottom = horPad.y;

        Vector2 spacing = new Vector2(0f, 0f);
        float size = (float)Mathf.Max(rectTransform.rect.width, rectTransform.rect.height);
        spacing.x = (spacingDividers.x < 1f) ? 0f : (size / spacingDividers.x);
        spacing.y = (spacingDividers.y < 1f) ? 0f : (size / spacingDividers.y);
        grid.spacing = spacing;

        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponentInChildren<RectTransform>());
    }

    // Adds all children of HorizontalLayoutHolder to Objects
    void AddAllElements() {
        // Resets Objects and Sizes
        objects = new List<GameObject>();
        ignoreCount = 0;

        // For all children
        foreach (Transform child in transform) {
            LayoutElement layout = child.gameObject.GetComponent<LayoutElement>();
            
            // If objects does not have a layout element, give it one!
            if (layout == null) {
                layout = child.gameObject.AddComponent<LayoutElement>();
                layout.ignoreLayout = false;
            }

            // If ignoreLayout is off, add this object to Objects and its size to Sizes
            if (!layout.ignoreLayout) {
                //if (objects == null) { Debug.Log("GridLayoutEditor: No objects, its null"); }
                //if (child == null) { Debug.Log("GridLayoutEditor: No child in transform " + transform.name); }
                if (child == null) { continue; }
                objects.Add(child.gameObject);
            } else {
                ++ignoreCount;
            }
        }
    }

    void GetObjects() {
        if (rectTransform == null) {
            rectTransform = gameObject.GetComponent<RectTransform>();
        }
        if (grid == null) {
            grid = gameObject.GetComponent<GridLayoutGroup>();
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