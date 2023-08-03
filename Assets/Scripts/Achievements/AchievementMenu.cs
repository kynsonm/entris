using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Achievements {

public class AchievementMenu : MonoBehaviour
{
    // Objects
    AchievementManager achievementManager;
    [SerializeField] ScrollRectPagesMenu scrollRectPages;

    // Prefabs
    [SerializeField] GameObject achievementPrefab;


    // Start is called before the first frame update
    void Awake() { OnEnable(); }
    void OnEnable() {
        StopAllCoroutines();
        StartCoroutine(Start());
    }
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Reset();
    }


    // Destroy and create achievement objects for each one
    public void Reset() {
        DestroyAchievements();
        CreateAchievements();
    }


    // Destroy each achievement under each parent
    public void DestroyAchievements() {
        if (scrollRectPages == null) {
            return;
        }
        foreach (var tab in scrollRectPages.tabs) {
            Transform holder = AchievementHolder(tab);
            if (holder == null) { continue; }

            foreach (Transform child in holder) {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    // Create each achievement under the parent
    public void CreateAchievements() {
        if (scrollRectPages == null) {
            return;
        }

        // Get the achievements data from the manager
        List<AchievementGroup> achievements = AchievementManager.achievements();
        if (achievements == null) {
            Debug.Log("No achievements. Returning");
            return;
        }

        var tabs = scrollRectPages.tabs;

        // Make new achievements for each
        for (int i = 0; i < achievements.Count; ++i) {
            if (i > tabs.Count - 1) { break; }
            Transform holder = AchievementHolder(tabs[i]);

            AchievementGroup group = achievements[i];
            foreach (Achievement achievement in group.achievements) {
                GameObject ach = GameObject.Instantiate(achievementPrefab, holder);

                AchievementObject achObject = ach.GetComponent<AchievementObject>();
                if (achObject == null) { continue; }

                if (achObject.iconBackgroundTheme != null) {
                    achObject.iconBackgroundTheme.color = group.category.color;
                }

                achObject.achievement = achievement;
                achObject.Reset();
            }
        }
    }

    Transform AchievementHolder(ScrollRectPagesMenu.TabClass tab) {
        return AchievementHolder(tab.paddingRect);
    }
    Transform AchievementHolder(Transform padding) {
        if (padding == null || padding.childCount == 0) { return null; }
        
        Transform holder = padding.GetChild(0);
        if (holder == null) { return null; }

        return holder;
    }
}}