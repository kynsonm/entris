using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class DynamicControls : MonoBehaviour,
IPointerClickHandler, ISubmitHandler,
// Selectable stuff
IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler, IDeselectHandler, ISelectHandler
{
    [SerializeField] bool logStates = false;
    [Space(10f)]
    [SerializeField] RectTransform radialMenu;
    RadialMenu radialMenuScript;
    [HideInInspector] public float sizeMultiplier;
    RectTransform rectTransform;
    GameObject radialObject;

    [SerializeField] [Range(0.01f, 1f)] float tweenMultiplier = 1f;
    [SerializeField] LeanTweenType easeCurve;

    void OnEnable() { Awake(); }
    void Awake() {
        StopAllCoroutines();
        StartCoroutine(Start());
    }
    IEnumerator Start() {
        radialMenu.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        radialMenu.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        radialMenu.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Reset();
    }

    public void Reset() {
        if (!ObjectsAreGood()) { return; }
        radialMenu.gameObject.SetActive(false);
        ResizeMenu();
    }

    void ResizeMenu() {
        float size = sizeMultiplier * Mathf.Min(rectTransform.rect.width, rectTransform.rect.height);
        radialMenu.sizeDelta = new Vector2(size, size);
    }


    public void OnPointerDown(PointerEventData eventData) {
        log("DynamicControls: OnPointerDown() called");

        if (!ObjectsAreGood()) { return; }
        ResizeMenu();
        radialMenuScript.Reset();
        
        radialMenu.position = eventData.position;
        TweenMenu(true);
    }

    public void OnPointerUp(PointerEventData eventData) {
        log("DynamicControls: OnPointerUp() called");

        if (!ObjectsAreGood()) { return; }

        float pointerDistance = Vector3.Distance(eventData.position, radialMenu.position);
        bool pointerMoved = pointerDistance >= 0.05 * Mathf.Max(radialMenu.sizeDelta.x, radialMenu.sizeDelta.y);

        if (radialMenuScript.buttonIsSelected && pointerMoved) {
            radialMenuScript.selectedButton.onClick.Invoke();
        } else {
            log(!pointerMoved ? "Pointer hasn't moved" : "No button selected");
        }

        TweenMenu(false);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        log("DynamicControls: OnPointerEnter() called");
        //radialMenuScript.SendMessage( "OnPointerEnter", eventData );
    }
    public void OnPointerClick(PointerEventData eventData) {
        log("DynamicControls: OnPointerClick() called");
        //radialMenuScript.SendMessage( "OnPointerClick", eventData );
    }
    public void OnPointerExit(PointerEventData eventData) {
        log("DynamicControls: OnPointerExit() called");
        //radialMenuScript.SendMessage( "OnPointerExit", eventData );
    }
    public void OnDeselect(BaseEventData eventData) {
        log("DynamicControls: OnDeselect() called");
        radialMenuScript.SendMessage( "OnDeselect", eventData );
    }
    public void OnSelect(BaseEventData eventData) {
        log("DynamicControls: OnSelect() called");
        radialMenuScript.SendMessage( "OnSelect", eventData );
    }
    public void OnSubmit(BaseEventData eventData) {
        log("DynamicControls: OnSubmit() called");
        radialMenuScript.SendMessage( "OnSubmit", eventData );
    }

    void TweenMenu(bool isOpening) {
        radialObject.LeanCancel();

        Vector3 scaleStart = isOpening ? new Vector3(0f, 0f, 1f) : new Vector3(1f, 1f, 1f);
        Vector3 scaleEnd = isOpening ? new Vector3(1f, 1f, 1f) : new Vector3(0f, 0f, 1f);

        LeanTween.value(radialObject, 0f, 1f, tweenMultiplier * Settings.animationTime)
        .setEase(easeCurve)
        .setOnStart(() => {
            radialMenu.localScale = scaleStart;
            radialObject.SetActive(true);
        })
        .setOnUpdate((float value) => {
            radialMenu.localScale = scaleStart + value * (scaleEnd - scaleStart);
        })
        .setOnComplete(() => {
            if (!isOpening) {
                radialObject.SetActive(false);
            }
            radialMenu.localScale = new Vector3(1f, 1f, 1f);
        });
    }

    bool ObjectsAreGood() {
        bool allGood = true;
        if (radialMenu == null) {
            Debug.LogError("DynamicControls: radialMenu is null");
            return false;
        }
        if (rectTransform == null) {
            rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform == null) {
                Debug.LogError("DynamicControls: rectTransform is null");
                allGood = false;
            }
        }
        if (radialObject == null) {
            radialObject = radialMenu.gameObject;
            if (radialObject == null) {
                Debug.LogError("DynamicControls: radialObject is null");
                allGood = false;
            }
        }
        if (radialMenuScript == null) {
            radialMenuScript = radialMenu.gameObject.GetComponent<RadialMenu>();
            if (radialMenuScript == null) {
                Debug.LogError("DynamicControls: radialMenuScript is null");
                allGood = false;
            }
        }
        return allGood;
    }

    void log(string message) {
        if (logStates) {
            Debug.Log(message);
        }
    }
}
