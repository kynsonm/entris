using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI {

    [ExecuteInEditMode]

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class ImageTheme : MonoBehaviour
    {
        // --- Public variables
        // Settings
        public bool updateColorOfImage = true;

        // Main stuff
        [Space(10f)]
        public ThemeColor color;
        [Range(0f, 1f)] public float alpha = 1f;

        // Mixed color
        [Space(10f)]
        public bool useMixedColor = false;
        public ThemeColor mixedColor;
        [Range(0f, 1f)] public float mixRatio;

        // PPU stuff
        [Space(10f)]
        public bool ignorePPUUpdate = true;
        public float PPUMultiplier = 1f;

        // --- Private variables
        Image image;
        RectTransform rectTransform;


        // --- Monobehaviour methods
        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            Reset();
        }
        void OnEnable() {
            StartCoroutine(Start());
        }
 #if UNITY_EDITOR
        private void Update() {
            if (!Application.isPlaying) {
                Reset();
            }
        }
 #endif


        // --- Methods

        // Reset
        public void Reset() {
            GetObjects();
            if (updateColorOfImage) {
                UpdateColor();
            }
            if (!ignorePPUUpdate && image.type == Image.Type.Sliced) {
                UpdatePPU();
            }
        }
        public void ResetColor() {
            GetObjects();
            if (updateColorOfImage) {
                UpdateColor();
            }
        }

        void UpdateColor() {
            Color newColor = Theme.background;
            if (!useMixedColor) {
                newColor = Theme.ColorFromType(color);
            } else {
                newColor = Theme.MixedColor(color, mixedColor, mixRatio);
            }
            newColor.a = alpha;
            image.color = newColor;
        }

        void UpdatePPU() {
            float size = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height);

            float a = 1.691267f;
            float b = 0.003931462f;
            float ppu = 1.025f * (a / (b * size));

            image.pixelsPerUnitMultiplier = ppu * PPUMultiplier;
        }

        void GetObjects() {
            if (rectTransform == null) {
                rectTransform = gameObject.GetComponent<RectTransform>();
            }
            if (image == null) {
                image = gameObject.GetComponent<Image>();
                if (image == null) {
                    Debug.LogWarning("No Image component on " + gameObject.name + ". Adding one!");
                    image = gameObject.AddComponent<Image>();
                }
            }
        }
    }
}