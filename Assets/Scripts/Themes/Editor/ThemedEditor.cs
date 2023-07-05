using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/*
[CustomEditor(typeof(ThemedImage), true)]
public class ThemedImageEditor : UnityEditor.UI.ImageEditor
{
    public override void OnInspectorGUI()
    {
        ThemedImage image = (ThemedImage)target;

        image.updateColorOfImage = EditorGUILayout.Toggle("Update Color of Image", image.updateColorOfImage);

        EditorGUILayout.Space(5f);
        image.color = (ImageColor)EditorGUILayout.EnumPopup("Color", image.color);
        image.alpha = EditorGUILayout.Slider("Alpha", image.alpha, 0f, 1f);
        
        EditorGUILayout.Space(5f);
        image.useMixedColor = EditorGUILayout.Toggle("Use Mixed Color", image.useMixedColor);
        image.mixedColor = (ImageColor)EditorGUILayout.EnumPopup("Mixed Color", image.mixedColor);
        image.mixRatio = EditorGUILayout.Slider("Mix Ratio", image.mixRatio, 0f, 1f);

        EditorGUILayout.Space(5f);
        image.ignorePPUUpdate = EditorGUILayout.Toggle("Ignore PPU Update", image.ignorePPUUpdate);
        image.PPUMultiplier = EditorGUILayout.FloatField("PPU Multiplier", image.PPUMultiplier);

        // Draw inspector UI of ImageEditor
        EditorGUILayout.Space(10f);
        base.OnInspectorGUI();
    }
}


[CustomEditor(typeof(ThemedText))]
public class ThemedTextEditor : TMP_EditorPanelUI
{
    public override void OnInspectorGUI()
    {
        ThemedText text = (ThemedText)target;

        text.updateFont = EditorGUILayout.Toggle("Update Font", text.updateFont);
        text.updateTextSize = EditorGUILayout.Toggle("Update Text Size", text.updateTextSize);
        text.updateColor = EditorGUILayout.Toggle("Update Color", text.updateColor);

        EditorGUILayout.Space(5f);
        text.color = (TextColor)EditorGUILayout.EnumPopup("Color", text.color);
        text.alpha = EditorGUILayout.Slider("Alpha", text.alpha, 0f, 1f);
        text.font = (FontType)EditorGUILayout.EnumPopup("Font", text.font);
        text.maxFontSizePercentage = EditorGUILayout.Slider("Max Font Size Percentage", text.maxFontSizePercentage, 0f, 1f);

        EditorGUILayout.Space(5f);
        text.useMixedColor = EditorGUILayout.Toggle("Use Mixed Color", text.useMixedColor);
        text.mixedColor = (TextColor)EditorGUILayout.EnumPopup("Mixed Color", text.mixedColor);
        text.mixRatio = EditorGUILayout.Slider("Mix Ratio", text.mixRatio, 0f, 1f);

        // Draw inspector UI of ImageEditor
        base.OnInspectorGUI();
    }
}
*/