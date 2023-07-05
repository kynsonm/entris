using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class ContextMenuAdditions
{
    [MenuItem("CONTEXT/Image/Add Image Theme")]
    static void MakeThemedImage(MenuCommand command)
    {
        Image image = (Image)command.context;
        GameObject gameObject = image.gameObject;

        if (gameObject.GetComponent<ImageTheme>() != null) {
            Debug.LogWarning("Object already has a ImageTheme script!");
            return;
        }

        ImageTheme theme = gameObject.AddComponent<ImageTheme>();
        theme.color = ThemeColor.UI_main;
        theme.alpha = image.color.a;
        theme.Reset();
    }

    [MenuItem("CONTEXT/TMP_Text/Add Text Theme")]
    static void MakeThemedText(MenuCommand command)
    {
        TMP_Text text = (TMP_Text)command.context;
        GameObject gameObject = text.gameObject;

        if (gameObject.GetComponent<TextTheme>() != null) {
            Debug.LogWarning("Object already has a TextTheme script!");
            return;
        }

        TextTheme theme = gameObject.AddComponent<TextTheme>();
        theme.color = TextColor.main;
        theme.font = FontType.main;
        theme.Reset();
    }
}

/*
public class ContextMenuAdditions
{
    [MenuItem("CONTEXT/Image/Make Themed Image")]
    static void MakeThemedImage(MenuCommand command)
    {
        // Get object information
        Image image = (Image)command.context;
        GameObject gameObject = image.gameObject;

        // Make a copy since we have to destory the original Image first
        GameObject copyObject = GameObject.Instantiate(gameObject, null);
        copyObject.name = gameObject.name;
        Object.DestroyImmediate(image);
        image = copyObject.GetComponent<Image>();

        ThemedImage themedImage = gameObject.AddComponent<ThemedImage>();

        // Set Image Properties
        themedImage.alphaHitTestMinimumThreshold = image.alphaHitTestMinimumThreshold;
        //themedImage.color = image.color;
        themedImage.material = image.material;
        themedImage.overrideSprite = image.overrideSprite;
        themedImage.preserveAspect = image.preserveAspect;
        themedImage.sprite = image.sprite;
        themedImage.type = image.type;
        themedImage.useSpriteMesh = image.useSpriteMesh;

        // Set inherited image properties
        themedImage.enabled = image.enabled;
        themedImage.tag = image.tag;
        //themedImage.color = image.color;
        themedImage.material = image.material;
        themedImage.raycastTarget = image.raycastTarget;
        themedImage.raycastPadding = image.raycastPadding;
        themedImage.maskable = image.maskable;
        themedImage.onCullStateChanged = image.onCullStateChanged;
        themedImage.runInEditMode = image.runInEditMode;
        themedImage.useGUILayout = image.useGUILayout;
        themedImage.hideFlags = image.hideFlags;
        themedImage.name = image.name;

        themedImage.Reset();

        // Destroy the copy
        GameObject.DestroyImmediate(copyObject);
    }

    [MenuItem("CONTEXT/TMP_Text/Make Themed Text")]
    static void MakeThemedText(MenuCommand command)
    {
        TMP_Text text = (TMP_Text)command.context;
        GameObject gameObject = text.gameObject;

        // Make a copy since we have to destory the original Image first
        GameObject copyObject = GameObject.Instantiate(gameObject, null);
        copyObject.name = gameObject.name;
        Object.DestroyImmediate(text);
        text = copyObject.GetComponent<TMP_Text>();

        ThemedText themedText = gameObject.AddComponent<ThemedText>();

        // Set text properties
        themedText.text = text.text;
        themedText.textStyle = text.textStyle;

        //themedText.font = text.font;
        themedText.fontMaterial = text.fontMaterial;
        themedText.fontMaterials = text.fontMaterials;
        themedText.fontSharedMaterial = text.fontSharedMaterial;
        themedText.fontSharedMaterials = text.fontSharedMaterials;
        themedText.material = text.material;
        themedText.fontStyle = text.fontStyle;
        themedText.enableAutoSizing = true;
        themedText.fontSizeMax = 9999f;
        //themedText.color = text.color;
        themedText.colorGradient = text.colorGradient;
        themedText.overrideColorTags = text.overrideColorTags;

        themedText.characterSpacing = text.characterSpacing;
        themedText.lineSpacing = text.lineSpacing;
        themedText.paragraphSpacing = text.paragraphSpacing;
        themedText.wordSpacing = text.wordSpacing;

        themedText.alignment = text.alignment;
        themedText.enableWordWrapping = text.enableWordWrapping;
        themedText.overflowMode = text.overflowMode;
        themedText.horizontalMapping = text.horizontalMapping;
        themedText.verticalMapping = text.verticalMapping;

        themedText.margin = new Vector4(text.margin.x, text.margin.y, text.margin.z, text.margin.w);
        themedText.spriteAsset = text.spriteAsset;
        themedText.styleSheet = text.styleSheet;
        themedText.enableKerning = text.enableKerning;

        themedText.Reset();

        // Destroy the copy
        GameObject.DestroyImmediate(copyObject);
    }
}
*/