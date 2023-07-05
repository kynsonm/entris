using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class Theme
{
    public static string name, description;

    public static Color background;

    public static Color UI_main;
    public static Color UI_background;
    public static Color UI_accent1, UI_accent2;

    public static Color text_main;
    public static Color text_background;
    public static Color text_accent1, text_accent2;

    public static Color spotBackgroundColor, ghostBlock;
    public static List<Color> blockColors;
    public static Sprite backgroundSprite, blockSprite;

    public static string font_name, font_description;
    public static TMP_FontAsset font_main, font_bold, font_thin;

    static ThemeManager themeManager;

    public static void SetTheme(ThemeClass theme, FontClass font, BlockClass blocks) {
        if (theme != null)  { SetTheme(theme); }
        if (font != null)   { SetFont(font); }
        if (blocks != null) { SetBlocks(blocks); }
        Reset();
    }
    public static void SetTheme(ThemeClass theme) {
        Theme.name = theme.name;
        Theme.description = theme.description;

        Theme.background = theme.background;

        Theme.UI_main = theme.UI_main;
        Theme.UI_background = theme.UI_background;
        Theme.UI_accent1 = theme.UI_accent1;
        Theme.UI_accent2 = theme.UI_accent2;

        Theme.text_main = theme.text_main;
        Theme.text_background = theme.text_background;
        Theme.text_accent1 = theme.text_accent1;
        Theme.text_accent2 = theme.text_accent2;

        Theme.spotBackgroundColor = theme.spotBackgroundColor;
        Theme.ghostBlock = theme.ghostBlock;
        Theme.blockColors = theme.blockColors;

        Reset();
    }
    public static void SetFont(FontClass font) {
        Theme.font_name = font.name;
        Theme.font_description = font.description;

        Theme.font_main = font.main;
        Theme.font_bold = font.bold;
        Theme.font_thin = font.thin;
        Reset();
    }
    public static void SetBlocks(BlockClass blocks) {
        Theme.backgroundSprite = blocks.backgroundSprite;
        Theme.blockSprite = blocks.blockSprite;
        Reset();
    }

    public static void Reset() {
        if (!checkThemeManager()) { return; }
        if (!themeIsSelected()) { SetTheme(themeManager.SelectedTheme(), themeManager.SelectedFont(), themeManager.SelectedBlocks()); }
        ResetImageThemes();
        ResetTextTheme();
        ResetBlockTheme();
    }
    public static void ResetImageThemes() {
        foreach (ImageTheme img in GameObject.FindObjectsOfType<ImageTheme>(true)) {
            img.Reset();
        }
    }
    public static void ResetTextTheme() {
        foreach (TextTheme text in GameObject.FindObjectsOfType<TextTheme>(true)) {
            text.Reset();
        }
    }
    public static void ResetBlockTheme() {
        foreach (BlockTheme block in GameObject.FindObjectsOfType<BlockTheme>(true)) {
            block.Reset();
        }
    }


    // ----- COLOR RETRIEVAL -----

    // Get colors from ImageColor, TextColor, or BlockColor
    public static Color ColorFromType(ImageColor imageColor) {
        if (!themeIsSelected()) { Reset(); }
        switch (imageColor) {
            case ImageColor.UI_background: return UI_background;
            case ImageColor.main:   return UI_main;
            case ImageColor.accent1: return UI_accent1;
            case ImageColor.accent2: return UI_accent2;
            case ImageColor.background: return background;
        }
        // For invalid
        return Color.magenta;
    }
    public static Color ColorFromType(TextColor textColor) {
        if (!themeIsSelected()) { Reset(); }
        switch (textColor) {
            case TextColor.background: return text_background;
            case TextColor.main:   return text_main;
            case TextColor.accent1: return text_accent1;
            case TextColor.accent2: return text_accent2;
        }
        // For invalid
        return Color.magenta;
    }
    public static Color ColorFromType(BlockColor blockColor) {
        if (!themeIsSelected()) { Reset(); }
        if (blockColor == BlockColor.none) {
            return Theme.spotBackgroundColor;
        }
        int index = indexFromBlockColor(blockColor);
        if (blockColors == null || index > blockColors.Count) {
            Theme.Reset();
            index = indexFromBlockColor(blockColor);
            if (blockColors == null || index > blockColors.Count) {
                Debug.LogError("Block color \"" + blockColor.ToString() + "\" has index too large for theme's blockColors!");
                return Color.magenta;
            }
        }
        return blockColors[index];
    }
    public static Color ColorFromType(ThemeColor themeColor) {
        if (!themeIsSelected()) { Reset(); }
        if (themeColor == ThemeColor.invalid) { return Color.magenta; }
        switch (themeColor) {
            case ThemeColor.background: return background;
            // UI
            case ThemeColor.UI_background: return UI_background;
            case ThemeColor.UI_main:    return UI_main;
            case ThemeColor.UI_accent1: return UI_accent1;
            case ThemeColor.UI_accent2: return UI_accent2;
            // Text
            case ThemeColor.text_main:   return text_main;
            case ThemeColor.text_background: return text_background;
            case ThemeColor.text_accent1: return text_accent1;
            case ThemeColor.text_accent2: return text_accent2;

            case ThemeColor.bNone: return spotBackgroundColor;
        }
        // Block colors
        int index = indexFromThemeColor(themeColor);
        if (blockColors == null || index == -1 || index >= blockColors.Count) { return Color.magenta; }
        return blockColors[index];
    }

    // ThemeColor to other colors
    public static ImageColor ImageColorFromTheme(ThemeColor themeColor) {
        switch (themeColor) {
            case ThemeColor.background: return ImageColor.background;
            case ThemeColor.UI_background: return ImageColor.UI_background;
            case ThemeColor.UI_main:    return ImageColor.main;
            case ThemeColor.UI_accent1: return ImageColor.accent1;
            case ThemeColor.UI_accent2: return ImageColor.accent2;
            default: return ImageColor.invalid;
        }
    }
    public static TextColor TextColorFromTheme(ThemeColor themeColor) {
        switch (themeColor) {
            case ThemeColor.text_main:    return TextColor.main;
            case ThemeColor.text_background: return TextColor.background;
            case ThemeColor.text_accent1: return TextColor.accent1;
            case ThemeColor.text_accent2: return TextColor.accent2;
            default: return TextColor.invalid;
        }
    }
    public static BlockColor BlockColorFromTheme(ThemeColor themeColor) {
        switch (themeColor) {
            case ThemeColor.bNone: return BlockColor.none;
            case ThemeColor.b1: return BlockColor._1;
            case ThemeColor.b2: return BlockColor._2;
            case ThemeColor.b3: return BlockColor._3;
            case ThemeColor.b4: return BlockColor._4;
            case ThemeColor.b5: return BlockColor._5;
            case ThemeColor.b6: return BlockColor._6;
            case ThemeColor.b7: return BlockColor._7;
            case ThemeColor.b8: return BlockColor._8;
            case ThemeColor.b9: return BlockColor._9;
            case ThemeColor.b10: return BlockColor._10;
            case ThemeColor.b11: return BlockColor._11;
            case ThemeColor.b12: return BlockColor._12;
            default: return BlockColor.invalid;
        }
    }

    // Get mixed colors
    public static Color MixedColor(ThemeColor originalColor, ThemeColor mixColor, float mixedColorPercentage) {
        return mix(Theme.ColorFromType(originalColor), Theme.ColorFromType(mixColor), mixedColorPercentage);
    }
    public static Color MixedColor(ImageColor originalColor, ThemeColor mixColor, float mixedColorPercentage) {
        return mix(Theme.ColorFromType(originalColor), Theme.ColorFromType(mixColor), mixedColorPercentage);
    }
    public static Color MixedColor(TextColor originalColor, ThemeColor mixColor, float mixedColorPercentage) {
        return mix(Theme.ColorFromType(originalColor), Theme.ColorFromType(mixColor), mixedColorPercentage);
    }
    public static Color MixedColor(BlockColor originalColor, ThemeColor mixColor, float mixedColorPercentage) {
        return mix(Theme.ColorFromType(originalColor), Theme.ColorFromType(mixColor), mixedColorPercentage);
    }


    // ----- STRINGS -----

    public static string ParseString(string text) {
        string str = text;
        str = str.Replace("<#m>", $"<#{ColorUtility.ToHtmlStringRGBA(text_main)}>");
        str = str.Replace("<#b>", $"<#{ColorUtility.ToHtmlStringRGBA(text_background)}>");
        str = str.Replace("<#a1>", $"<#{ColorUtility.ToHtmlStringRGBA(text_accent1)}>");
        str = str.Replace("<#a2>", $"<#{ColorUtility.ToHtmlStringRGBA(text_accent2)}>");
        str = str.Replace("<c>", "</color>");
        str = str.Replace("</c>", "</color>");
        return str;
    }


    // ----- UTILITIES -----

    static int indexFromBlockColor(BlockColor blockColor) {
        switch (blockColor) {
            case BlockColor._1: return 0;
            case BlockColor._2: return 1;
            case BlockColor._3: return 2;
            case BlockColor._4: return 3;
            case BlockColor._5: return 4;
            case BlockColor._6: return 5;
            case BlockColor._7: return 6;
            case BlockColor._8: return 7;
            case BlockColor._9: return 8;
            case BlockColor._10: return 9;
            case BlockColor._11: return 10;
            case BlockColor._12: return 11;
        }
        return -1;
    }
    static int indexFromThemeColor(ThemeColor themeColor) {
        switch (themeColor) {
            case ThemeColor.b1: return 0;
            case ThemeColor.b2: return 1;
            case ThemeColor.b3: return 2;
            case ThemeColor.b4: return 3;
            case ThemeColor.b5: return 4;
            case ThemeColor.b6: return 5;
            case ThemeColor.b7: return 6;
            case ThemeColor.b8: return 7;
            case ThemeColor.b9: return 8;
            case ThemeColor.b10: return 9;
            case ThemeColor.b11: return 10;
            case ThemeColor.b12: return 11;
        }
        return -1;
    }

    static Color mix(Color originalColor, Color mixColor, float mixedColorPercentage) {
        float q = mixedColorPercentage, p = 1f - q;

        float r = p*originalColor.r + q*mixColor.r;
        float g = p*originalColor.g + q*mixColor.g;
        float b = p*originalColor.b + q*mixColor.b;
        float a = originalColor.a;

        return new Color(r, g, b, a);
    }

    static bool checkThemeManager() {
        if (themeManager == null) {
            themeManager = GameObject.FindObjectOfType<ThemeManager>();
            if (themeManager == null) {
                Debug.LogWarning("No ThemeManger object in the scene!!");
                return false;
            }
        }
        return true;
    }
    static bool themeIsSelected() {
        bool allGood = colorIsNotBlack(Theme.UI_accent1);
        allGood = allGood && colorIsNotBlack(Theme.UI_accent2);
        allGood = allGood && colorIsNotBlack(Theme.text_accent1);
        allGood = allGood && colorIsNotBlack(Theme.text_accent2);
        return allGood;
    }
    static bool colorIsNotBlack(Color c) {
        if (c.r == 0f && c.g == 0f && c.b == 0f) { return false; }
        return true;
    }
}