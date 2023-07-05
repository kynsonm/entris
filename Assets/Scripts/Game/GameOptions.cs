using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class GameOptions : MonoBehaviour
{
    [System.Serializable]
    public class GameOption {
        public string name;
        [TextArea(2, 5)] public string description;
        [Space(5f)]
        public BoardWidth width;
        public BoardHeight height;
        public GameType type;
        [Space(5f)]
        public bool canRotate;
        public bool canFlip;
        [Space(10f)]
        public List<GamePiece> pieces;
    }

    public List<GameOption> gameOptions;
    int selectedGameIndex;

    public void Set(int index) { SelectGame(index); }
    public void Set() { SelectGame(selectedGameIndex); }
    public void SelectGame(int index) {
        if (index >= gameOptions.Count) { index = 0; }
        if (index < 0) { index = gameOptions.Count - 1; }
        selectedGameIndex = index;
        GameInfo.Set(gameOptions[selectedGameIndex]);
    }
    public void NextGame() { SelectGame(++selectedGameIndex); }
    public void PreviousGame() { SelectGame(--selectedGameIndex); }
}
