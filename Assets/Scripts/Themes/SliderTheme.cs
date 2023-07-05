using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]

[RequireComponent(typeof(Slider))]
public class SliderTheme : MonoBehaviour
{
    [Header("Colors")]
    public ThemeColor backgroundColor;
    public ThemeColor fillColor, knobColor;

    [Header("Sizing")]
    [SerializeField] [Range(0.05f, 1f)] float heightMultiplier = 1f;
    [SerializeField] [Range(0.05f, 1f)] float widthMultiplier = 1f;
    [SerializeField] [Range(0.05f, 1f)] float handleAreaWidthMultiplier = 1f;
    [SerializeField] [Min(0f)] Vector2 handleSizeMultiplier = new Vector2(1f, 1f);

    Slider slider;
    RectTransform sliderRect;
    RectTransform backgroundRect, fillArea, fillRect, handleSlideArea, handleRect;


    // Start is called before the first frame update
    void OnEnable() {
        StopAllCoroutines();
        StartCoroutine(Start());
    }
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Reset();  
    }

    public void Reset() {
        if (!GetObjects()) { return; }
        ResetColor();

        // Anchors and pivots
        backgroundRect.anchorMin = new Vector2(0f, 0f);
        backgroundRect.anchorMax = new Vector2(1f, 1f);
        fillArea.anchorMin = new Vector2(0f, 0f);
        fillArea.anchorMax = new Vector2(1f, 1f);
        handleSlideArea.anchorMin = new Vector2(0f, 0f);
        handleSlideArea.anchorMax = new Vector2(1f, 1f);

        float width = sliderRect.rect.width, height = sliderRect.rect.height;

        // Width of each area
        float mult = AdjustedMultiplier(widthMultiplier);
        RectTransformOffset.Sides(backgroundRect, mult * width);
        RectTransformOffset.Sides(fillArea, mult * width);
        RectTransformOffset.Sides(handleSlideArea, AdjustedMultiplier(widthMultiplier * handleAreaWidthMultiplier) * width);
        float fillAreaOffset = AdjustedMultiplier(handleAreaWidthMultiplier) * width;

        // Height of each area
        mult = AdjustedMultiplier(heightMultiplier);
        RectTransformOffset.Vertical(backgroundRect, mult * height);
        RectTransformOffset.Vertical(fillArea, mult * height);
        RectTransformOffset.Vertical(handleSlideArea, mult * height);

        // Height of handle
        float handleHeightOffset = mult * height;
        mult = AdjustedMultiplier(handleSizeMultiplier.y);
        RectTransformOffset.Vertical(handleRect, mult * height - handleHeightOffset);

        // Width of handle
        float handleWidthOffset = AdjustedMultiplier(handleAreaWidthMultiplier) * width;
        mult = AdjustedMultiplier(handleSizeMultiplier.x);
        handleRect.sizeDelta = new Vector2(mult * width - handleWidthOffset, handleRect.sizeDelta.y);
        handleRect.anchoredPosition = new Vector2(0f, handleRect.anchoredPosition.y);

        // Position of fill stuff
        fillRect.pivot = new Vector2(0f, fillRect.pivot.y);
        fillRect.anchoredPosition = new Vector2(0f, fillRect.anchoredPosition.y);
        float halfMaxValue = 0.5f * (slider.maxValue - slider.minValue);
        if (halfMaxValue == 0f) {
            Debug.LogWarning("SliderTheme: Slider's min/max value is 0f. Please don't :(");
            return;
        }
        float fillWidth = -fillAreaOffset * ((slider.value - halfMaxValue) / halfMaxValue);
        fillRect.sizeDelta = new Vector2(fillWidth, fillRect.sizeDelta.y);
    }
    float AdjustedMultiplier(float multiplier) {
        return 0.5f * (1f - multiplier);
    }

    public void ResetColor() {
        foreach (ImageTheme theme in GetThemesRecursive(transform)) {
            string name = theme.gameObject.name.ToLower();
            if (name.Contains("background")) {
                theme.color = backgroundColor;
            }
            if (name.Contains("fill")) {
                theme.color = fillColor;
            }
            if (name.Contains("handle")) {
                theme.color = knobColor;
            }
            theme.ResetColor();
        }
    }
    List<ImageTheme> GetThemesRecursive(Transform parent) {
        // Add ImageTheme on the parent, if possible
        List<ImageTheme> themes = new List<ImageTheme>();
        ImageTheme theme = parent.GetComponent<ImageTheme>();
        if (theme != null) {
            themes.Add(theme);
        }

        // Get each ImageTheme in children, return the list
        foreach (Transform child in parent) {
            themes.AddRange(GetThemesRecursive(child));
        }
        return themes;
    }


    bool GetObjects() {
        bool allGood = true;
        if (slider == null) {
            slider = gameObject.GetComponent<Slider>();
            allGood &= checkNull(slider, "Slider");
        }
        if (sliderRect == null) {
            sliderRect = slider.gameObject.GetComponent<RectTransform>();
        }
        if (backgroundRect == null) {
            backgroundRect = transform.Find("Background").GetComponent<RectTransform>();
            allGood &= checkNull(backgroundRect, "Bacground rect");
        }
        if (fillArea == null) {
            fillArea = transform.Find("Fill Area").GetComponent<RectTransform>();
            allGood &= checkNull(fillArea, "Fill Area rect");
        }
        if (fillRect == null) {
            if (fillArea != null) {
                fillRect = fillArea.Find("Fill").GetComponent<RectTransform>();
                allGood &= checkNull(fillArea, "Fill Area rect");
            } else {
                allGood = false;
            }
        }
        if (handleSlideArea == null) {
            handleSlideArea = transform.Find("Handle Slide Area").GetComponent<RectTransform>();
            allGood &= checkNull(handleSlideArea, "Handle Slide Area rect");
        }
        if (handleRect == null) {
            handleRect = transform.Find("Handle Slide Area").Find("Handle").GetComponent<RectTransform>();
            allGood &= checkNull(handleRect, "Handle rect");
        }
        return allGood;
    }
    bool checkNull(Object obj, string message) {
        if (obj == null) {
            Debug.LogError("SliderTheme: " + message + " is null");
            return false;
        }
        return true;
    }


#if UNITY_EDITOR
    // Update is called once per frame
    void Update() {
        if (!Application.isPlaying) {
            Reset();
        }
    }
#endif
}
