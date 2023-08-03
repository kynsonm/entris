using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayerInfo.Achievements;

public class AchievementManager : MonoBehaviour
{
    // All achievements!
    [SerializeField] List<AchievementGroup> achievementGroups;


    // Start is called before the first frame update
    void Awake() { Start(); }
    void OnEnable() { Start(); }
    void Start() {
        StopAllCoroutines();
        StartCoroutine(SaveAchievementsEnum());
    }

    IEnumerator SaveAchievementsEnum() {
        while (true) {
            Save();
            yield return new WaitForSeconds(5f);
        }
    }


    // Yeah idk, pretty self explanatory
    public static AchievementManager achievementManager;
    static bool ManagerIsGood() {
        if (achievementManager == null) {
            achievementManager = GameObject.FindObjectOfType<AchievementManager>();
            if (achievementManager == null) {
                return false;
            }
        }
        return true;
    }

    // Get all the achievements
    public static List<AchievementGroup> achievements() {
        if (!ManagerIsGood()) { return null; }
        return achievementManager.achievementGroups;
    }

    // Save all the achievements to a json
    // Static so it can be called from anywhere (just in case)
    public static void Save() {
        if (!ManagerIsGood()) { return; }

        // Do JSON stuff here

        // idk
    }
}
