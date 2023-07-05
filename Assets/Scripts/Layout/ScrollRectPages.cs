using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI {

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectPages : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [HideInInspector] public ScrollRect scrollRect { get; private set; }
    PinchableScrollRect pinchableScrollRect;
    [SerializeField] bool horizontal;
    [SerializeField] bool vertical;
    [Space(5f)]
    [SerializeField] float delayBeforeSelecting;
    [SerializeField] float selectionTime;
    [SerializeField] [Range(0f, 1f)] float velocityPower;
    [SerializeField] LeanTweenType selectionEaseCurve;
    [HideInInspector] public List<Transform> children { get; private set; }
    [HideInInspector] public Transform lastSelectedChild { get; private set; }

    // Start is called before the first frame update
    void Awake() { CheckScrollRect(); }
    IEnumerator Start() {
        CheckScrollRect();
        yield return new WaitForEndOfFrame();
        CheckScrollRect();
        StartCoroutine(WaitToSelect());
    }
    void OnEnable() { StartCoroutine(CheckChildrenEnum()); }
    IEnumerator CheckChildrenEnum() {
        int childCount = 0;
        while (true) {
            yield return new WaitForSeconds(0.25f);
            if (scrollRect == null) { Debug.Log("No scroll rect"); continue; }
            if (scrollRect.content == null) { Debug.Log("No scroll rect content"); continue; }
            if (scrollRect.content.childCount != childCount) {
                children = new List<Transform>();
                foreach (Transform child in scrollRect.content) {
                    children.Add(child);
                }
                childCount = scrollRect.content.childCount;
            }
        }
    }

    // Stop all selection when a drag starts
    public void OnBeginDrag(PointerEventData eventData) {
        StopAllCoroutines();
        gameObject.LeanCancel();
    }

    // On end drag, select the closest "menu" (child object)
    public void OnEndDrag(PointerEventData eventData) {
        StopAllCoroutines();
        gameObject.LeanCancel();
        StartCoroutine(WaitToSelect());
    }
    IEnumerator WaitToSelect() {
        yield return new WaitForSeconds(delayBeforeSelecting);
        SelectMenu();
    }
    void SelectMenu() {
        if (!CheckScrollRect()) { return; }
        RectTransform rect = scrollRect.content;
        Vector2 velocity = pow(scrollRect.velocity, velocityPower);
        Vector2 startPos = rect.position;
        Vector2 endPos = rect.position - ClosestChildOffset();
        if (!horizontal) { endPos.x = startPos.x; }
        if (!vertical)   { endPos.y = startPos.y; }

        LeanTween.value(gameObject, 0f, 1f, selectionTime)
        .setEase(selectionEaseCurve)
        .setOnStart(() => {
            scrollRect.velocity = new Vector2(0f, 0f);
        })
        .setOnUpdate((float value) => {
            scrollRect.velocity = new Vector2(0f, 0f);

            Vector2 pos = new Vector2(startPos.x, startPos.y);
            Vector2 newPos = (1f - value) * (startPos + velocity*value) + (endPos*value);
            if (horizontal) { pos.x = newPos.x; }
            if (vertical)   { pos.y = newPos.y; }

            rect.position = pos;
        })
        .setOnComplete(() => {
            scrollRect.velocity = new Vector2(0f, 0f);
            rect.position = endPos;
        });
    }

    Vector3 ClosestChildOffset() {
        if (scrollRect.content.transform.childCount == 0) { return new Vector3(0f, 0f, 0f); }
        Vector2 center = CenterOfRect(scrollRect.viewport);
        RectTransform closest = scrollRect.content.transform.GetChild(0).GetComponent<RectTransform>();
        float closestDistance = float.MaxValue;

        foreach (Transform child in scrollRect.content.transform) {
            Vector2 pos = CenterOfRect(child.gameObject.GetComponent<RectTransform>());
            float distance = Vector2.Distance(center, pos);
            if (distance < closestDistance) {
                closestDistance = distance;
                closest = child.gameObject.GetComponent<RectTransform>();
            }
        }

        lastSelectedChild = closest.transform;

        Vector2 offset = CenterOfRect(closest) - CenterOfRect(scrollRect.viewport);
        return offset;
    }
    Vector2 CenterOfRect(RectTransform rect) {
        Vector2 pos = rect.position;
        pos = rect.TransformPoint(rect.rect.center);

        return pos;
    }

    Vector2 pow(Vector2 num, float pow) {
        Vector2 signs = new Vector2(num.x < 0 ? -1f : 1f, num.y < 0 ? -1f : 1f);
        num = new Vector2(Mathf.Abs(num.x), Mathf.Abs(num.y));
        num = new Vector2(Mathf.Pow(num.x, pow), Mathf.Pow(num.y, pow));
        return signs * num;
    }


    // Get the scroll rect if necessary
    bool CheckScrollRect() {
        bool allGood = true;
        if (scrollRect == null) {
            scrollRect = gameObject.GetComponent<ScrollRect>();
            if (scrollRect == null) {
                Debug.Log("ScrollRectPages: No scroll rect on the object");
                allGood = false;
            }
        }
        if (pinchableScrollRect == null) {
            pinchableScrollRect = gameObject.GetComponent<PinchableScrollRect>();
            if (pinchableScrollRect != null) {
                scrollRect = pinchableScrollRect;
                allGood = true;
            }
        }
        return allGood;
    }
}}