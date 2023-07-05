using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEngine.UI;
using Board = System.Collections.Generic.List<System.Collections.Generic.List<Game.Spot>>;

public class GameManager : MonoBehaviour
{
    [Header("---  Board Objects  ---")]
    [SerializeField] Transform boardHolder;
    [SerializeField] GameObject boardSpotPrefab;
    [SerializeField] GridLayoutEditor gridLayoutEditor;

    [Header("---  Block Areas   ---")]
    [SerializeField] Transform heldBlocksParent;
    [SerializeField] Transform nextBlocksParent;

    [Header("---  Game Options  ---")]
    [SerializeField] [Min(0.1f)] float speedingUpTimestepMultiplier;
    [SerializeField] float timeToMoveBeforeLocking;
    [SerializeField] int timestepsBeforeSpawningBlock;
    [SerializeField] [Min(0.1f)] float timestepMultiplier;
    [SerializeField] int bucketSizeMultiplier;

    // Managing the game
    Board board;
    [HideInInspector] public bool isPlaying;

    // Pieces
    List<Spot> movingSpots;
    GamePiece pieceSpawning, currentPiece;
    Vector2Int coordinatesOfPiece;
    int stepsUntilPieceSpawns;
    int yLevelToSpawn;

    // Held pieces
    List<int> pieceBucket;
    List<GamePiece> queue, held;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        CreateBoard();
    }

    // Update is called once per frame
    void Update() {
        CheckInputs();
    }

    void CheckInputs() {
        if (!isPlaying) { return; }
        if (InputCodes.left()) {
            MoveBlocksSideways(true);
            return;
        }
        if (InputCodes.right()) {
            MoveBlocksSideways(false);
            return;
        }
        if (InputCodes.rotateL()) {
            RotateBlock(true);
            return;
        }
        if (InputCodes.rotateR()) {
            RotateBlock(false);
            return;
        }
        if (InputCodes.speedUp()) {
            SpeedUp();
            return;
        }
        if (InputCodes.place()) {
            PlaceBlock();
            return;
        }
    }


    // ----- PAUSING AND PLAYING -----

    // Starts the game
    public void StartGame() {
        Debug.Log("Starting Game");
        Clear();
        Resume();
    }
    // Pauses the game
    public void Pause() {
        Debug.Log("Pausing Game");
        isPlaying = false;
        StopAllCoroutines();
    }
    // Resumes the game
    public void Resume() {
        Debug.Log("Resuming Game");
        isPlaying = true;
        StopAllCoroutines();
        StartCoroutine(RunGame());
    }
    // Clear board and reset the game
    public void Reset() { Clear(); }
    public void Clear() {
        // Clear any leftover information
        movingSpots = null;
        currentPiece = null;
        pieceSpawning = null;
        yLevelToSpawn = 0;
        queue = new List<GamePiece>();
        held = new List<GamePiece>();

        // Clear the board
        GameInfo.Clear();
        foreach (var row in board) {
            foreach (var spot in row) {
                spot.SetBackground();
            }
        }
    }


    // ----- GAME MANAGEMENT -----

    // Timer for each step of the game
    IEnumerator RunGame() {
        Debug.Log("Running Game");
        while (true) {
            yield return new WaitForSeconds(CalculateTimestep());
            NextTimestep();
        }
    }

    // Does one timestep of the game
    void NextTimestep() {
        // Move the moving spots down
        if (movingSpots != null && movingSpots.Count > 0) {
            MoveBlocksDown();
            // See if the next segment needs to be spawned
            if (pieceSpawning != null) {
                SpawnNextSegment();
            }
        }
        // See if there needs to be a block spawned
        else {
            if (stepsUntilPieceSpawns <= 0) {
                SpawnNextPiece();
            } else {
                --stepsUntilPieceSpawns;
            }
        }
    }

    // Decrease the timestep while the speed up key is pressed
    void SpeedUp() {
        StopAllCoroutines();

        if (speedingUpTimestepMultiplier <= 0.1f) { speedingUpTimestepMultiplier = 0.1f; }
        timestepMultiplier *= speedingUpTimestepMultiplier;
        NextTimestep();

        StartCoroutine(RunGame());
        StartCoroutine(SpeedUpEnum());
    }
    IEnumerator SpeedUpEnum() {
        bool keepGoing = true;
        while (keepGoing) {
            yield return new WaitForEndOfFrame();
            keepGoing = InputCodes.holdDown();
        }
        timestepMultiplier /= speedingUpTimestepMultiplier;
    }


    // ----- BLOCK MOVEMENT -----

    // Given an offset on each dimension, check if it can move there
    bool BlockCanMove(int xOffset, int yOffset) {
        if (movingSpots == null) { return false; }
        for (int i = 0; i < movingSpots.Count; ++i) {
            int x = movingSpots[i].x + xOffset;
            int y = movingSpots[i].y + yOffset;
            if (x < 0 || y < 0 || x >= board.Count || y >= board[x].Count) { return false; }
            if (!board[x][y].isBackground() && !board[x][y].isMoving) { return false; }
        }
        return true;
    }
    bool BlockCanMove(List<Vector2Int> coordinates, int xOffset, int yOffset) {
        for (int i = 0; i < coordinates.Count; ++i) {
            int x = coordinates[i].x + xOffset;
            int y = coordinates[i].y + yOffset;
            if (x < 0 || y < 0 || x >= board.Count) {
                continue;
                //return false;
            }
            if (y >= board[x].Count) {
                continue;
                //return false;
            }
            if (!board[x][y].isBackground() && !board[x][y].isMoving) {
                return false;
            }
        }
        return true;
    }
    void MoveBlock(int xOffset, int yOffset) {
        BlockColor color = movingSpots[0].theme.color;
        List<Spot> newSpots = new List<Spot>();
        for (int i = 0; i < movingSpots.Count; ++i) {
            int x = movingSpots[i].x, newX = x + xOffset;
            int y = movingSpots[i].y, newY = y + yOffset;
            newSpots.Add(board[newX][newY]);
            board[x][y].SetBackground();
        }
        for (int i = 0; i < newSpots.Count; ++i) {
            newSpots[i].SetMoving(color);
        }
        movingSpots = newSpots;
    }
    List<Vector2Int> MoveCoordinates(List<Vector2Int> coordinates, int xOffset, int yOffset) {
        List<Vector2Int> newCoords = new List<Vector2Int>();
        for (int i = 0; i < coordinates.Count; ++i) {
            int x = coordinates[i].x + xOffset;
            int y = coordinates[i].y + yOffset;
            newCoords.Add(new Vector2Int(x, y));
        }
        return newCoords;
    }

    // Move all moving spots down one if possible
    void MoveBlocksDown() {
        // Check that the piece can move down
        if (!BlockCanMove(0, 1)) {
            // TODO: Add a timer before locking pieces down
            foreach (var spot in movingSpots) {
                spot.isMoving = false;
            }
            movingSpots = null;
            //currentPiece = null;
            return;
        }

        // Move the piece down
        MoveBlock(0, 1);
        ++coordinatesOfPiece.y;
    }

    // Move all moving spots left or right if possible
    public void MoveBlockLeft() {
        MoveBlocksSideways(true);
    }
    public void MoveBlockRight() {
        MoveBlocksSideways(false);
    }
    void MoveBlocksSideways(bool movedLeft) {
        // Check that the piece can move sideways
        int xOffset = movedLeft ? -1 : 1;
        if (movedLeft) {
            if (!BlockCanMove(xOffset, 0)) {
                Debug.Log("Block cannot move left");
                return;
            }
        } else {
            if (!BlockCanMove(xOffset, 0))  {
                Debug.Log("Block cannot move right");
                return;
            }
        }

        // Move the piece sideways
        coordinatesOfPiece.x += xOffset;
        MoveBlock(xOffset, 0);
    }

    // TODO: Implement this
    // Places the block at the bottom of the board at its current position
    public void PlaceBlock() {

    }

    // TODO: Refine this later
    // Calculate the time to wait before the next game update
    float CalculateTimestep() {
        if (GameInfo.timeStep < 1) { GameInfo.timeStep = 1; }
        if (timestepMultiplier <= 0.1f) { timestepMultiplier = 0.1f; }
        float time = 1f / (float)GameInfo.timeStep;
        time *= timestepMultiplier;
        return time;
    }


    // ----- BLOCK ROTATION -----

    public void RotateBlockLeft() {
        RotateBlock(true);
    }
    public void RotateBlockRight() {
        RotateBlock(false);
    }
    void RotateBlock(bool rotatedLeft) {
        if (!GameInfo.canRotate) { return; }
        if (coordinatesOfPiece.y + 1 < (int)GameInfo.type) {
            Debug.Log("y coordinate of the piece is too small to rotate");
            return;
        }

        // Calculate the offsets for each block relative to the center
        List<Vector2Int> newCoords;
        if (rotatedLeft) {
            newCoords = currentPiece.rotateLeft();
        } else {
            newCoords = currentPiece.rotateRight();
        }
        if (newCoords == null) {
            Debug.Log("Coordinate offsets are null");
            return;
        }

        // Find the new coordinates
        for (int i = 0; i < newCoords.Count; ++i) {
            newCoords[i] = new Vector2Int(coordinatesOfPiece.x + newCoords[i].x, coordinatesOfPiece.y + newCoords[i].y);
        }

        // Fix the coordinates of the piece
        // Loop x times (depending on game type)
        //    ex. If block rotation causes it to be 2 blocks out of bounds, etc
        for (int i = 0; i < (int)GameInfo.type; ++i) {
            newCoords = CheckAndFixRotation(newCoords, rotatedLeft);
        }
        if (newCoords == null) {
            Debug.Log("Fixed coordinates are null");
            return;
        }

        // Reset the old blocks
        BlockColor color = movingSpots[0].theme.color;
        foreach (Spot spot in movingSpots) {
            spot.SetBackground();
        }

        // Set the new blocks
        movingSpots = new List<Spot>();
        foreach (Vector2Int coord in newCoords) {
            board[coord.x][coord.y].SetMoving(color);
            movingSpots.Add(board[coord.x][coord.y]);
        }
    }

    // Check if the block is off bounds or intersecting other blocks
    // AND if that is fixable
    // Then, fix it once
    List<Vector2Int> CheckAndFixRotation(List<Vector2Int> coordinates, bool rotatedLeft) {
        // Check if the piece needs to be moved
        if (coordinates == null) { return null; }
        for (int i = 0; i < coordinates.Count; ++i) {
            int x = coordinates[i].x, y = coordinates[i].y;
            if (x < 0) {
                if (!BlockCanMove(coordinates, 1, 0)) {
                    coordinates = rotatedLeft ? currentPiece.rotateRight() : currentPiece.rotateLeft();
                    return null;
                }
                coordinates = MoveCoordinates(coordinates, 1, 0);
                ++coordinatesOfPiece.x;
            }
            if (x >= (int)GameInfo.width) {
                if (!BlockCanMove(coordinates, -1, 0)) {
                    coordinates = rotatedLeft ? currentPiece.rotateRight() : currentPiece.rotateLeft();
                    return null;
                }
                coordinates = MoveCoordinates(coordinates, -1, 0);
                --coordinatesOfPiece.x;
            }
            if (!FixOverlap(coordinates[i], coordinates, rotatedLeft)) {
                coordinates = rotatedLeft ? currentPiece.rotateRight() : currentPiece.rotateLeft();
                return null;
            }
        }
        return coordinates;
    }

    // TODO: FIX THIS!
    bool FixOverlap(Vector2Int coordinate, List<Vector2Int> coordinates, bool rotatedLeft) {
        bool overlap = false;
        int x = coordinate.x, y = coordinate.y;

        // Check if any overlapping blocks exist, and stop if not
        if (!board[x][y].isBackground() && !board[x][y].isMoving) {
            overlap = true;
        }
        //Debug.Log("There is an overlap? " + overlap.ToString());
        if (!overlap) { return true; }

        // Try moving the piece <size> blocks to the left and right
        for (int i = 0; i < currentPiece.size(); ++i) {
            if (BlockCanMove(coordinates, i, 0)) {
                coordinates = MoveCoordinates(coordinates, i, 0);
                Debug.Log("Piece can move " + i + " blocks left");
                return true;
            }
            else if (BlockCanMove(coordinates, -i, 0)) {
                coordinates = MoveCoordinates(coordinates, -i, 0);
                Debug.Log("Piece can move " + i + " blocks right");
                return true;
            }
            Debug.Log("Piece cannot move " + i + " blocks left or right");
        }
        // Otherwise, the overlap cannot be fixed
        Debug.Log("Piece cannot be moved to fix overlap");
        return false;
    }


    // ----- PIECE GENERATION -----

    // Spawns the next segment of the spawning piece
    void SpawnNextSegment() {
        if (pieceSpawning == null) { return; }
        
        // Spawn the segments
        int highestY = int.MinValue;
        foreach (var coord in pieceSpawning.centeredCoordinates(true)) {
            if (coord.y > highestY) { highestY = coord.y; }
            if (coord.y != yLevelToSpawn) { continue; }

            int x = coordinatesOfPiece.x + coord.x;
            board[x][0].SetMoving(pieceSpawning.color);

            Debug.Log("Spawning segment " + coord.ToString() + " of piece " + pieceSpawning.name + "\n--- piece spawning:\n" + pieceSpawning.ToString());

            movingSpots.Add(board[x][0]);
        }
        ++yLevelToSpawn;
        if (yLevelToSpawn <= highestY) { return; }

        // Stop spawning when done
        pieceSpawning = null;
        yLevelToSpawn = 0;
    }

    // Starts the spawning process for a piece
    void SpawnNextPiece() {
        // Get the next piece
        if (queue == null || queue.Count == 0) {
            pieceSpawning = NextPiece();
        } else {
            pieceSpawning = queue[0];
            queue.RemoveAt(0);
            QueuePiece();
        }

        Debug.Log("Spawning piece:\n" + pieceSpawning.ToString());

        // Setup info
        currentPiece = new GamePiece(pieceSpawning);
        movingSpots = new List<Spot>();
        yLevelToSpawn = 0;
        coordinatesOfPiece.x = (int)GameInfo.width / 2;
        coordinatesOfPiece.y = 0;
        stepsUntilPieceSpawns = timestepsBeforeSpawningBlock;

        SpawnNextSegment();
    }

    // Get the next piece to spawn and add it to the queue
    void QueuePiece() {
        if (queue == null) { queue = new List<GamePiece>(); }
        queue.Add(NextPiece());
    }
    // Finds/resets the bucket and retrieves a random index to the GameInfo's pieces
    GamePiece NextPiece() {
        // Reset the bucket
        if (pieceBucket == null || pieceBucket.Count == 0) {
            pieceBucket = new List<int>();
            bucketSizeMultiplier = bucketSizeMultiplier <= 0 ? 1 : bucketSizeMultiplier;
            for (int i = 0; i < bucketSizeMultiplier; ++i) {
                for (int j = 0; j < GameInfo.pieces.Count; ++j) {
                    pieceBucket.Add(j);
                }
            }
        }
        // Get a random piece index
        int bucketIndex = Random.Range(0, pieceBucket.Count);
        int pieceIndex = pieceBucket[bucketIndex];
        pieceBucket.RemoveAt(bucketIndex);
        return new GamePiece(GameInfo.pieces[pieceIndex]);
    }


    // ----- GAME CREATION -----

    // Remove existing board
    public void DestroyBoard() {
        foreach (Transform child in boardHolder) {
            GameObject.Destroy(child.gameObject);
        }
        if (board == null) { return; }
        board.Clear();
        board = null;
    }
    // Create a board
    public void CreateBoard() {
        // Check if objects are good to actually create a board
        if (!OkToMakeBoard()) { return; }
        DestroyBoard();

        // Make sure a game is selected
        if (!GameInfo.GameIsSelected()) { GameInfo.SetDefaultGame(); }

        // How large to make the board
        int width, height;
        if (GameInfo.type == GameType.custom) {
            width = GameInfo.customWidth;
            height = GameInfo.customHeight;
        } else {
            width = (int)GameInfo.width;
            height = (int)GameInfo.height;
        }

        // Setup the grid layout group
        gridLayoutEditor.constraintType = GridLayoutEditor.ConstraintType.columnCount;
        gridLayoutEditor.constraintCount = width;
        gridLayoutEditor.Reset();

        // Create the board
        board = new Board();

        for (int x = 0; x < width; ++x) {
            List<Spot> row = new List<Spot>();
            for (int y = 0; y < height; ++y) {
                GameObject obj = Instantiate(boardSpotPrefab, boardHolder);
                row.Add(new Spot(obj, x, y));
            }
            board.Add(row);
        }
        
        // Set the size of the game board
        RectTransform boardRect = boardHolder.GetComponent<RectTransform>();
        RectTransform parRect = boardHolder.parent.GetComponent<RectTransform>();
        GridLayoutGroup gridLayout = boardHolder.GetComponent<GridLayoutGroup>();
        float size = (float)height * gridLayout.cellSize.y;
        float bottom = size - parRect.rect.height;
        RectTransformOffset.Bottom(boardRect, -bottom);

        Debug.Log("HEIGHT OF THE GAME BOARD IS " + size + " FROM gameHeight = " + height + " and cell height = " + gridLayout.cellSize.y);

    }
    bool OkToMakeBoard() {
        bool allGood = true;
        if (boardHolder == null) {
            Debug.LogError("GameManager: BoardHolder is null. Cannot create a board");
            allGood = false;
        }
        if (boardSpotPrefab == null) {
            Debug.LogError("GameManager: Board spot prefab is null. Cannot create a board");
            allGood = false;
        }
        if (gridLayoutEditor == null) {
            Debug.LogError("GameManager: GridLayoutGroup editor is null. Cannot create a board");
            allGood = false;
        }
        if (heldBlocksParent == null) {
            Debug.LogWarning("GameManager: Held blocks object is null. Creating board but cannot add blocks to this");
        }
        if (nextBlocksParent == null) {
            Debug.LogWarning("GameManager: Next blocks object is null. Creating board but cannot add blocks to this");
        }
        return allGood;
    }
    // ----- END GAME CREATION -----
}
