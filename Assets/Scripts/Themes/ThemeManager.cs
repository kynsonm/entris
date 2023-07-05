using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]

public class ThemeManager : MonoBehaviour
{
    [SerializeField] bool alwaysResetThemes;
    [SerializeField] List<ThemeClass> themes;
    int currentThemeIndex;
    [SerializeField] List<FontClass> fonts;
    int currentFontIndex;
    [SerializeField] List<BlockClass> blocks;
    int currentBlockIndex;

    // Start is called before the first frame update
    private void Awake() {
        Settings.Load();
        currentThemeIndex = Settings.themeIndex;
        currentFontIndex = Settings.fontIndex;
        currentBlockIndex = Settings.blocksIndex;
        SetTheme(currentThemeIndex);
        SetFont(currentFontIndex);
        SetBlocks(currentBlockIndex);
    }
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        Awake();
    }

    public ThemeClass SelectedTheme() {
        if (themes == null || currentThemeIndex >= themes.Count) { return null; }
        return themes[currentThemeIndex];
    }
    public FontClass SelectedFont() {
        if (fonts == null || currentFontIndex >= fonts.Count) { return null; }
        return fonts[currentFontIndex];
    }
    public BlockClass SelectedBlocks() {
        if (blocks == null || currentBlockIndex >= blocks.Count) { return null; }
        return blocks[currentBlockIndex];
    }

    // Methods for setting the theme
    public void SetTheme(int index) {
        if (index >= themes.Count) { index = 0; }
        if (index < 0) { index = themes.Count - 1; }
        currentThemeIndex = index;

        Theme.SetTheme(themes[currentThemeIndex]);
    }
    public void NextTheme()     { SetTheme(++currentThemeIndex); }
    public void PreviousTheme() { SetTheme(--currentThemeIndex); }

    // Methods for setting the fonts
    public void SetFont(int index) {
        if (index >= fonts.Count) { index = 0; }
        if (index < 0) { index = fonts.Count - 1; }
        currentFontIndex = index;

        Theme.SetFont(fonts[currentFontIndex]);
    }
    public void NextFont()     { SetFont(++currentFontIndex); }
    public void PreviousFont() { SetFont(--currentFontIndex); }

    // Methods for setting blocks
    public void SetBlocks(int index) {
        if (index >= blocks.Count) { index = 0; }
        if (index < 0) { index = blocks.Count - 1; }
        currentBlockIndex = index;

        Theme.SetBlocks(blocks[currentBlockIndex]);
    }

#if UNITY_EDITOR
    bool coroutineRunning = false;
    private void Update() {
        if (alwaysResetThemes && !coroutineRunning) {
            StopAllCoroutines();
            StartCoroutine(ResetAllThemesEachSecond());
        }
    }
    IEnumerator ResetAllThemesEachSecond() {
        coroutineRunning = true;
        while (alwaysResetThemes) {
            yield return new WaitForSeconds(1f);
            Theme.Reset();
        }
        coroutineRunning = false;
    }
#endif
}
