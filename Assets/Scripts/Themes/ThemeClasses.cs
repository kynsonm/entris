using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UnityEngine.UI {

    [System.Serializable]
    public class ThemeClass
    {
        public string name;
        [TextArea(2, 5)] public string description;

        [Space(10f)]
        public Color background;

        [Space(10f)]
        public Color UI_main;
        public Color UI_background;
        public Color UI_accent1, UI_accent2;

        [Space(10f)]
        public Color text_main;
        public Color text_background;
        public Color text_accent1, text_accent2;

        [Space(10f)]
        public Color spotBackgroundColor;
        public Color ghostBlock;
        public List<Color> blockColors;
    }

    [System.Serializable]
    public class FontClass
    {
        public string name;
        [TextArea(2, 5)] public string description;

        [Space(10f)]
        public TMP_FontAsset main;
        public TMP_FontAsset bold, thin;
    }

    [System.Serializable]
    public class BlockClass
    {
        public string name;
        [TextArea(2, 5)] public string description;

        [Space(10f)]
        public Sprite backgroundSprite;
        public Sprite blockSprite;
    }

    [System.Serializable]
    public enum ImageColor
    {
        background, main, UI_background, accent1, accent2,
        invalid = -1
    }


    [System.Serializable]
    public enum TextColor
    {
        main, background, accent1, accent2,
        invalid = -1
    }

    [System.Serializable]
    public enum BlockColor
    {
        invalid = -1,
        none = 0,
        _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12,
    }

    [System.Serializable]
    public enum ThemeColor {
        /* Image */ background, UI_main, UI_background, UI_accent1, UI_accent2,
        /* Text  */ text_main, text_background, text_accent1, text_accent2,
        /* Block */ b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12,
        bNone,
        invalid = -1
    }

    [System.Serializable]
    public enum FontType
    {
        main, bold, thin,
    }
    
}