using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Achievements;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Achievements {

public class AchievementObject : MonoBehaviour
{
    [HideInInspector] public Achievement achievement = null;

    [Header("Background")]
    [SerializeField] public Image backgroundImage;
    [Header("Icon")]
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text iconText;
    [SerializeField] public ImageTheme iconBackgroundTheme;
    [Header("Info")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [Header("Reward")]
    [SerializeField] public Image rewardBackgroundImage;
    [SerializeField] Image coinsImage;
    [SerializeField] TMP_Text coinsText;
    [SerializeField] TMP_Text achievedText;
    [Header("Progress")]
    [SerializeField] Slider progressSlider;
    [SerializeField] TMP_Text percentageText;


    // ----- Monobehaviour stuff -----
    void Awake() { StartCoroutine(Start()); }
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Reset();
    }
 #if UNITY_EDITOR
    void Update() {
        if (Application.isPlaying) { return; }
        Reset();
    }
 #endif


    public void Reset() {
        if (achievement == null) {
            Debug.Log($"No achievement on AchievementObject \"{gameObject.name}\"");
            return;
        }
        CheckNullThenExecute(iconImage, () => { iconImage.sprite = achievement.iconSprite; });
        CheckNullThenExecute(iconText,  () => { iconText.text = achievement.iconText; });

        CheckNullThenExecute(nameText, () => { nameText.text = achievement.name; });
        CheckNullThenExecute(descriptionText, () => { descriptionText.text = achievement.description; });

        CheckNullThenExecute(coinsText, () => { coinsText.text = ((int)achievement.money).ToString(); });
        CheckNullThenExecute(achievedText, () => {
            achievedText.text = achievement.rewardText;
            if (!achievement.achieved) {
                achievedText.gameObject.SetActive(false);
            } else {
                coinsText.transform.parent.gameObject.SetActive(false);
            }
        });

        float value = achievement.Progress();
        CheckNullThenExecute(progressSlider, () => {
            progressSlider.interactable = false;
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = value;
        });
        CheckNullThenExecute(percentageText, () => { percentageText.text = ((int)value).ToString(); });
    }

    bool CheckNullThenExecute(Object objectToCheck, UnityAction actionIfItExists) {
        if (objectToCheck == null) {
            Debug.Log($"AchievementObject: Object \"{objectToCheck.name}\" is null");
            return false;
        }
        actionIfItExists.Invoke();
        return true;
    }
}}