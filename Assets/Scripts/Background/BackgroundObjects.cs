using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundObject
{
    public Sprite backgroundSprite = null;
    public float movingBackgroudImageMultiplier = 1;

    [Space(10f)]
    public List<Sprite> floatingSprites = new List<Sprite>();
    public bool floatingsUseColor = false;
    public int maxNumberOfObjects = 100;
    public float spawnInterval = 2f;

    [Space(10f)]
    public Vector2Int objectsOnDimension = new Vector2Int(0, 6);

    [Space(10f)]
    public Vector2 movementMultipliers = new Vector2(1f, 1f);
    public Vector2 sizeMultipliers = new Vector2(1f, 1f);
}

public static class Background
{
    public static Sprite backgroundSprite;
    public static float movingBackgroudImageMultiplier;

    public static List<Sprite> floatingSprites;
    public static bool floatingsUseColor;
    public static int maxNumberOfObjects;
    public static float spawnInterval;

    public static Vector2 objectsOnDimension;

    public static Vector2 movementMultipliers;
    public static Vector2 sizeMultipliers;

    public static void Set(BackgroundObject background) {
        Background.backgroundSprite = background.backgroundSprite;
        Background.movingBackgroudImageMultiplier = background.movingBackgroudImageMultiplier;

        Background.floatingSprites = background.floatingSprites;
        Background.floatingsUseColor = background.floatingsUseColor;
        Background.maxNumberOfObjects = background.maxNumberOfObjects;
        Background.spawnInterval = background.spawnInterval;

        Background.objectsOnDimension = background.objectsOnDimension;

        Background.movementMultipliers = background.movementMultipliers;
        Background.sizeMultipliers = background.sizeMultipliers;
    }
}