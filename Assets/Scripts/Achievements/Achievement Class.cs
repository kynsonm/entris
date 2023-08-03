using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Achievements {

    // Enum for each type of achievement
    [System.Serializable]
    public enum AchievmentCategoryEnum {
        gamesCompleted, daysPlayed, 
    }

    // Class to hold each category of achievements
    // Ex. its enum, name, color, description, etc
    [System.Serializable]
    public class AchievementCategory
    {
        public AchievmentCategoryEnum category;
        public ThemeColor color;
    }


    // Collection of acheivements for each group
    [System.Serializable]
    public class AchievementGroup
    {
        [SerializeField] public AchievementCategory category;
        [SerializeField] public List<Achievement> achievements;
    }


// Class that represents an achievement
// Handles all the info, looks, and reward of the achievement
// Handles the achieving and collection of each too
[System.Serializable]
public class Achievement
{
    // ----- VARIABLES -----

    public long id;

    [Header("--- Info ---")]
    [SerializeField] public string name;
    [SerializeField] public string description;

    [Header("--- Progress ---")]
    [SerializeField] protected int targetNumber;
    [SerializeField] protected UnityEvent progressMethod;
    [HideInInspector] protected float progress;

    [Header("--- Icon ---")]
    [SerializeField] public Sprite iconSprite;
    [SerializeField] public ImageColor iconColor;
    [SerializeField] public string iconText;

    [Header("--- Reward ---")]
    [SerializeField] public int xp;
    [SerializeField] public int money;
    [SerializeField] public string rewardText;

    // Collection
    [HideInInspector] public bool achieved = false;
    [HideInInspector] public bool collected = false;


    // ----- METHODS -----

    public float Progress() {
        progressMethod.Invoke();
        return progress;
    }

    // Unlock/Achieve the achievement
    // Does NOT collect the rewards though
    // Returns true if unlock is successful, false otherwise
    //   ex. If already unlocked or collected, etc
    public bool Unlock() {
        // Check achieve and collection status
        if (achieved || collected) {
            achieved = true;
            return false;
        }

        // TODO: Unlocking stuff idk

        // Check progress method?

        achieved = true;
        AchievementManager.Save();
        return true;
    }

    // Collects the rewards of the achievement
    // Returns true if collection is successful, false otherwise
    //   ex. If already collected
    public bool Collect() {
        // Check collection status
        if (collected) {
            return false;
        }

        // TODO: Collecting stuff idk

        collected = true;
        AchievementManager.Save();
        return true;
    }
}

}