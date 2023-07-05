using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOption : MonoBehaviour
{
    [HideInInspector] public GameOptions.GameOption gameOption;

    TMP_Text titleTMP, infoTMP;
    Transform blockHolder;

    // Start is called before the first frame update
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        Reset();
    }

    // Reset the whole GameOption
    void Reset() {
        if (!CheckObjects()) { return; }
        titleTMP.text = gameOption.name;
        infoTMP.text = gameOption.description;
        CreatePieces();
    }

    // Create each piece
    void CreatePieces() {
        // Destroy whats already in it
        foreach (Transform child in blockHolder) {
            GameObject.Destroy(child.gameObject);
        }

        // Create each area
        pieceDisplays = new List<PieceDisplay>();
        foreach (GamePiece piece in gameOption.pieces) {
            GameObject baseObj = new GameObject();

            GameObject obj = Instantiate(baseObj, blockHolder);
            obj.name = piece.name + " Piece Display";
            PieceDisplay display = obj.AddComponent<PieceDisplay>();
            pieceDisplays.Add(display);
            display.useCustomGamePiece = true;
            display.customGamePiece = new GamePiece(piece);

            GameObject.Destroy(baseObj);
        }

        StartCoroutine(MakeBlocksTheSameSize());
    }

    List<PieceDisplay> pieceDisplays;
    IEnumerator MakeBlocksTheSameSize() {
        yield return new WaitForEndOfFrame();
        foreach (PieceDisplay disp in pieceDisplays) {
            disp.pieceSizeMultiplier = 1f;
            disp.SetupGridEditor();
        }

        yield return new WaitForEndOfFrame();

        float minCellSize = float.PositiveInfinity;
        foreach (PieceDisplay disp in pieceDisplays) {
            if (disp.grid.cellSize.x < minCellSize) { minCellSize = disp.grid.cellSize.x; }
        }

        foreach (PieceDisplay disp in pieceDisplays) {
            float multiplier = minCellSize / disp.grid.cellSize.x;
            disp.pieceSizeMultiplier = multiplier;
            disp.SetupGridEditor();

            Debug.Log("Setting cellSizeMultiplier on gridEditor " + disp.gameObject.name + " to " + multiplier);
        }

        Debug.Log("MinCellSize was: " + minCellSize);
    }

    bool CheckObjects() {
        bool allGood = true;
        if (titleTMP == null) {
            titleTMP = transform.Find("Title").GetComponentInChildren<TMP_Text>();
            if (titleTMP == null) {
                Debug.Log("GameOption: No Title TMP found on " + gameObject.name);
                allGood = false;
            }
        }
        if (infoTMP == null) {
            infoTMP = transform.Find("Info").GetComponentInChildren<TMP_Text>();
            if (infoTMP == null) {
                Debug.Log("GameOption: No Info TMP found on " + gameObject.name);
                allGood = false;
            }
        }
        if (blockHolder == null) {
            blockHolder = transform.Find("Blocks").Find("Content");
            if (blockHolder == null) {
                Debug.Log("GameOption: No block holder found on " + gameObject.name);
                allGood = false;
            }
        }
        return allGood;
    }
}
