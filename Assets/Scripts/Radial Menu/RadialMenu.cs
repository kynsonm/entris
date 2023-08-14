using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

[ExecuteInEditMode]

public class RadialMenu : MonoBehaviour,
// Button stuff
IPointerClickHandler, ISubmitHandler,
// Selectable stuff
IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler, IDeselectHandler, ISelectHandler
{
    [SerializeField] bool logStates = false;

    [Header("---  Objects  ---")]
    [SerializeField] Transform optionHolder;
    [SerializeField] GameObject optionPrefab;
    RectTransform rectTransform;

    [Header("---  Options  ---")]
    [SerializeField] bool centerFirstElement;
    [SerializeField] bool flipLayout;
    [SerializeField] [Range(0, 360)] int initialDegreeOffset;
    [SerializeField] bool allowDraggingBeyondBorders;

    [Header("---  Icon Sizing  ---")]
    [SerializeField] [Range(0f, 1f)] float iconSizeMultiplier = 0.5f;
    [SerializeField] [Range(0f, 1f)] float iconDistanceMultiplier = 0.5f;
    [SerializeField] [Range(0f, 1f)] float spacingMultiplier = 0f;
    float radius;

    [Header("---  Radial Buttons  ---")]
    [SerializeField] List<RadialButton> buttons;


    [HideInInspector] public bool buttonIsSelected {
        get {
            return buttonSelected != null;
        }
        private set { }
    }
    [HideInInspector] public Button selectedButton {
        get {
            if (buttonSelected == null) { return null; }
            return buttonSelected.button;
        }
        private set { }
    }


    // Start is called before the first frame update
    void OnEnable() { Awake(); }
    void Awake() {
        StopAllCoroutines();
        StartCoroutine(Start());
    }
    IEnumerator Start() {
        lastNumberOfButtons = buttons.Count;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Reset();
    }
    public void Reset() {
        if (rectTransform == null) {
            rectTransform = gameObject.GetComponent<RectTransform>();
        }
        CheckForButtonsChange();
        ResetTransforms();
    }


    public void DEBUG_Say(string message) {
        Debug.Log(message);
    }


    // ----- RADIAL BUTTON SLECTION -----

    RadialButton buttonSelected = null;
    RadialButton buttonDown = null;

    // Button function overloading
    public void OnPointerClick(PointerEventData eventData) {
        log("RadialMenu: OnPointerClick() called");

        if (buttonSelected == null) { return; }
        buttonSelected.button.SendMessage( "OnPointerClick", eventData );

        resetVars();
    }
    public void OnSubmit(BaseEventData eventData) {
        log("RadialMenu: OnSubmit() called");

        if (buttonSelected == null) { return; }
        buttonSelected.button.SendMessage( "OnSubmit", eventData );

        resetVars();
    }

    // Selectable function overloading
    public void OnPointerEnter(PointerEventData eventData) {
        log("RadialMenu: OnPointerEnter() called");
        
        resetVars();
        StopAllCoroutines();
        StartCoroutine(CheckAndSelectOption(eventData));
    }
    public void OnPointerDown(PointerEventData eventData) {
        log("RadialMenu: OnPointerDown() called");

        if (buttonSelected == null) { return; }
        buttonDown = buttonSelected;
        buttonSelected.button.SendMessage( "OnPointerDown", eventData );
    }
    public void OnPointerExit(PointerEventData eventData) {
        log("RadialMenu: OnPointerExit() called");

        if (buttonSelected != null && !allowDraggingBeyondBorders) {
            buttonSelected.button.SendMessage( "OnPointerExit", eventData );
            resetVars();
        }
        if (!allowDraggingBeyondBorders) {
            resetVars();
        }
    }
    public void OnPointerUp(PointerEventData eventData) {
        log("RadialMenu: OnPointerUp() called");

        if (buttonDown == null) { return; }
        buttonDown.button.SendMessage( "OnPointerUp", eventData );

        resetVars();
    }
    public void OnDeselect(BaseEventData eventData) {
        log("RadialMenu: OnDeselect() called");

        if (buttonSelected == null) { return; }
        buttonSelected.button.SendMessage( "OnDeselect", eventData );
    }
    public void OnSelect(BaseEventData eventData) {
        log("RadialMenu: OnSelect() called");

        if (buttonSelected == null) { return; }
        buttonSelected.button.SendMessage( "OnSelect", eventData );
    }

    // Helpers
    IEnumerator CheckAndSelectOption(PointerEventData eventData) {
        float interval = (Application.targetFrameRate == -1) ? (1f / 60f) : (1f / Mathf.Max(Application.targetFrameRate, 30f));

        // Stop when the cursor leaves the gameObject
        while (true) {
            // Get the angle
            eventData.position = Input.mousePosition;
            Vector2 pointerVector = angleFromCenter(Input.mousePosition);
            float angle = Vector2.Angle(Vector2.up, pointerVector);
            if (pointerVector.x < 0) { angle = 360f - angle; }

            // Check which button the pointer is within
            // Select the one its in
            foreach (RadialButton radialButton in buttons) {
                if (angleIsInButton(angle, radialButton) && buttonSelected != radialButton) {
                    log("Selecting button " + radialButton.gameObject.name);

                    buttonSelected = radialButton;

                    if (buttonDown != null) {
                        buttonDown.button.SendMessage( "OnPointerUp", eventData );
                        buttonDown = buttonSelected;
                        buttonDown.button.SendMessage( "OnPointerDown", eventData );
                    }

                    break;
                }
            }
            yield return new WaitForSeconds(interval);
        }

        //resetVars();
    }
    void resetVars() {
        StopAllCoroutines();
        buttonSelected = null;
        buttonDown = null;
        xBounds = Vector2.negativeInfinity;
        yBounds = Vector2.negativeInfinity;
    }

    Vector2 angleFromCenter(Vector3 pointerCoordinate) {
        Vector2 pointer = new Vector2(pointerCoordinate.x, pointerCoordinate.y);
        Vector2 center = new Vector2(rectTransform.position.x, rectTransform.position.y);
        return (pointer - center).normalized;
    }
    Vector2 xBounds = Vector2.negativeInfinity, yBounds = Vector2.negativeInfinity;
    bool positionIsWithinObject(Vector2 coordinate) {
        if (rectTransform == null) {
            rectTransform = gameObject.GetComponent<RectTransform>();
        }

        if (xBounds.x < -100000f || yBounds.x == -100000f) {
            float xOffset = 0.5f * rectTransform.rect.width;
            xBounds = new Vector2(rectTransform.position.x - xOffset, rectTransform.position.x + xOffset);
            float yOffset = 0.5f * rectTransform.rect.height;
            yBounds = new Vector2(rectTransform.position.y - yOffset, rectTransform.position.y + yOffset);
        }

        bool xGood = coordinate.x > xBounds.x && coordinate.y < xBounds.y;
        bool yGood = coordinate.y > yBounds.x && coordinate.y < yBounds.y;
        return xGood && yGood;
    }

    bool angleIsInButton(float angle, RadialButton radialButton) {
        float angleLow = radialButton.angle.x, angleHigh = radialButton.angle.y;
        if (angleLow < 0f) {
            angleLow += 360f;
            angleHigh += 360f;
            if (angle < angleLow) { angle += 360f; }
        }
        return angleLow <= angle && angleHigh >= angle;
    }


    // ----- Button Looks -----

    // Set each angle, size, icon stuff, etc
    public void ResetTransforms() {
        if (buttons == null || buttons.Count == 0) { return; }
        RectTransform rect = optionHolder.GetComponent<RectTransform>();
        radius = 0.5f * Mathf.Min(rect.rect.width, rect.rect.height);

        float iconSize = iconSizeMultiplier * (2f * radius);
        float iconDistance = iconDistanceMultiplier * radius;

        float sum = 0;
        foreach (RadialButton button in buttons) {
            sum += button.relativeSize;
        }
        
        float angleOffset = -1f * (float)initialDegreeOffset;
        if (centerFirstElement) {
            angleOffset += -0.5f * (360f * ((float)buttons[0].relativeSize / sum));
        }

        if (flipLayout) { rect.eulerAngles = new Vector3(0f, 0f, 0f); }
        else { rect.eulerAngles = new Vector3(0f, 180f, 0f); }

        foreach (RadialButton button in buttons) {
            float size = (float)button.relativeSize / sum;
            float offset = size * Mathf.Min(spacingMultiplier, 0.99f);
            angleOffset += 0.5f * offset * 360f;

            float angle = (size - offset) * 360f;
            float currentAngle = angleOffset + angle;

            button.SetImageProperties(size - offset, currentAngle, iconDistance, iconSize);

            angleOffset += angle + (0.5f * offset * 360f);
        }
    }

    // See if there are any new buttons, extra buttons, or out of orders
    // Reset them if necessary
    int lastNumberOfButtons = 0;
    void CheckForButtonsChange() {
        if (buttons == null) { return; }

        // Add new buttons
        if (buttons.Count > lastNumberOfButtons) {
            buttons[buttons.Count-1] = new RadialButton(optionPrefab, optionHolder);
            buttons[buttons.Count-1].relativeSize = 1;
        }

        // Delete removed buttons
        int count = 0;
        for (int i = optionHolder.childCount; i > buttons.Count; --i) {
            GameObject toDelete = notInButtons();
            if (toDelete != null) {
                destroyObject(toDelete);
            }
            ++count;
            if (count > 10) { break; }
        }
        lastNumberOfButtons = buttons.Count;

        // Sort them if needed
        bool needsSorting = false;
        for (int i = 0; i < buttons.Count; ++i) {
            if (buttons[i].transform != optionHolder.GetChild(i)) {
                needsSorting = true;
                break;
            }
        }
        if (needsSorting) {
            for (int i = buttons.Count-1; i >= 0; --i) {
                buttons[i].transform.SetAsFirstSibling();
            }
        }

        // Reset them
        foreach (RadialButton butt in buttons) {
            butt.Reset();
        }
    }
    // Finds an object that is not within buttons
    GameObject notInButtons() {
        foreach (Transform child in optionHolder) {
            bool isInButtons = false;
            foreach (RadialButton button in buttons) {
                if (button.transform == child) {
                    isInButtons = true;
                    break;
                }
            }
            if (!isInButtons) {
                return child.gameObject;
            }
        }
        Debug.Log("Could not find any buttons missing in optionHolder");
        return null;
    }


    // Destroys an object depending on editor and isPlaying
    void destroyObject(GameObject toDelete) {
#if UNITY_EDITOR
        if (!Application.isPlaying) {
            GameObject.DestroyImmediate(toDelete);
        } else {
            GameObject.Destroy(toDelete);
        }
#else
        GameObject.Destroy(toDelete);
#endif
    }

    void log(string message) {
        if (logStates) {
            Debug.Log(message);
        }
    }


    // ----- RADIAL BUTTON CLASS -----
    // Holds icon, position, rotation, theme, etc data

    [System.Serializable]
    class RadialButton {
        // Options
        [Header("---  Options  ---")]
        [SerializeField] string name;
        [Range(1, 100)] public int relativeSize;

        [Header("---  Button Looks  ---")]
        [SerializeField] string buttonText;
        [SerializeField] Sprite buttonSprite;
        [SerializeField] ThemeColor buttonColor;

        [Header("---  Icon Looks  ---")]
        [SerializeField] Sprite iconSprite;
        [SerializeField] ThemeColor iconColor;
        [SerializeField] [Range(0f, 360f)] float iconRotation;
        [SerializeField] bool flipIcon;

        [Space(10f)]
        [SerializeField] UnityEvent onClickEvent;

        // Transform
        [HideInInspector] public GameObject gameObject;
        [HideInInspector] public Transform transform;
        RectTransform rect;
        [HideInInspector] public CanvasGroup canvasGroup { get; private set; }

        // Button stuff
        [HideInInspector] public Button button { get; private set; }
        Image buttonImage;
        ImageTheme buttonTheme;
        RectTransform buttonRect;

        // Icon and Text
        TMP_Text text;
        RectTransform textRect;
        Image icon;
        ImageTheme iconTheme;
        RectTransform iconRect;

        // Rotation
        [HideInInspector] public Vector2 angle { get; private set; }


        public void SetImageProperties(float percentage, float angle, float iconDistance, float iconSize) {
            if (!CheckObjects()) {
                Debug.Log("RadialButton: Not resetting iamge properties cuz CheckObjects is bad :(");
                return;
            }
            if (buttonImage.type != Image.Type.Filled || buttonImage.fillMethod != Image.FillMethod.Radial360) {
                Debug.Log("RadialButton: Not resetting iamge properties cuz buttonImage is not fill and/or radial :(");
                return;
            }

            // Setup button image
            buttonImage.fillOrigin = 2;
            buttonImage.fillClockwise = true;
            buttonImage.fillAmount = percentage;

            // Set angles
            if (angle == float.NaN || angle < -360f || angle > 360f) { return; }
            rect.eulerAngles = new Vector3(rect.eulerAngles.x, rect.eulerAngles.y, angle);
            iconRect.eulerAngles = new Vector3(0f, 0f, -iconRotation);
            textRect.eulerAngles = new Vector3(0f, 0f, 0f);

            // Set icon/text distance/postiion
            float radians = (0.5f * 360f * percentage) * (Mathf.PI / 180f);
            Vector2 pos = iconDistance * new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
            iconRect.anchoredPosition = pos;
            textRect.anchoredPosition = pos;

            textRect.sizeDelta = new Vector2(iconSize, iconSize) - new Vector2(rect.rect.width, rect.rect.height) * (textRect.anchorMax - textRect.anchorMin);
            iconRect.sizeDelta = new Vector2(iconSize, iconSize) - new Vector2(rect.rect.width, rect.rect.height) * (iconRect.anchorMax - iconRect.anchorMin);

            iconRect.localScale = new Vector3(flipIcon ? -1f : 1f, 1f, 1f);

            float absAngle = Mathf.Abs(angle % 360f);
            float localAngle = percentage * 360f;
            this.angle = new Vector2(absAngle - localAngle, absAngle);
        }

        public void Reset() {
            if (!CheckObjects()) {
                Debug.Log("RadialButton: Not resetting cuz CheckObjects is bad :(");
                return;
            }

            // Text and icon
            gameObject.name = name + " Button";
            text.text = buttonText;
            icon.enabled = (iconSprite == null) ? false : true;
            icon.sprite = iconSprite;
            iconTheme.color = iconColor;

            // Button stuff
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { onClickEvent.Invoke(); } );
            buttonImage.enabled = (buttonSprite == null) ? false : true;
            buttonImage.sprite = buttonSprite;
            buttonTheme.color = buttonColor;
        }

        public RadialButton(GameObject prefab, Transform parent) {
            if (prefab == null || parent == null) { return; }

 #if UNITY_EDITOR
            if (Application.isPlaying) {
                gameObject = Instantiate(prefab, parent);
            } else {
                gameObject = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            }
 #else
            gameObject = Instantiate(prefab, parent);
 #endif
            for (int i = 0; i < 5; ++i) {
                GetObjects();
            }
            GetInitialValues();
            if (!CheckObjects()) {
                Debug.LogWarning("Newly created RadialButton is not good");
                return;
            }
            Reset();
        }
        public RadialButton(RadialButton toCopy) {
            copyVars(toCopy);
        }
        void copyVars(RadialButton toCopy) {
            this.name = toCopy.name;
            // Serialized fields
            this.relativeSize = toCopy.relativeSize;
            this.buttonText = toCopy.buttonText;
            this.buttonSprite = toCopy.buttonSprite;
            this.buttonColor = toCopy.buttonColor;
            this.iconSprite = toCopy.iconSprite;
            this.iconColor = toCopy.iconColor;
            this.onClickEvent = toCopy.onClickEvent;
            // Transforms
            this.gameObject = toCopy.gameObject;
            this.transform = toCopy.transform;
            this.rect = toCopy.rect;
            this.canvasGroup = toCopy.canvasGroup;
            // Button
            this.button = toCopy.button;
            this.buttonImage = toCopy.buttonImage;
            this.buttonTheme = toCopy.buttonTheme;
            this.buttonRect = toCopy.buttonRect;
            // icon/text
            this.text = toCopy.text;
            this.textRect = toCopy.textRect;
            this.icon = toCopy.icon;
            this.iconTheme = toCopy.iconTheme;
            this.iconRect = toCopy.iconRect;
            this.iconRotation = toCopy.iconRotation;
            this.flipIcon = toCopy.flipIcon;
        }

        bool CheckObjects() {
            if (!checkEachObject()) {
                GetObjects();
                if (!checkEachObject()) {
                    return false;
                }
            }
            return true;
        }
        bool checkEachObject() {
            bool allGood = true;
            // Transform
            allGood = allGood && checkObject(gameObject, "GameObject is null");
            allGood = allGood && checkObject(transform, "Transform is null");
            allGood = allGood && checkObject(rect, "RectTransform is null");
            allGood = allGood && checkObject(canvasGroup, "CanvasGroup is null");
            // Button stuff
            allGood = allGood && checkObject(button, "Button is null");
            allGood = allGood && checkObject(buttonImage, "Button Image is null");
            allGood = allGood && checkObject(buttonTheme, "Button ImageTheme is null");
            allGood = allGood && checkObject(buttonRect, "Button RectTransform is null");
            // Icon and Text
            allGood = allGood && checkObject(text, "Text is null");
            allGood = allGood && checkObject(textRect, "Text RectTrasform is null");
            allGood = allGood && checkObject(icon, "Icon is null");
            allGood = allGood && checkObject(iconTheme, "Icon ImageTheme is null");
            allGood = allGood && checkObject(iconRect, "Icon RectTransform is null");
            return allGood;
        }
        bool checkObject(Object obj, string message) {
            if (obj == null) {
                string log = "RadialButton: " + message;
                if (gameObject != null) {
                    log += "  ||  on gameObject: " + gameObject.name;
                }
                Debug.Log(log);
                return false;
            }
            return true;
        }
        void GetObjects() {
            // Transform
            transform = gameObject.transform;
            rect = gameObject.GetComponent<RectTransform>();
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // Button stuff
            button = gameObject.GetComponent<Button>();
            if (button == null) {
                Debug.Log("Getting button from children");
                button = gameObject.GetComponentInChildren<Button>();
            }
            buttonImage = button.image;
            buttonTheme = buttonImage.GetComponent<ImageTheme>();
            buttonRect = buttonImage.GetComponent<RectTransform>();

            // Icon and Text
            text = gameObject.GetComponent<TMP_Text>();
            if (text == null) {
                text = gameObject.GetComponentInChildren<TMP_Text>();
            }
            textRect = text.GetComponent<RectTransform>();

            foreach (Transform child in transform) {
                Image img = child.GetComponent<Image>();
                if (img != null) {
                    icon = img;
                    break;
                }
            }
            if (icon == buttonImage) {
                Debug.Log("Icon == buttonImage");
                icon = null;
            }
            iconTheme = icon.GetComponent<ImageTheme>();
            iconRect = icon.GetComponent<RectTransform>();
        }
        void GetInitialValues() {
            // Text and icon
            name = text.text;
            buttonText = text.text;
            iconSprite = icon.sprite;
            iconColor = iconTheme.color;

            // Button stuff
            buttonSprite = buttonImage.sprite;
            buttonColor = buttonTheme.color;
        }
    }

#if UNITY_EDITOR
    void Update() {
        if (!Application.isPlaying) {
            Reset();
        }
    }
#endif
}
