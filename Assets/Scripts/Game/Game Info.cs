using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game {

public enum BoardWidth {
    very_small = 5, small = 10, medium = 15, large = 20, very_large = 25, massive = 30,
    custom = 0,
    invalid = -1
}
public enum BoardHeight {
    very_small = 10, small = 20, medium = 30, large = 40, very_large = 50, massive = 60,
    custom = 0,
    invalid = -1
}

public enum GameType {
    tritris = 3, tetris = 4, pentris = 5, sextris = 6, septris = 7, octris = 8, nontris = 9,
    custom = 0,
    invalid = -1
}

public static class GameInfo
{
    // Identifiers
    public static string name;
    public static string description;

    // Board info
    public static BoardWidth width;
    public static BoardHeight height;
    public static GameType type;
    public static List<GamePiece> pieces;

    public static int customWidth, customHeight;

    // Options
    public static bool canRotate;
    public static bool canFlip;

    // Time
    public static float time;
    public static int timeStep;

    // Stats
    public static int score;
    public static int level;
    public static int lines;

    public static void Set(GameOptions.GameOption gameOption) {
        Reset();

        name = gameOption.name;
        description = gameOption.description;

        width = gameOption.width;
        height = gameOption.height;
        type = gameOption.type;
        pieces = copyPieces(gameOption.pieces);

        canRotate = gameOption.canRotate;
        canFlip = gameOption.canFlip;
    }

    public static void Clear() {
        time = 0f;
        timeStep = 0;
        score = 0;
        level = 0;
        lines = 0;
    }

    public static void Reset() {
        name = "No game selected";
        description = "N/A";

        width = BoardWidth.invalid;
        height = BoardHeight.invalid;
        type = GameType.invalid;
        pieces = null;

        canRotate = false;
        canFlip = false;

        time = 0f;
        timeStep = 0;

        score = 0;
        level = 0;
        lines = 0;
    }

    public static void SetDefaultGame() {
        Reset();
        //DefaultTritris();
        DefaultTetris();
        //DefaultPentris();
    }
    static void DefaultTetris() {
        name = "Default Game (Tetronimo)";
        description = "Default game description";
        width = BoardWidth.small;
        height = BoardHeight.small;
        type = GameType.tetris;
        timeStep = 1;
        canRotate = true;

        pieces = new List<GamePiece>();
        pieces.Add( new GamePiece("I", BlockColor._1, new List<Vector2Int>{new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0)}, true) );
        pieces.Add( new GamePiece("J", BlockColor._2, new List<Vector2Int>{new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(2, 0)}, true) );
        pieces.Add( new GamePiece("L", BlockColor._3, new List<Vector2Int>{new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1)}, true) );
        pieces.Add( new GamePiece("O", BlockColor._4, new List<Vector2Int>{new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(1, 1), new Vector2Int(2, 1)}, false) );
        pieces.Add( new GamePiece("S", BlockColor._5, new List<Vector2Int>{new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 1)}, true) );
        pieces.Add( new GamePiece("T", BlockColor._6, new List<Vector2Int>{new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(1, 1)}, true) );
        pieces.Add( new GamePiece("Z", BlockColor._7, new List<Vector2Int>{new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(2, 0)}, true) );
    }
    static void DefaultPentris() {
        GameOptions gameOptions = GameObject.FindObjectOfType<GameOptions>();
        if (gameOptions == null) {
            Debug.Log("No GameOptions found. Setting default game to Tetris");
            DefaultTetris();
            return;
        }

        foreach (var option in gameOptions.gameOptions) {
            if (option.type == GameType.pentris) {
                Debug.Log("Setting default game to Pentris");
                Set(option);
                return;
            }
        }

        Debug.Log("Could not find Pentris game on GameOptions. Setting default game to Tetris");
        DefaultTetris();
    }
    static void DefaultTritris() {
        GameOptions gameOptions = GameObject.FindObjectOfType<GameOptions>();
        if (gameOptions == null) {
            Debug.Log("No GameOptions found. Setting default game to Tetris");
            DefaultTetris();
            return;
        }

        foreach (var option in gameOptions.gameOptions) {
            if (option.type == GameType.tritris) {
                Debug.Log("Setting default game to Tritris");
                Set(option);
                return;
            }
        }

        Debug.Log("Could not find Pentris game on GameOptions. Setting default game to Tetris");
        DefaultTetris();
    }


    public static bool GameIsSelected() {
        bool notSelected = false;

        notSelected = notSelected || name == "No game selected";
        notSelected = notSelected || description == "N/A";

        notSelected = notSelected || width == BoardWidth.invalid;
        notSelected = notSelected || height == BoardHeight.invalid;
        notSelected = notSelected || type == GameType.invalid;
        notSelected = notSelected || pieces == null || pieces.Count == 0;

        return !notSelected;
    }

    static List<GamePiece> copyPieces(List<GamePiece> toCopy) {
        List<GamePiece> pieces = new List<GamePiece>();
        for (int i = 0; i < toCopy.Count; ++i) {
            GamePiece piece = new GamePiece(toCopy[i]);
            pieces.Add(piece);
        }
        return pieces;
    }
}}