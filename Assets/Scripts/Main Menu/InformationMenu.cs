using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationMenu : MonoBehaviour
{
    // ----- Variables ------

    [Header("Buttons")]
    [SerializeField] Button tourButton;
    [SerializeField] Button supportMeButton, contactMeButton, shopButton;

    [Header("Info")]
    [SerializeField] string supportLink;
    [SerializeField] string emailLink;


    // ----- Monobehaviour stuff -----
    void Awake() { OnEnable(); }
    void OnEnable() {
        StopAllCoroutines();
        StartCoroutine(Start());
    }
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        Reset();
    }


    // ----- Methods -----

    // Reset each button
    // -- Reset the clicks of each button in the info menu
    public void Reset() {
        if (!CheckObjects()) { return; }

        tourButton.onClick.RemoveAllListeners();
        tourButton.onClick.AddListener(() => {
            TourButtonClick();
        });

        supportMeButton.onClick.RemoveAllListeners();
        supportMeButton.onClick.AddListener(() => {
            SupportButtonClick();
        });

        contactMeButton.onClick.RemoveAllListeners();
        contactMeButton.onClick.AddListener(() => {
            ContactButtonClick();
        });

        shopButton.onClick.RemoveAllListeners();
        shopButton.onClick.AddListener(() => {
            ShopButtonClick();
        });
    }

    // Tour button click
    // -- Call start tour from a tour script (TODO)
    void TourButtonClick() {

    }

    // Support button click
    // -- Open link to my linktree lol
    void SupportButtonClick() {

        Debug.Log("Opening support link");

        Application.OpenURL(supportLink);
    }

    // Contact button click
    // -- Open the email menu
    // -- Email menu script? (TODO)
    void ContactButtonClick() {
        string t =
        "mailto:blah@blah.com?subject=Question%20on%20Awesome%20Game";
        Application.OpenURL(t);
    }

    // Shop button click
    // -- Open shop menu
    // -- Shop menu script? (TODO)
    void ShopButtonClick() {

    }


    // ----- UTILITIES -----

    bool CheckObjects() {
        bool allGood = true;
        if (tourButton      == null) { allGood = false; }
        if (supportMeButton == null) { allGood = false; }
        if (contactMeButton == null) { allGood = false; }
        if (shopButton      == null) { allGood = false; }
        return allGood;
    }
}
