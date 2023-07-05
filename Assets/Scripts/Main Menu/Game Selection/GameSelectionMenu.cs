using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;

// NOTE: Put this on main menu manager object!
public class GameSelectionMenu : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject gamePrefab;
    [SerializeField] Transform content;

    [Header("Selected Game Display")]
    [SerializeField] TMP_Text selectedGameText;
    [SerializeField] string prefix, suffix;
    [SerializeField] string colorStart, colorEnd;
    
    [Header("Layout Options")]
    [SerializeField] [Range(0.1f, 1f)] float gameOptionWidthMultiplier;
    [SerializeField] [Range(0.01f, 0.5f)] float spacingMultiplier;
    GameOptions gameOptions;
    ScrollRectPages scrollRectPages;

    // Start is called before the first frame update
    void OnEnable() { StartCoroutine(Start()); }
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Reset();
    }

    // Select the game type based on <ScrollRectPages> selected page
    IEnumerator SelectGameEnum() {
        string originalColorStart = new string(colorStart);
        string colorEnd = Theme.ParseString(this.colorEnd);
        while (true) {
            yield return new WaitForSeconds(0.25f);
            if (!CheckObjects() || !content.gameObject.activeInHierarchy) { continue; }
            if (scrollRectPages.lastSelectedChild == null) { continue; }

            GameOption optionObject = scrollRectPages.lastSelectedChild.GetComponent<GameOption>();
            if (optionObject == null) { continue; }

            // Set the game
            GameOptions.GameOption game = optionObject.gameOption;
            GameInfo.Set(game);

            // Set the text
            string colorStart = Theme.ParseString(originalColorStart);
            selectedGameText.text = colorStart + prefix + colorEnd + game.name + suffix;
        }
    }

    // Create all the game options onto content
    void Reset() {
        if (!CheckObjects()) { return; }
        if (!content.gameObject.activeInHierarchy) { return; }

        // Destroy whats already in it
        foreach (Transform child in content) {
            GameObject.Destroy(child.gameObject);
        }

        // Create each object
        for (int i = 0; i < gameOptions.gameOptions.Count; ++i) {
            GameObject obj = Instantiate(gamePrefab, content);
            GameOption option = obj.GetComponent<GameOption>();
            option.gameOption = gameOptions.gameOptions[i];

            obj.name = option.gameOption.name + " Menu";
            Debug.Log("Name is " + obj.name);
        }

        // Resize the children
        RectTransform contentRect = content.GetComponent<RectTransform>();
        RectTransform parentRect = scrollRectPages.scrollRect.viewport;
        float size = gameOptionWidthMultiplier * parentRect.rect.width;
        float totalSize = 0f;
        foreach (Transform child in content) {
            RectTransform childRect = child.GetComponent<RectTransform>();
            childRect.sizeDelta = new Vector2(size, childRect.sizeDelta.y);
            totalSize += size;

            Debug.Log("Child " + child.name + " || Adding " + size + " to totalSize == " + totalSize);
        }

        // Set the spacing and padding
        int pad = (int)(0.5f * (parentRect.rect.width - size));
        HorizontalLayoutGroup hor = content.GetComponent<HorizontalLayoutGroup>();
        hor.padding.left = pad;
        hor.padding.right = pad;
        totalSize += 2f * pad;

        Debug.Log("Adding " + (2f * pad) + " to totalSize == " + totalSize);

        float spacing = spacingMultiplier * parentRect.rect.width;
        hor.spacing = spacing;
        totalSize += (float)(content.childCount - 1) * spacing;

        Debug.Log("Adding " + ((float)(content.childCount - 1) * spacing) + " to totalSize == " + totalSize);

        // Set the size of the content
        contentRect.sizeDelta = new Vector2(totalSize, contentRect.sizeDelta.y);

        Debug.Log("Content size == " + totalSize);

        StopAllCoroutines();
        StartCoroutine(SelectGameEnum());
    }

    // Returns true if everything is good to create the game options
    bool CheckObjects() {
        bool allGood = true;
        if (gamePrefab == null) {
            Debug.Log("GameSelectionMenu: No game menu prefab");
            allGood = false;
        }
        if (content == null) {
            Debug.Log("GameSelectionMenu: No scroll rect content");
            allGood = false;
        }
        if (gameOptions == null) {
            gameOptions = GameObject.FindObjectOfType<GameOptions>();
            if (gameOptions == null) {
                Debug.Log("GameSelectionMenu: Couldn't find GameManger object");
                allGood = false;
            }
        }
        if (selectedGameText == null) {
            Debug.Log("GameSelectionMenu: No selected game text");
            allGood = false;
        }
        if (scrollRectPages == null) {
            scrollRectPages = content.parent.GetComponent<ScrollRectPages>();
            if (scrollRectPages == null) {
                Debug.Log("GameSelectionMenu: No scroll rect pages on content's scroll rect");
                allGood = false;
            }
        }
        return allGood;
    }
}
