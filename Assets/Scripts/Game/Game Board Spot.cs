using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game {

// One spot/pixel on the game board
public class Spot {
    // Objects
    public GameObject gameObject;
    public Image image;
    public BlockTheme theme;

    // Properties
    public int x, y;
    public bool isMoving;

    // Get properties
    public bool isBackground() {
        bool back = true;
        back = back && theme.color == BlockColor.none;
        back = back && image.sprite == Theme.backgroundSprite;
        return back;
    }

    // Set the spot to the given block color
    // Turn off ghosting
    public void Set(BlockColor color) {
        theme.isGhost = false;
        theme.color = color;
        if (image.sprite != Theme.blockSprite) {
            image.sprite = Theme.blockSprite;
        }
        ResetTheme();
    }
    public void SetMoving(BlockColor color) {
        isMoving = true;
        Set(color);
    }

    // Set the spot to a ghost block
    public void SetGhost() {
        isMoving = false;
        theme.isGhost = true;
        theme.color = BlockColor.none;
        if (image.sprite != Theme.blockSprite) {
            image.sprite = Theme.blockSprite;
        }
        ResetTheme();
    }

    // Reset the spot to the background sprite/color
    public void SetBackground() { Reset(); }
    public void Reset() {
        isMoving = false;
        theme.isGhost = false;
        theme.color = BlockColor.none;
        if (image.sprite != Theme.backgroundSprite) {
            image.sprite = Theme.backgroundSprite;
        }
        theme.Reset();
    }

    public void ResetTheme() {
        theme.Reset();
    }

    // Constructor
    public Spot(GameObject spotObject, int x_in, int y_in) {
        if (spotObject == null) { 
            Debug.LogError("Inputted SpotParent is null");
            return;
        }
        gameObject = spotObject;
        if (gameObject.name.ToLower().Contains("clone")) {
            gameObject.name = $"Spot ({x_in}, {y_in})";
        }

        image = gameObject.GetComponent<Image>();
        theme = gameObject.GetComponent<BlockTheme>();
        theme.color = BlockColor.none;
        theme.isGhost = false;
        x = x_in; y = y_in;

        isMoving = false;
        
        ResetTheme();
    }

    public bool CheckObjects() {
        if (gameObject == null) { return false; }
        if (image == null) { return false; }
        if (theme == null) { return false; }
        return true;
    }

    // String that thing!
    public override string ToString() {
        string str = $"\"{gameObject.name}\" -- ({x}, {y}) -- isMoving?: {isMoving.ToString()}";
        return str;
    }
}}