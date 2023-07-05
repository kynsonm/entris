using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro.EditorUtilities;
using L = UnityEditor.EditorGUILayout;

/*
[CustomEditor(typeof(HorizontalLayoutEditor), true)]
public class HorizontalLayoutGroupEditor : UnityEditor.UI.HorizontalOrVerticalLayoutGroupEditor
{
    bool showHorSizes = false;
    bool lastExpandWidth = true;
    public override void OnInspectorGUI()
    {
        HorizontalLayoutEditor hor = (HorizontalLayoutEditor)target;

        hor.controlWidth = L.Toggle("Control Child Width", hor.controlWidth);
        hor.expandWidth = L.Toggle("Expand Children to Width", hor.expandWidth);
        if (lastExpandWidth != hor.expandWidth) {
            lastExpandWidth = hor.expandWidth;
            hor.Reset();
        }

        L.Space(5f);
        if (hor.controlWidth) {
            showHorSizes = L.Foldout(showHorSizes, "Children Sizes - Preferred (relative)");
        } else {
            showHorSizes = L.Foldout(showHorSizes, "Children Sizes - Multiplied by ScreenWidth");
        }
        if (showHorSizes) {
            for (int i = 0; i < hor.sizes.Count; ++i) {
                hor.sizes[i] = L.FloatField(hor.objects[i].name, hor.sizes[i]);
            }
        }
        L.Space(10f);

        //GUILayoutOption[] options = new GUILayoutOption[]{ GUILayout.ExpandWidth(true) };
        float size = Screen.width;
        GUILayoutOption[] label = { GUILayout.MaxWidth(size/5f) };
        GUILayoutOption[] area  = { GUILayout.MaxWidth(size/6f) };
        GUILayoutOption[] space = { GUILayout.MaxWidth(size/20f) };
        L.BeginHorizontal();
        L.LabelField("Pad Left", label);
        hor.horPaddingMultiplier.x = L.FloatField(hor.horPaddingMultiplier.x, area);
        L.LabelField("", space);
        L.LabelField("Pad Right", label);
        hor.horPaddingMultiplier.y = L.FloatField(hor.horPaddingMultiplier.y, area);
        L.EndHorizontal();

        L.BeginHorizontal();
        L.LabelField("Pad Top", label);
        hor.vertPaddingMultiplier.x = L.FloatField(hor.vertPaddingMultiplier.x, area);
        L.LabelField("", space);
        L.LabelField("Pad Bottom", label);
        hor.vertPaddingMultiplier.y = L.FloatField(hor.vertPaddingMultiplier.y, area);
        L.EndHorizontal();

        hor.spacingDivider = L.FloatField("Spacing - Divides max scren dimension", hor.spacingDivider);

        // Draw inspector UI of ImageEditor
        L.Space(10f);
        base.OnInspectorGUI();
    }
}

[CustomEditor(typeof(VerticalLayoutEditor), true)]
public class VerticalLayoutGroupEditor : UnityEditor.UI.HorizontalOrVerticalLayoutGroupEditor
{
    bool showVertSizes = false;
    bool lastExpandHeight = true;
    public override void OnInspectorGUI()
    {
        VerticalLayoutEditor vert = (VerticalLayoutEditor)target;

        vert.controlHeight = L.Toggle("Control Child Height", vert.controlHeight);
        vert.expandHeight = L.Toggle("Expand Children to Height", vert.expandHeight);
        if (lastExpandHeight != vert.expandHeight) {
            lastExpandHeight = vert.expandHeight;
            vert.Reset();
        }

        L.Space(5f);
        if (vert.controlHeight) {
            showVertSizes = L.Foldout(showVertSizes, "Children Sizes - Preferred (relative)");
        } else {
            showVertSizes = L.Foldout(showVertSizes, "Children Sizes - Multiplied by ScreenHeight");
        }
        if (showVertSizes) {
            for (int i = 0; i < vert.sizes.Count; ++i) {
                vert.sizes[i] = L.FloatField(vert.objects[i].name, vert.sizes[i]);
            }
        }
        L.Space(10f);

        //GUILayoutOption[] options = new GUILayoutOption[]{ GUILayout.ExpandWidth(true) };
        float size = Screen.width;
        GUILayoutOption[] label = { GUILayout.MaxWidth(size/5f) };
        GUILayoutOption[] area  = { GUILayout.MaxWidth(size/6f) };
        GUILayoutOption[] space = { GUILayout.MaxWidth(size/20f) };
        L.BeginHorizontal();
        L.LabelField("Pad Left", label);
        vert.horPaddingMultiplier.x = L.FloatField(vert.horPaddingMultiplier.x, area);
        L.LabelField("", space);
        L.LabelField("Pad Right", label);
        vert.horPaddingMultiplier.y = L.FloatField(vert.horPaddingMultiplier.y, area);
        L.EndHorizontal();

        L.BeginHorizontal();
        L.LabelField("Pad Top", label);
        vert.vertPaddingMultiplier.x = L.FloatField(vert.vertPaddingMultiplier.x, area);
        L.LabelField("", space);
        L.LabelField("Pad Bottom", label);
        vert.vertPaddingMultiplier.y = L.FloatField(vert.vertPaddingMultiplier.y, area);
        L.EndHorizontal();

        vert.spacingDivider = L.FloatField("Spacing - Divides max scren dimension", vert.spacingDivider);

        // Draw inspector UI of ImageEditor
        L.Space(10f);
        base.OnInspectorGUI();
    }
}
*/