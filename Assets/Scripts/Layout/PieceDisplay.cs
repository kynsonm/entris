using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;

public class PieceDisplay : MonoBehaviour
{
    // Options
    [Range(0.05f, 1f)] public float pieceSizeMultiplier = 1f;
    float lastPieceSizeMultiplier;
    [Header("Select piece indices from GameOptions script")]
    [SerializeField] int gameOptionIndex = 0;
    [SerializeField] int gamePieceIndex = 0;
    [Header("Or, use a custom GamePiece")]
    public bool useCustomGamePiece = false;
    public GamePiece customGamePiece = null;

    // Piece objects
    List<List<Spot>> spots;

    // Pieces
    GamePiece gamePiece = null;
    List<Vector2Int> coordinates = null;
    List<Vector2Int> lastCoordinates = null;
    GameOptions gameOptions;
    List<GamePiece> gamePieces = null;
    PieceDisplayManager displayManager;

    // Layout stuff
    [HideInInspector] public GridLayoutGroup grid;
    GridLayoutEditor gridEditor;
    RectTransform rect;
    Vector2 lastRectSize;


    // Start is called before the first frame update
    void OnEnable() { StartCoroutine(Start()); }
    IEnumerator Start() {
        yield return new WaitForEndOfFrame();
        Reset();
    }


    // Create the piece and reset variables
    public void Reset() {
        if (!CheckObjects()) { return; }
        if (!PiecesHaveChanged()) { return; }

        // Destroy whats already there
        foreach (Transform child in grid.gameObject.transform) {
            GameObject.Destroy(child.gameObject);
        }

        SetupGridEditor();
        CreatePieces();

        StopAllCoroutines();
        StartCoroutine(CheckSizeEnum());
    }

    void CreatePieces() {
        int width = gamePiece.width(), height = gamePiece.height();

        // Fill the spots to the correct size (So no oob indexes)
        spots = new List<List<Spot>>();
        for (int x = 0; x < width; ++x) {
            List<Spot> row = new List<Spot>();
            for (int y = 0; y < height; ++y) {
                row.Add(null);
            }
            spots.Add(row);
        }

        // Create each spot display
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                CreateSpot(x, y);
            }
        }
    }
    void CreateSpot(int x, int y) {
        // No piece to create
        if (!coordinates.Contains(new Vector2Int(x, y))) {
            GameObject objBase = new GameObject();
            GameObject blankObj = Instantiate(objBase, grid.gameObject.transform);
            blankObj.name = $"({x}, {y}) - Blank";
            GameObject.Destroy(objBase);
            return;
        }
        // Create the spot
        GameObject obj = Instantiate(displayManager.GetPrefab(), grid.gameObject.transform);
        obj.name = $"({x}, {y}) - Spot";
        Spot spot = new Spot(obj, x, y);
        spots[x][y] = spot;
        spot.Set(gamePiece.color);
    }

    public void SetupGridEditor() {
        grid.startAxis = GridLayoutGroup.Axis.Vertical;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.startCorner = GridLayoutGroup.Corner.UpperRight;

        gridEditor.constraintType = GridLayoutEditor.ConstraintType.columnCount;
        gridEditor.constraintCount = gamePiece.width();
        gridEditor.dontUpdateConstraintCount = false;
        gridEditor.cellSizeXMultiplier = pieceSizeMultiplier;
        gridEditor.cellSizeYMultiplier = pieceSizeMultiplier;
        gridEditor.Reset();
    }


    // ----- UTILITIES -----

    // Check if coordinates have changed
    bool PiecesHaveChanged() {
        if (spots == null || spots.Count == 0) { return true; }
        if (grid.gameObject.transform.childCount == 0) { return true; }
        if (grid.gameObject.transform.childCount != coordinates.Count) { return true; }

        if (coordinates == null || lastCoordinates == null) { return false; }
        if (coordinates.Count == 0 || lastCoordinates.Count == 0) { return false; }
        if (coordinates.Count != lastCoordinates.Count) { return true; }

        for (int i = 0; i < coordinates.Count; ++i) {
            if (coordinates[i].x != lastCoordinates[i].x) { return true; }
            if (coordinates[i].y != lastCoordinates[i].y) { return true; }
        }
        return false;
    }

    // Check if the size of the grid has changed
    IEnumerator CheckSizeEnum() {
        float interval = 0.333f;
        while (true) {
            yield return new WaitForSeconds(interval);
            if (SizeHasChanged()) { SetupGridEditor(); }
        }
    }
    bool SizeHasChanged() {
        if (rect.rect.width != lastRectSize.x || rect.rect.height != lastRectSize.y) {
            lastRectSize = new Vector2(rect.rect.width, rect.rect.height);
            return true;
        }
        if (pieceSizeMultiplier != lastPieceSizeMultiplier) {
            lastPieceSizeMultiplier = pieceSizeMultiplier;
            return true;
        }
        return false;
    }

    // Get relevant variables from serialized options, or return false if something goes wrong
    bool CheckObjects() {
        // Check PieceDisplay manager
        if (displayManager == null) {
            displayManager = GameObject.FindObjectOfType<PieceDisplayManager>();
            if (displayManager == null) {
                Debug.LogError("PieceDisplay: PieceDisplayManager is null");
                return false;
            }
        }

        // Check for gameOptions
        if (gameOptions == null && !useCustomGamePiece) {
            gameOptions = GameObject.FindObjectOfType<GameOptions>();
            if (gameOptions == null) {
                Debug.LogError("PieceDisplay: No GameOptions object in the scene");
                return false;
            }
        }

        // Make sure objects are good and set variables from them
        if (!useCustomGamePiece) {
            if (gameOptionIndex < 0 || gameOptionIndex >= gameOptions.gameOptions.Count) {
                Debug.LogError($"PieceDisplay: GameOption index is out of range -- Index = {gameOptionIndex} and Size = {gameOptions.gameOptions.Count}");
                return false;
            }
            gamePieces = gameOptions.gameOptions[gameOptionIndex].pieces;
            if (gamePieceIndex < 0 || gamePieceIndex >= gamePieces.Count) {
                Debug.LogError($"PieceDisplay: GamePiece index is out of range -- Index = {gamePieceIndex} and Size = {gamePieces.Count}");
                return false;
            }
            gamePiece = gamePieces[gamePieceIndex];
        } else {
            if (customGamePiece == null || customGamePiece.coordinates == null || customGamePiece.coordinates.Count == 0) {
                Debug.LogError("PieceDisplay: Custom game piece is not set up");
                return false;
            }
            gamePiece = customGamePiece;
        }
        // Check coordinates
        if (gamePiece == null || gamePiece.coordinates == null || gamePiece.coordinates.Count == 0) {
            Debug.LogError("PieceDisplay: GamePiece is not set up");
            return false;
        }
        coordinates = GamePiece.CopyCoordinates(gamePiece.coordinates);
        gamePiece.centeredCoordinates();
        lastCoordinates = GamePiece.CopyCoordinates(coordinates);

        // Add layout stuff
        if (grid == null) {
            grid = gameObject.GetComponent<GridLayoutGroup>();
            if (grid == null) {
                grid = gameObject.GetComponentInChildren<GridLayoutGroup>();
            }
            if (grid == null) {
                grid = gameObject.AddComponent<GridLayoutGroup>();
            }
        }
        if (gridEditor == null) {
            gridEditor = gameObject.GetComponent<GridLayoutEditor>();
            if (gridEditor == null) {
                gridEditor = gameObject.GetComponentInChildren<GridLayoutEditor>();
            }
            if (gridEditor == null) {
                gridEditor = gameObject.AddComponent<GridLayoutEditor>();
            }
        }
        rect = grid.gameObject.GetComponent<RectTransform>();
        lastRectSize = new Vector2(rect.rect.width, rect.rect.height);

        return true;
    }
}
