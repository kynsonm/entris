using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DoubleScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] ScrollRect frontScroll, backScroll;
    [SerializeField] [Range(0f, 90f)] float angleToTurnOffFront;
    [SerializeField] [Range(0f, 1f)] float distanceToCheckMultiplier;
    bool verticalCheck;

    RectTransform rectTransform;
    Vector2 startPosition, currentPosition;
    Vector2 angleTo, angleFrom;
    float distance, distanceToCheck;
    float angle;

    bool objectsAreGood;
    bool doAngleCheck, backIsDragging;


    // Start is called before the first frame update
    void Start() {
        objectsAreGood = CheckObjects();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        objectsAreGood = CheckObjects();
        if (!objectsAreGood) { return; }

        startPosition = new Vector2(eventData.position.x, eventData.position.y);
        distanceToCheck = distanceToCheckMultiplier * (float)Mathf.Min(rectTransform.rect.width, rectTransform.rect.height);
        distance = 0f;
        angle = 0f;

        angleFrom = new Vector2();
        if (verticalCheck) {
            angleFrom.x = 0f;
        } else {
            angleFrom.y = 0f;
        }

        doAngleCheck = true;
        backIsDragging = false;

        backScroll.SendMessage( "OnBeginDrag", eventData );
    }

    public void OnDrag(PointerEventData eventData) {
        if (backIsDragging) {
            backScroll.SendMessage( "OnDrag", eventData );
            return;
        }
        if (!doAngleCheck || !objectsAreGood) { return; }

        currentPosition = new Vector2(eventData.position.x, eventData.position.y);
        distance = Vector2.Distance(startPosition, currentPosition);

        if (distance >= distanceToCheck) {
            angleTo = (currentPosition - startPosition).normalized;
            angleFrom = AngleAxis(angleTo);
            angle = Vector2.Angle(angleFrom, angleTo);

            if (angle < angleToTurnOffFront) {
                backIsDragging = true;
            }
            doAngleCheck = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!objectsAreGood) { return; }
        backScroll.SendMessage( "OnEndDrag", eventData );
    }

    Vector2 AngleAxis(Vector2 vector) {
        if (verticalCheck) {
            if (vector.y < 0) {
                return Vector2.down;
            } else {
                return Vector2.up;
            }
        } else {
            if (vector.x < 0) {
                return Vector2.left;
            } else {
                return Vector2.right;
            }
        }
    }


    // Make sure scroll rects exist and are opposite dimensions
    bool CheckObjects() {
        if (frontScroll == null || backScroll == null) {
            Debug.Log("DoubleScrollRect: Front scroll rect and/or back scroll rect are not set");
            return false;
        }

        if (!frontScroll.gameObject.activeInHierarchy || !backScroll.gameObject.activeInHierarchy) {
            Debug.Log("DoubleScrollRect: One of the scroll rects is not active");
            return false;
        }

        if (frontScroll.vertical && frontScroll.horizontal) {
            Debug.Log("DoubleScrollRect: Front scroll rect is both vertical and horizontal");
            return false;
        }
        if (backScroll.vertical && backScroll.horizontal) {
            Debug.Log("DoubleScrollRect: Back scroll rect is both vertical and horizontal");
            return false;
        }
        if (frontScroll.vertical && backScroll.vertical || frontScroll.horizontal && backScroll.horizontal) {
            Debug.Log("DoubleScrollRect: Front and back scroll rects match dimensions");
            return false;
        }

        if (rectTransform == null) {
            rectTransform = frontScroll.viewport;
        }

        verticalCheck = backScroll.vertical;
        return true;
    }
}
