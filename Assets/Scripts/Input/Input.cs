using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputCodes
{
    // Housekeeping
    public static void Set(InputManager m) {
        up1 = m.up1; up2 = m.up2;
        down1 = m.down1; down2 = m.down2;
        left1 = m.left1; left2 = m.left2;
        right1 = m.right1; right2 = m.right2;

        rotateL1 = m.rotateL1; rotateL2 = m.rotateL2;
        rotateR1 = m.rotateR1; rotateR2 = m.rotateR2;

        place1 = m.place1; place2 = m.place2;
    }

    // Inputs
    static KeyCode up1, down1, left1, right1;
    static KeyCode up2, down2, left2, right2;

    static KeyCode rotateL1, rotateR1;
    static KeyCode rotateL2, rotateR2;

    static KeyCode place1,  place2;

    // General
    public static bool select() { return place(); }

    // Directions
    public static bool left() {
        return Input.GetKeyDown(left1) || Input.GetKeyDown(left2);
    }
    public static bool right() {
        return Input.GetKeyDown(right1) || Input.GetKeyDown(right2);
    }
    public static bool up() {
        return Input.GetKeyDown(up1) || Input.GetKeyDown(up2);
    }
    public static bool down() {
        return Input.GetKeyDown(down1) || Input.GetKeyDown(down2);
    }
    public static bool holdDown() {
        return Input.GetKey(down1) || Input.GetKey(down2);
    }

    // Rotate
    public static bool rotateL() {
        return Input.GetKeyDown(rotateL1) || Input.GetKeyDown(rotateL2);
    }
    public static bool rotateR() {
        return Input.GetKeyDown(rotateR1) || Input.GetKeyDown(rotateR2);
    }

    // Speedup and placing
    public static bool speedUp() { return down(); }
    public static bool place() {
        return Input.GetKeyDown(place1) || Input.GetKeyDown(place2);
    }
}
