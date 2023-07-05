using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;

public class PieceDisplayManager : MonoBehaviour
{
    [SerializeField] GameObject pieceDisplayPrefab;

    // Return the prefab
    public GameObject GetPrefab() {
        if (!CheckPrefab()) {
            Debug.Log("PieceDisplayManager: Cannot create a GamePiece Spot, returning null");
            return null;
        }
        return pieceDisplayPrefab;
    }

    // Make sure the prefab is good
    bool CheckPrefab() {
        Spot spot = new Spot(pieceDisplayPrefab, 0, 0);
        if (!spot.CheckObjects()) { return false; }
        return true;
    }
}
