using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]

public class DEBUG_ResetAllLayoutGroups : MonoBehaviour
{
    [SerializeField] bool doReset;

    private void Update() {
        if (doReset) {
            ResetAll();
            doReset = false;
        }
    }

    void ResetAll() {
        foreach (VerticalLayoutEditor edit in GameObject.FindObjectsOfType<VerticalLayoutEditor>()) {
            edit.AddAllElements();
            edit.Reset();
        }
        foreach (HorizontalLayoutEditor edit in GameObject.FindObjectsOfType<HorizontalLayoutEditor>()) {
            edit.AddAllElements();
            edit.Reset();
        }
    }
}
