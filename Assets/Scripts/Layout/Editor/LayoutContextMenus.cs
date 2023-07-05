using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class LayoutContextMenus
{
    [MenuItem("CONTEXT/HorizontalLayoutGroup/Add Horiztonal Editor")]
    static void MakeHorizontalEditor(MenuCommand command)
    {
        // Get object information
        HorizontalLayoutGroup horLayout = (HorizontalLayoutGroup)command.context;
        GameObject gameObject = horLayout.gameObject;

        if (gameObject.GetComponent<HorizontalLayoutEditor>() != null) {
            Debug.LogError("There is already a HorizontalLyoutEditor on this object!");
            return;
        }

        HorizontalLayoutEditor horEdit = gameObject.AddComponent<HorizontalLayoutEditor>();
        horEdit.controlWidth = horLayout.childControlWidth;
        horEdit.expandWidth = horLayout.childForceExpandWidth;
        horEdit.horPaddingMultiplier  = new Vector2(horLayout.padding.left / (float)Screen.width, horLayout.padding.right / (float)Screen.width);
        horEdit.vertPaddingMultiplier = new Vector2(horLayout.padding.top / (float)Screen.height, horLayout.padding.bottom / (float)Screen.height);
        horEdit.spacingDivider = (horLayout.spacing == 0f) ? 0f : (float)Screen.width / horLayout.spacing;
        horEdit.Reset();
    }

    [MenuItem("CONTEXT/VerticalLayoutGroup/Add Vertial Editor")]
    static void MakeVerticalEditor(MenuCommand command)
    {
        // Get object information
        VerticalLayoutGroup vertLayout = (VerticalLayoutGroup)command.context;
        GameObject gameObject = vertLayout.gameObject;

        if (gameObject.GetComponent<HorizontalLayoutEditor>() != null) {
            Debug.LogError("There is already a HorizontalLyoutEditor on this object!");
            return;
        }

        VerticalLayoutEditor vertEdit = gameObject.AddComponent<VerticalLayoutEditor>();
        vertEdit.controlHeight = vertLayout.childControlWidth;
        vertEdit.expandHeight = vertLayout.childForceExpandWidth;
        vertEdit.horPaddingMultiplier  = new Vector2(vertLayout.padding.left / (float)Screen.width, vertLayout.padding.right / (float)Screen.width);
        vertEdit.vertPaddingMultiplier = new Vector2(vertLayout.padding.top / (float)Screen.height, vertLayout.padding.bottom / (float)Screen.height);
        vertEdit.spacingDivider = (vertLayout.spacing == 0f) ? 0f : (float)Screen.width / vertLayout.spacing;
        vertEdit.Reset();
    }
}

/*
public class LayoutContextMenus
{
    [MenuItem("CONTEXT/HorizontalLayoutGroup/Make Horizontal Editor")]
    static void MakeHorizontalEditor(MenuCommand command)
    {
        // Get object information
        HorizontalLayoutGroup horLayout = (HorizontalLayoutGroup)command.context;
        GameObject gameObject = horLayout.gameObject;

        // Make a copy since we have to destory the original Image first
        GameObject copyObject = GameObject.Instantiate(gameObject, null);
        copyObject.name = gameObject.name;
        Object.DestroyImmediate(horLayout);
        horLayout = copyObject.GetComponent<HorizontalLayoutGroup>();

        HorizontalLayoutEditor horEdit = gameObject.AddComponent<HorizontalLayoutEditor>();

        horEdit.childAlignment = horLayout.childAlignment;
        horEdit.reverseArrangement = horLayout.reverseArrangement;
        horEdit.childControlWidth = true;
        horEdit.childControlHeight = true;

        horEdit.Reset();

        // Destroy the copy
        GameObject.DestroyImmediate(copyObject);
    }

    [MenuItem("CONTEXT/VerticalLayoutGroup/Make Vertical Editor")]
    static void MakeVerticalEditor(MenuCommand command)
    {
        // Get object information
        VerticalLayoutGroup vertLayout = (VerticalLayoutGroup)command.context;
        GameObject gameObject = vertLayout.gameObject;

        // Make a copy since we have to destory the original Image first
        GameObject copyObject = GameObject.Instantiate(gameObject, null);
        copyObject.name = gameObject.name;
        Object.DestroyImmediate(vertLayout);
        vertLayout = copyObject.GetComponent<VerticalLayoutGroup>();

        VerticalLayoutEditor vertEdit = gameObject.AddComponent<VerticalLayoutEditor>();

        vertEdit.childAlignment = vertLayout.childAlignment;
        vertEdit.reverseArrangement = vertLayout.reverseArrangement;
        vertEdit.childControlWidth = true;
        vertEdit.childControlHeight = true;

        vertEdit.Reset();

        // Destroy the copy
        GameObject.DestroyImmediate(copyObject);
    }
}
*/