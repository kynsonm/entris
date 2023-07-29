using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]

public class ScrollRectPagesMenu : MonoBehaviour
{
    // General Options
    [SerializeField] bool showTabs;
    [SerializeField] bool showSearch;
    [Tooltip("Replaces the separate pages (stacked horizontally) with one page stacked vertically")]
    [SerializeField] bool onePage;
    bool lastShowTabs, lastShowSearch, lastOnePage;

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
        // TODO: Implement this
    }
#endif

}
