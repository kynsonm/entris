using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[ExecuteInEditMode]

public class UserInfoArea : MonoBehaviour
{
    // ----- VARIABLES -----

    /*[HideInInspector] */public UserClass user;

    [Header("Options")]
    [SerializeField] bool showUsername = true;
    [SerializeField] bool customColors = true;
    [SerializeField] bool showLevelStuff = true;
    [SerializeField] bool showXpStuff = true;

    [Header("Important Objects")]
    [SerializeField] GameObject userArea;
    [SerializeField] GameObject levelArea;
    [SerializeField] GameObject xpArea;

    [Header("Text")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text catchphraseText;

    [Header("Pics")]
    [SerializeField] Image profilePicImage;
    [SerializeField] Image bannerImage;
    ImageTheme profilePicTheme, bannerTheme;

    [Header("Level")]
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text xpText;
    [SerializeField] Slider xpSlider;
    

    // ----- MONOBEHAVIOUR STUFF -----
    void Awake() { OnEnable(); }
    void OnEnable() {
        StopAllCoroutines();
        StartCoroutine(Start());
    }
    IEnumerator Start() {
        yield break;
    }

 #if UNITY_EDITOR
    void Update(){
        if (!Application.isPlaying) {
            Reset();
        }
    }
 #endif
    
    public void Reset() {
        ResetUserInfoObjects();
        ResetLevelObjects();
    }
    

    // ----- METHODS -----

    // Resets profile pic, banner, username, catchphrase, and colors
    void ResetUserInfoObjects() {
        bool getObjects = GetObjects();

        // Text
        CheckNullAndExecute(nameText, () => { nameText.text = FormatUsersName(); });
        CheckNullAndExecute(catchphraseText, () => { catchphraseText.text = user.catchphrase; });

        // Images
        CheckNullAndExecute(profilePicImage, () => { profilePicImage.sprite = user.profilePicture; });
        CheckNullAndExecute(bannerImage, () => { bannerImage.sprite = user.profileBanner; });
        if (getObjects && customColors) {
            profilePicTheme.color = user.profilePictureColor;
            bannerTheme.color = user.profileBannerColor;
        }
    }

    // Resets level text, slider, and slider text
    void ResetLevelObjects() {
        // Turn the area on/off
        CheckNullAndExecute(levelArea, () => { GetCanvasGroup(levelArea).alpha = showLevelStuff ? 1f : 0f; });
        if (!showLevelStuff) { return; }

        // Set level text
        CheckNullAndExecute(levelText, () => { levelText.text = user.level.ToString(); });

        // Set xp stuff
        CheckNullAndExecute(xpArea, () => { GetCanvasGroup(xpArea).alpha = showXpStuff ? 1f : 0f; });
        if (!showXpStuff) { return; }

        int xp = user.xp;
        int outOf = 100;
        CheckNullAndExecute(xpText, () => {
            xpText.text = xp.ToString() + " / " + outOf.ToString();
        });
        CheckNullAndExecute(xpSlider, () => { xpSlider.value = (float)xp / (float)outOf; });
    }

    // Check nickname, username, and options to return a formatted string
    string FormatUsersName() {
        string text = user.username;
        if (user.nickname != null && user.nickname != "") {
            text = user.nickname;
        }
        if (showUsername && user.nickname != "") {
            string code = Theme.ColorCode(Theme.MixedColor(TextColor.main, ThemeColor.text_background));
            string prefix = " " + code + "<size=50%>";
            string suffix = "</color></size>";
            text += prefix + user.username + suffix;
        }
        return text;
    }


    // ----- UTILITIES -----

    // Check of the Object is null
    // -- If it is NOT, then execute the UnityAction
    // -- Return true if the action is executed and false otherwise
    bool CheckNullAndExecute(Object obj, UnityAction executeIfNotNull) {
        return CheckNullAndExecute(obj, false, executeIfNotNull);
    }
    bool CheckNullAndExecute(Object obj, bool writeMessage, UnityAction executeIfNotNull) {
        if (obj == null || executeIfNotNull == null) {
            if (writeMessage) {
                Debug.Log($"UserInfoArea: Object is null");
            }
            return false;
        }
        executeIfNotNull.Invoke();
        return true;
    }

    // Get the canvas group off the GameObject
    // -- If there isnt one, add it
    CanvasGroup GetCanvasGroup(GameObject gameObject) {
        CanvasGroup canv = gameObject.GetComponent<CanvasGroup>();
        if (canv == null) {
            canv = gameObject.AddComponent<CanvasGroup>();
        }
        return canv;
    }

    // Get necessary objects to update the objects
    // -- Right now, it just looks for image themes
    bool GetObjects() {
        bool allGood = true;
        if (profilePicImage == null || bannerTheme == null) {
            return false;
        }
        if (profilePicTheme == null) {
            profilePicTheme = profilePicImage.GetComponent<ImageTheme>();
            if (profilePicTheme == null) {
                Debug.Log("UserInfoArea: No ImageTheme on profile picture");
                allGood = false;
            }
        }
        if (bannerTheme == null) {
            bannerTheme = bannerImage.GetComponent<ImageTheme>();
            if (bannerTheme == null) {
                Debug.Log("UserInfoArea: No ImageTheme on banner image");
                allGood = false;
            }
        }
        return allGood;
    }
}
