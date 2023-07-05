using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]

public class MenuMovement : MonoBehaviour
{
    [SerializeField] TweenSequence tweenSequence;
    MenuMovement menuMovement;
    MainMenuManager mainMenuManager;

    public void Open(GameObject toOpen) {
        Open(toOpen, true);
    }
    public void Open(GameObject toOpen, bool addToMainMenuManager) {
        bool opened = TweenObject(toOpen, true);
        if (opened && GetObjects()) {
            if (addToMainMenuManager) {
                Debug.Log("Adding menu " + toOpen.name + " to main menu manager");
                mainMenuManager.OpenMenu(toOpen);
            }

            mainMenuManager.ActivateObjects(toOpen);
        }
    }

    public void Close(GameObject toClose) {
        bool closed = TweenObject(toClose, false);
        if (closed && GetObjects()) {
            mainMenuManager.CloseMenu(toClose);

            mainMenuManager.ActivateObjects(toClose);
        }
    }
    public void CloseWithoutRemoving(GameObject toClose) {
        bool closed = TweenObject(toClose, false);
        if (closed && GetObjects()) {
            mainMenuManager.ActivateObjects(toClose);
        }
    }

    bool TweenObject(GameObject obj, bool isOpening) {
        if (obj == null) { return false; }
        if (tweenSequence == null || tweenSequence.events.Count == 0) { return false; }
        obj.LeanCancel();

        // Initial conditions
        CanvasGroup canv = GetCanvas(obj);

        TweenSequence seq = new TweenSequence(tweenSequence);
        if (!isOpening) {
            seq.InvertAlpha();
            seq.TurnOffOnEnd();
        } else {
            canv.alpha = 0f;
        }

        seq.DoTween(obj);
        return true;
    }


    // ----- UTILITIES -----

    CanvasGroup GetCanvas(GameObject obj) {
        CanvasGroup canv = obj.GetComponent<CanvasGroup>();
        if (canv == null) {
            canv = obj.AddComponent<CanvasGroup>();
        }
        return canv;
    }

    bool GetObjects() {
        bool allGood = true;
        if (menuMovement == null) {
            menuMovement = GameObject.FindObjectOfType<MenuMovement>();
            if (menuMovement == null) {
                Debug.Log("No MenuMovement in the scene");
                allGood = false;
            }
        }
        if (mainMenuManager == null) {
            mainMenuManager = GameObject.FindObjectOfType<MainMenuManager>();
            if (mainMenuManager == null) {
                Debug.Log("No MainMenuManager in the scene");
                allGood = false;
            }
        }
        return allGood;
    }


    // ----- EDITOR STUFF -----

#if UNITY_EDITOR
    bool firstRun = true;
    int lastEventsSize = 0;
    void CheckTweenEvents() {
        if (tweenSequence == null) { return; }
        if (tweenSequence.events == null) { tweenSequence.events = new List<TweenEvent>(); }
        if (firstRun) {
            lastEventsSize = tweenSequence.events.Count;
            firstRun = false;
            return;
        }
        if (lastEventsSize == tweenSequence.events.Count) { return; }
        if (lastEventsSize > tweenSequence.events.Count) {
            lastEventsSize = tweenSequence.events.Count;
            return;
        }
        lastEventsSize = tweenSequence.events.Count;

        TweenEvent tween = tweenSequence.events[lastEventsSize-1];
        tween.positionOffsetMultiplier = new Vector2(0f, 0f);
        tween.scale = new Vector3(1f, 1f, 1f);
        tween.rotationAngle = new Vector3(0f, 0f, 0f);
        tween.alpha = 1f;
        tween.easeCurve = LeanTweenType.linear;
        tween.timeMultiplier = 1f;
        tween.delay = 0f;
        tween.turnOffOnEnd = false;
        tween.turnOnOnStart = false;
    }

    void Update() {
        if (!Application.isPlaying) {
            CheckTweenEvents();
        }
    }
#endif

}
