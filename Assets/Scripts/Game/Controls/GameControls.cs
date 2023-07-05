using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]

public class GameControls : MonoBehaviour
{
    [Header("Area Objects")]
    [SerializeField] Canvas gameControlCanvas;
    [SerializeField] List<RectTransform> interactionAreas;
    [SerializeField] RectTransform interactionAreaReference;

    [Header("Dynamic Controls")]
    [SerializeField] GameObject dynamicControlArea;
    [SerializeField] [Range(0.05f, 1f)] float dynamicMenuSizeMultiplier;
    DynamicControls dynamicControls;

    [Header("Static Controls")]
    [SerializeField] GameObject staticControlArea;


    // Start is called before the first frame update
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Reset();
    }

    public void Reset() {
        if (dynamicControls == null) {
            dynamicControls = GameObject.FindObjectOfType<DynamicControls>();
            if (dynamicControls == null) {
                Debug.LogWarning("No DynamicControls script found in the secne");
            }
        }

        SetupAreaSizes();
        SetupAreas();
    }

    // Create the menu
    public void CreateDynamicControls() {
        Debug.Log("Creating dynamic controls");
    }

    // Turn on/off areas based on settings
    // And some other stuff? idk
    void SetupAreas() {
        // Turn on/off given area
        if (Settings.dynamicControls) {
            dynamicControlArea.SetActive(true);
            staticControlArea.SetActive(false);
        } else {
            dynamicControlArea.SetActive(false);
            staticControlArea.SetActive(true);
        }
        dynamicControls.sizeMultiplier = dynamicMenuSizeMultiplier;
        dynamicControls.Reset();
    }

    // Make sure each menu is in the correct position and is the correct size
    void SetupAreaSizes() {
        if (!CheckVariables()) { return; }
        foreach (RectTransform interactionArea in interactionAreas) {
            if (interactionArea == null) { continue; }
            interactionArea.position = interactionAreaReference.position;
            interactionArea.sizeDelta = new Vector2(interactionAreaReference.rect.width, interactionAreaReference.rect.height);
        }
    }

#if UNITY_EDITOR
    void Update() {
        if (!Application.isPlaying) {
            SetupAreaSizes();
        }
    }
#endif

    bool CheckVariables() {
        if (interactionAreas == null) { return false; }
        if (interactionAreaReference == null) { return false; }
        if (gameControlCanvas == null) { return false;  }
        return true;
    }
}
