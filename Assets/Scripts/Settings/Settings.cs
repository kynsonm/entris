using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    // ----- SETTINGS VARIABLES -----

    // Looks
    public static int themeIndex;
    public static int fontIndex;
    public static int blocksIndex;
    public static int backgroundIndex;
    public static bool backgroundIsActive;
    public static float animationTime;

    // Gameplay
    public static bool dynamicControls;


    // ----- METHODS -----

    // Load all saved settings
    public static void Load() {
        // Check if there are settings to load
        if (!PlayerPrefs.HasKey(nameof(animationTime))) {
            FirstRun();
        }

        // Load them
        themeIndex = getInt(nameof(themeIndex));
        fontIndex = getInt(nameof(fontIndex));
        blocksIndex = getInt(nameof(blocksIndex));
        backgroundIndex = getInt(nameof(backgroundIndex));
        animationTime = getFloat(nameof(animationTime));
        backgroundIsActive = getBool(nameof(backgroundIsActive));

        dynamicControls = getBool(nameof(dynamicControls));
    }

    // Save all settings
    public static void Save() {
        set(nameof(themeIndex), themeIndex);
        set(nameof(fontIndex), fontIndex);
        set(nameof(blocksIndex), blocksIndex);
        set(nameof(backgroundIndex), backgroundIndex);
        set(nameof(animationTime), animationTime);
        set(nameof(backgroundIsActive), backgroundIsActive);

        set(nameof(dynamicControls), dynamicControls);

        // Save each SettingsMenu
        // TODO: Is this enough? I'm not sure lol
        /*
        foreach (var menu in GameObject.FindObjectsOfType<SettingsMenu>()) {
            menu.Save();
        }
        */
    }

    // Set settings to their initial values
    public static void FirstRun() {
        themeIndex = 0;
        fontIndex = 0;
        blocksIndex = 0;
        backgroundIndex = 0;
        animationTime = 1f;
        backgroundIsActive = true;

        dynamicControls = false;

        Save();
    }


    // Set values by name
    static void set(string name, bool value) {
        PlayerPrefs.SetInt(name, value ? 1 : 0);
    }
    static void set(string name, int value) {
        PlayerPrefs.SetInt(name, value);
    }
    static void set(string name, float value) {
        PlayerPrefs.SetFloat(name, value);
    }
    static void set(string name, string value) {
        PlayerPrefs.SetString(name, value);
    }

    // Get values from name
    static bool getBool(string name) {
        return PlayerPrefs.GetInt(name) == 1;
    }
    static int getInt(string name) {
        return PlayerPrefs.GetInt(name);
    }
    static float getFloat(string name) {
        return PlayerPrefs.GetFloat(name);
    }
    static string getString(string name) {
        return PlayerPrefs.GetString(name);
    }
}
