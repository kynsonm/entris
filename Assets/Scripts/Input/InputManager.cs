using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Inputs
    [HideInInspector] public KeyCode up1, down1, left1, right1;
    [HideInInspector] public KeyCode up2, down2, left2, right2;

    [HideInInspector] public KeyCode rotateL1, rotateR1;
    [HideInInspector] public KeyCode rotateL2, rotateR2;

    [HideInInspector] public KeyCode place1,  place2;

    // Start is called before the first frame update
    void Start() {
        Load();
        Debug.Log(AllInputs());
        StartCoroutine(SaveEnum());
    }

    // Save input settings every X seconds
    IEnumerator SaveEnum() {
        while (true) {
            yield return new WaitForSeconds(5f);
            Save();
        }
    }

    // Load input key settings from PlayerPrefs
    public void Load() {
        if (!PlayerPrefs.HasKey(nameof(up1))) {
            FirstRun();
            return;
        }
        up1 = (KeyCode)PlayerPrefs.GetInt(nameof(up1));
        up2 = (KeyCode)PlayerPrefs.GetInt(nameof(up2));
        down1 = (KeyCode)PlayerPrefs.GetInt(nameof(down1));
        down2 = (KeyCode)PlayerPrefs.GetInt(nameof(down2));
        left1 = (KeyCode)PlayerPrefs.GetInt(nameof(left1));
        left2 = (KeyCode)PlayerPrefs.GetInt(nameof(left2));
        right1 = (KeyCode)PlayerPrefs.GetInt(nameof(right1));
        right2 = (KeyCode)PlayerPrefs.GetInt(nameof(right2));

        rotateL1 = (KeyCode)PlayerPrefs.GetInt(nameof(rotateL1));
        rotateL2 = (KeyCode)PlayerPrefs.GetInt(nameof(rotateL2));
        rotateR1 = (KeyCode)PlayerPrefs.GetInt(nameof(rotateR1));
        rotateR2 = (KeyCode)PlayerPrefs.GetInt(nameof(rotateR2));

        place1 = (KeyCode)PlayerPrefs.GetInt(nameof(place1));
        place2 = (KeyCode)PlayerPrefs.GetInt(nameof(place2));

        InputCodes.Set(this);
    }

    // Save input settings to PlayerPrefs
    public void Save() {
        if (up1 == KeyCode.None || down1 == KeyCode.None || rotateL1 == KeyCode.None || place1 == KeyCode.None)
        { FirstRun(); }

        PlayerPrefs.SetInt(nameof(up1), (int)up1);
        PlayerPrefs.SetInt(nameof(up2), (int)up2);
        PlayerPrefs.SetInt(nameof(down1), (int)down1);
        PlayerPrefs.SetInt(nameof(down2), (int)down2);
        PlayerPrefs.SetInt(nameof(left1), (int)left1);
        PlayerPrefs.SetInt(nameof(left2), (int)left2);
        PlayerPrefs.SetInt(nameof(right1), (int)right1);        
        PlayerPrefs.SetInt(nameof(right2), (int)right2);

        PlayerPrefs.SetInt(nameof(rotateL1), (int)rotateL1);
        PlayerPrefs.SetInt(nameof(rotateL2), (int)rotateL2);
        PlayerPrefs.SetInt(nameof(rotateR1), (int)rotateR1);
        PlayerPrefs.SetInt(nameof(rotateR2), (int)rotateR2);

        PlayerPrefs.SetInt(nameof(place1), (int)place1);
        PlayerPrefs.SetInt(nameof(place2), (int)place2);
    }

    // Give each input their default values
    public void FirstRun() {
        up1 = KeyCode.UpArrow;
        up2 = KeyCode.W;
        down1 = KeyCode.DownArrow;
        down2 = KeyCode.S;
        left1 = KeyCode.LeftArrow;
        left2 = KeyCode.A;
        right1 = KeyCode.RightArrow;
        right2 = KeyCode.D;

        rotateL1 = KeyCode.Q;
        rotateL2 = KeyCode.None;
        rotateR1 = KeyCode.E;
        rotateR2 = KeyCode.None;

        place1 = KeyCode.Space;
        place2 = KeyCode.Return;

        InputCodes.Set(this);
        Save();
    }

    public string AllInputs() {
        string log = "--- ALL INPUTS ---\n";
        log += inputString("up", up1, up2);
        log += inputString("down", down1, down2);
        log += inputString("left", left1, left2);
        log += inputString("right", right1, right2);

        log += inputString("rotateL", rotateL1, rotateL2);
        log += inputString("rotateR", rotateR1, rotateR2);

        log += inputString("place", place1, place2);
        return log;
    }
    string inputString(string name, KeyCode k1, KeyCode k2) {
        return name + " -- " + k1.ToString() + " || " + k2.ToString() + "\n";
    }
}
