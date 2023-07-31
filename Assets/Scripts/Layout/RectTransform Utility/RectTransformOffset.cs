using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformOffset
{
    // ----- Set width/height to a specific value

    public static void SetWidth(RectTransform rect, float width) {
        RectTransform parentRect = rect.parent.GetComponent<RectTransform>();
        float newWidth = width - parentRect.rect.width * (rect.anchorMax.x - rect.anchorMin.x);
        rect.sizeDelta = new Vector2(newWidth, rect.sizeDelta.y);
    }
    public static void SetHeight(RectTransform rect, float height) {
        RectTransform parentRect = rect.parent.GetComponent<RectTransform>();
        float newHeight = height - parentRect.rect.height * (rect.anchorMax.y - rect.anchorMin.y);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
    }
    // TODO: Fix this before you use it!
    public static void SetDimensions(RectTransform rect, Vector2 size) {
        RectTransform parentRect = rect.parent.GetComponent<RectTransform>();
        SetWidth(rect, size.x);
        SetHeight(rect, size.y);
    }


    // ----- Set all dimensions ever -----
    
    public static void All(RectTransform rect, float size) {
        Left(rect, size);
        Right(rect, size);
        Top(rect, size);
        Bottom(rect, size);
    }


    // ----- SET HOR OR VERT DIMENSIONS -----

    public static void Horizontal(RectTransform rect, float size) {
        Left(rect, size);
        Right(rect, size);
    }
    public static void Sides(RectTransform rect, float size) {
        Horizontal(rect, size);
    }

    public static void Vertical(RectTransform rect, float size) {
        Top(rect, size);
        Bottom(rect, size);
    }


    // ----- SET EACH SIDE -----

    public static void Left(RectTransform rect, float left) {
        rect.offsetMin = new Vector2(left, rect.offsetMin.y);
    }

    public static void Right(RectTransform rect, float right) {
        rect.offsetMax = new Vector2(-right, rect.offsetMax.y);
    }

    public static void Top(RectTransform rect, float top) {
        rect.offsetMax = new Vector2(rect.offsetMax.x, -top);
    }

    public static void Bottom(RectTransform rect, float bottom) {
        rect.offsetMin = new Vector2(rect.offsetMin.x, bottom);
    }
}
