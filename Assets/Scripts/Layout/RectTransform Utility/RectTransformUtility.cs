using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class RectTransformSizing
{
    // Sets the size of the parent Object to that of the combined size of the children
    public static void SetWidthToWidthOfChildren(Transform transform) {
        SetWidthToWidthOfChildren(transform.GetComponent<RectTransform>(), true);
    }
    public static void SetWidthToWidthOfChildren(Transform transform, bool parentSizeIsMinimum) {
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        float width = WidthOfChildren(rectTransform);
        if (parentSizeIsMinimum) {
            RectTransform parentRect = rectTransform.parent.GetComponent<RectTransform>();
            width = (width < parentRect.rect.width) ? parentRect.rect.width : width;
        }
        RectTransformOffset.SetWidth(rectTransform, width);
    }
    
    public static void SetHeightToHeightOfChildren(Transform transform) {
        SetHeightToHeightOfChildren(transform.GetComponent<RectTransform>(), true);
    }
    public static void SetHeightToHeightOfChildren(Transform transform, bool parentSizeIsMinimum) {
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        float height = HeightOfChildren(rectTransform);
        if (parentSizeIsMinimum) {
            RectTransform parentRect = rectTransform.parent.GetComponent<RectTransform>();
            height = (height < parentRect.rect.height) ? parentRect.rect.height : height;
        }
        RectTransformOffset.SetHeight(rectTransform, height);
    }

    public static void SetSizeToSizeOfChildren(Transform transform) {
        SetSizeToSizeOfChildren(transform.GetComponent<RectTransform>(), true);
    }
    public static void SetSizeToSizeOfChildren(Transform transform, bool parentSizeIsMinimum) {
        SetWidthToWidthOfChildren(transform, parentSizeIsMinimum);
        SetHeightToHeightOfChildren(transform, parentSizeIsMinimum);
    }


    // Gets the combined height and width of the children on <rectTransform>
    public static float HeightOfChildren(RectTransform rectTransform) {
        return SizeOfChildren(rectTransform).y;
    }
    public static float WidthOfChildren(RectTransform rectTransform) {
        return SizeOfChildren(rectTransform).x;
    }
    public static Vector2 SizeOfChildren(RectTransform rectTransform) {
        Vector2 sizes = new Vector2(0f, 0f);

        // Size of each child rect
        foreach (Transform child in rectTransform.transform) {
            RectTransform childRect = child.GetComponent<RectTransform>();
            sizes.x += childRect.rect.width;
            sizes.y += childRect.rect.height;
        }

        // Add VerticalLayoutGroup paddings, if necessary
        VerticalLayoutGroup vert = rectTransform.gameObject.GetComponent<VerticalLayoutGroup>();
        if (vert != null) {
            sizes.y += vert.padding.top + vert.padding.bottom;
            sizes.y += vert.spacing * (float)(rectTransform.childCount - 1);
        }

        // Add HorizontalLayoutGroup paddings, if necessary
        HorizontalLayoutGroup hor = rectTransform.gameObject.GetComponent<HorizontalLayoutGroup>();
        if (hor != null) {
            sizes.x += hor.padding.left + hor.padding.right;
            sizes.x += hor.spacing * (float)(rectTransform.childCount - 1);
        }

        return sizes;
    }
}
