using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]

[RequireComponent(typeof(RectTransform))]
public class MainMenuMenu : MonoBehaviour
{
    // Main variables
    RectTransform menuRect;

    [Header("Title Stuff")]
    [SerializeField] [Range(0f, 0.5f)] float titleSizeMultiplier = 0.1f;
    [SerializeField] string titleText;
    [SerializeField] ThemeColor titleBarColor;
    [SerializeField] ThemeColor mixColor;
    [SerializeField] [Range(0f, 1f)] float mixColorRatio;
    [Header("Other Children Stuff")]
    [SerializeField] [Range(0f, 1f)] float paddingMultiplier;
    RectTransform titleBarRect;
    TMP_Text titleTMP;
    TextTheme titleTextTheme;
    Image titleBarImage;
    ImageTheme titleBarImageTheme;

    // Start is called before the first frame update
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        Reset();
    }

    public void Reset() {
        ResetTitle();
        ResetOtherObject();
    }

    void ResetTitle() {
        if (!GetTitleObjects()) { return; }
        titleBarRect.gameObject.SetActive(titleSizeMultiplier >= 0.05f);

        titleBarRect.sizeDelta = new Vector2(0f, menuRect.rect.height * titleSizeMultiplier);
        titleBarRect.anchoredPosition = new Vector2(0f, 0f);

        float scale = (titleBarRect.rect.width == 0) ? 0f : Screen.width / titleBarRect.rect.width;
        titleBarRect.localScale = new Vector3(scale, titleBarRect.localScale.y, titleBarRect.localScale.z);

        titleTMP.text = titleText;

        titleBarImageTheme.color = titleBarColor;
        titleBarImageTheme.useMixedColor = true;
        titleBarImageTheme.mixedColor = mixColor;
        titleBarImageTheme.mixRatio = mixColorRatio;
    }

    void ResetOtherObject() {
        if (transform.childCount != 2) { return; }

        Transform child = transform.GetChild(1);
        if (child == transform.Find("Title")) {
            child = transform.GetChild(0);
        }

        RectTransform childRect = child.GetComponent<RectTransform>();
        childRect.anchorMax = new Vector2(1f, 0f);
        childRect.anchorMin = new Vector2(0f, 0f);
        childRect.pivot = new Vector2(0.5f, 0f);
        childRect.anchoredPosition = new Vector2(0f, 0f);

        float height = menuRect.rect.height * (1f - titleSizeMultiplier);
        height *= (1f - paddingMultiplier);
        childRect.sizeDelta = new Vector2(childRect.sizeDelta.x, height);
    }

    bool GetTitleObjects() {
        if (CheckTitleObjects()) { return true; }
        menuRect = gameObject.GetComponent<RectTransform>();

        Transform titleArea = gameObject.transform.Find("Title");
        if (titleArea == null) { return false; }

        titleBarRect = titleArea.GetComponent<RectTransform>();
        titleTMP = titleArea.GetComponentInChildren<TMP_Text>();
        titleTextTheme = titleTMP.gameObject.GetComponent<TextTheme>();
        titleBarImage = titleArea.GetComponentInChildren<Image>();
        titleBarImageTheme = titleBarImage.gameObject.GetComponent<ImageTheme>();

        titleSizeMultiplier = (titleSizeMultiplier < 0f) ? 0f : titleSizeMultiplier;
        titleSizeMultiplier = (titleSizeMultiplier > 0.5f) ? 0.5f : titleSizeMultiplier;

        return CheckTitleObjects();
    }
    bool CheckTitleObjects() {
        if (menuRect == null) { return false; }
        if (titleBarRect == null) { return false; }
        if (titleTMP == null) { return false; }
        if (titleTextTheme == null) { return false; }
        if (titleBarImage == null) { return false; }
        if (titleBarImageTheme == null) { return false; }
        return true;
    }

#if UNITY_EDITOR
    void Update() {
        if (!Application.isPlaying) {
            Reset();
        }
    }
#endif
}
