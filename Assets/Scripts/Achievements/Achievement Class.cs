using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Achievements {


// Collection of acheivements for each group
[System.Serializable]
public class AchievementGroup
{
    [SerializeField] AchievementCategory category;
    [SerializeField] List<Achievement> achievements;
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
    [SerializeField] protected string name;
    [SerializeField] protected string description;

    [Header("--- Progress ---")]
    [SerializeField] protected int targetNumber;
    [SerializeField] protected UnityEvent progressMethod;

    [Header("--- Icon ---")]
    [SerializeField] protected Sprite iconSprite;
    [SerializeField] protected ImageColor iconColor;
    [SerializeField] protected string iconText;

    [Header("--- Reward ---")]
    [SerializeField] protected int xp;
    [SerializeField] protected int money;
    [SerializeField] protected string rewardText;

    // Collection
    public bool achieved = false;
    public bool collected = false;


    // ----- METHODS -----

    // Check the progress of the achievement
    public void CheckProgress() {
        progressMethod.Invoke();
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
        return true;
    }
}

}