using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Achievements;
using TMPro;

public class AchievementManager : MonoBehaviour
{
    // Objects
    [SerializeField] Transform scrollRectPagesHolder;

    // Prefabs
    GameObject achievementPrefab;

    // List of all achievements
    [SerializeField] List<AchievementGroup> achievementGroups;
    List<Transform> achievementPageParents;


    // Start is called before the first frame update
    void Awake() { Start(); }
    void Start() {
        Reset();
    }


    public void Reset() {
        CreateAchievements();
    }


    // Create each achievement under the parent
    public void CreateAchievements() {
        // Destroy whats alread there
        foreach (Transform parent in achievementPageParents) {
            foreach (Transform child in parent) {
                GameObject.Destroy(child.gameObject);
            }
        }

        // Create them

    }


    // Class that holds each obect of an achievement
    // Ex. name, description, icon stuff, etc
    class AchievementObject : Achievement {
        TMP_Text nameTMP, descriptionTMP, iconTMP, rewardTMP;
        ImageTheme nameTheme, descriptionTheme, iconTextTheme, rewardTheme;

        Image iconImage;
        ImageTheme iconTheme;

        public AchievementObject() {

        }
    }
}
