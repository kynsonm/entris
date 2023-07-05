using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UnityEngine.UI {

    [ExecuteInEditMode]

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(TMP_Text))]
    public class TextTheme : MonoBehaviour
    {
        // --- Public variables
        // Settings
        public bool updateFont = true;
        public bool updateTextSize = true;
        public bool updateColor = true;

        // Main stuff
        [Space(10f)]
        public TextColor color;
        [Range(0f, 1f)] public float alpha = 1f;
        public FontType font;
        [Range(0f, 1f)]  public float maxFontSizePercentage = 1f;

        // Mixed color
        [Space(10f)]
        public bool useMixedColor = false;
        public ThemeColor mixedColor;
        [Range(0f, 1f)] public float mixRatio;

        // Others
        [SerializeField] bool alwaysOverrideChecks = false;

        // --- Private variables
        TMP_Text text;
        RectTransform rectTransform;
        string lastText;
        Vector2 lastRectSize = new Vector2();
        float lastMaxTextSize = 0f;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            Reset();
        }
        void OnEnable() {
            StartCoroutine(Start());
        }

        // Reset
        public void Reset() {
            Reset(alwaysOverrideChecks);
        }
        public void Reset(bool overrideChecks) {
            GetComponents();
            if (updateColor) {
                UpdateColor();
            }
            if (updateFont) {
                UpdateFont();
            }
            if (updateTextSize) {
                ResetTextSize(overrideChecks);
            }
            text.ForceMeshUpdate();
        }

        // Updates text color to selected image color type (main, accent, background)
        public void UpdateColor() {
            Color newColor = Theme.background;
            if (!useMixedColor) {
                newColor = Theme.ColorFromType(color);
            } else {
                newColor = Theme.MixedColor(color, mixedColor, mixRatio);
            }

            newColor.a = alpha;
            text.color = newColor;
        }

        // Update font type (main, bold, thin)
        public void UpdateFont() {
            if (!updateFont) { return; }

            switch (font) {
                case FontType.main: text.font = Theme.font_main;
                    break;
                case FontType.bold: text.font = Theme.font_bold;
                    break;
                case FontType.thin: text.font = Theme.font_thin;
                    break;
            }
        }

        // Update text size depending on MaxTextRatio
        bool coroutineRunning = false;
        public void ResetTextSize() { ResetTextSize(false); }
        public void ResetTextSize(bool overrideSizeAndTextChecks) {
            text.enableAutoSizing = true;
            if (coroutineRunning || !gameObject.activeInHierarchy) { return; }

            if (overrideSizeAndTextChecks) { }
            else if (lastRectSize.x == rectTransform.rect.width && lastRectSize.y == rectTransform.rect.height && lastText == text.text) {
                text.fontSizeMax = maxFontSizePercentage * lastMaxTextSize;
                text.ForceMeshUpdate(true, false);
                return;
            }
            lastRectSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            lastText = text.text;

            StopAllCoroutines();
            StartCoroutine(UpdateTextSizeEnumerator());
        }
        IEnumerator UpdateTextSizeEnumerator() {
            coroutineRunning = true;

            text.fontSizeMax = 10000f;
            yield return new WaitForEndOfFrame();
            
            lastMaxTextSize = text.fontSize;
            text.fontSizeMax = maxFontSizePercentage * text.fontSize;
            text.ForceMeshUpdate(true, false);
            coroutineRunning = false;
        }

        void GetComponents() {
            if (rectTransform == null) {
                rectTransform = gameObject.GetComponent<RectTransform>();
            }
            if (text == null) {
                text = gameObject.GetComponent<TMP_Text>();
                if (text == null) {
                    Debug.LogWarning("No TMP_Text component on " + gameObject.name + ". Adding one!");
                    text = gameObject.AddComponent<TMP_Text>();
                }
            }
        }

        private void Update() {
    #if UNITY_EDITOR
            if (!Application.isPlaying /* && stateHasChanged() */) {
                Reset();
            }
    #endif
        }
    }
}