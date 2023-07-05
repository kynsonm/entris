using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    // Override options
    [Header("Options")]
    [SerializeField] bool alwaysStaticBackground = false;

    // Background objects
    [Space(10f)]
    [Header("Background Objects")]
    [SerializeField] Image staticImageObject;
    [SerializeField] Transform movingBackgroundHolder;
    [SerializeField] Transform floatingObjectHolder;

    // Moving background info
    [Space(10f)]
    [Header("Moving Background")]
    [SerializeField] float backgroundImagesSizeMultiplier;
    [SerializeField] float totalBackgroundImagesSizeMultiplier;
    [SerializeField] GameObject backgroundImagePrefab;

    // The actual backgrounds
    [Space(10f)]
    [SerializeField] List<BackgroundObject> backgrounds;
    int selectedBackgroundIndex = 0;


    // Resets the background objects to the info in Background
    // Sets colors, sprites, visibility, etc
    public void Reset() {
        bool isStatic = alwaysStaticBackground || !Settings.backgroundIsActive;

        // Static background
        staticImageObject.sprite = Background.backgroundSprite;
        staticImageObject.gameObject.SetActive(isStatic);

        // Moving background
        foreach (Transform child in movingBackgroundHolder) {
            GameObject.Destroy(child.gameObject);
        }
        movingBackgroundHolder.gameObject.SetActive(!isStatic);
        SetupMovingBackground();

        // Floating objects
        foreach (Transform child in floatingObjectHolder) {
            GameObject.Destroy(child.gameObject);
        }
        floatingObjectHolder.gameObject.SetActive(!isStatic);
        SetupFloatingObjects();
    }

    // Resets, sets up, and starts moving the background images
    void SetupMovingBackground() {
        // If a moving background is not necessary
        if (!Settings.backgroundIsActive || alwaysStaticBackground) { return; }
        if (Background.backgroundSprite == null) {
            staticImageObject.sprite = Background.backgroundSprite;
            staticImageObject.gameObject.SetActive(true);
            movingBackgroundHolder.gameObject.SetActive(false);
            return;
        }

        // Create the images and set their positions, rotations, etc

    }

    // Resets, sets up, and starts creating floating objects
    void SetupFloatingObjects() {
        if (!Settings.backgroundIsActive || alwaysStaticBackground) { return; }
        if (Background.floatingSprites == null || Background.floatingSprites.Count == 0) {
            floatingObjectHolder.gameObject.SetActive(false);
            return;
        }

        // Start creating the floating objects
    }


    // Background selection methods
    public void NextBackground()     { SelectBackground(++selectedBackgroundIndex); }
    public void PreviousBackground() { SelectBackground(--selectedBackgroundIndex); }
    public void SelectBackground(int newBackgroundIndex) {
        // Make sure index is good and apply it to selected index
        if (newBackgroundIndex < 0) {
            newBackgroundIndex = backgrounds.Count - 1;
        }
        if (newBackgroundIndex >= backgrounds.Count) {
            newBackgroundIndex = 0;
        }
        selectedBackgroundIndex = newBackgroundIndex;

        // Apply it
        Background.Set(backgrounds[selectedBackgroundIndex]);
    }
}
