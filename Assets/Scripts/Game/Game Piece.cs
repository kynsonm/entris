using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Game;

[System.Serializable]
public class GamePiece
{
    public string name;
    public BlockColor color;
    public List<Vector2Int> coordinates;
    public bool canRotate = true;
    [Space(5f)]

    int numRotations = 0;
    List<Vector2Int> lastCenteredCoordinates = null;
    List<Vector2Int> lastPositiveCenteredCoordinates = null;
    Vector2 lastCenter = new Vector2(-999f, -999f);
    Vector2Int lastCenterBlock = new Vector2Int(-999, -999);
    int lastSize = -1;


    // ----- CENTERING -----

    // Central point in the block
    public Vector2 center() {
        if (lastCenter.x > -990f && lastCenter.y > -990f) { return lastCenter; }
        if (coordinates == null || coordinates.Count == 0) { return new Vector2(0f, 0f); }
        lastCenter = center(coordinates);
        return lastCenter;
    }
    Vector2 center(List<Vector2Int> coordinates) {
        if (coordinates == null || coordinates.Count == 0) { return new Vector2(0f, 0f); }
        Vector2 c = new Vector2();
        foreach (var coord in coordinates) {
            c.x += (float)coord.x + 1;
            c.y += (float)coord.y + 1;
        }
        c /= (float)coordinates.Count;
        c -= new Vector2(1f, 1f);
        return c;
    }

    // Get the center of mass of the piece, but the specific index/coordinate of it
    public Vector2Int centerBlock() {
        if (lastCenterBlock.x > 990 && lastCenterBlock.y > 990) { return lastCenterBlock; }
        Vector2 cent = center();
        lastCenterBlock = new Vector2Int(Mathf.RoundToInt(cent.x), Mathf.RoundToInt(cent.y));
        return lastCenterBlock;
    }

    // Get all of the coordinates but centered on the center
    public List<Vector2Int> centeredCoordinates() {
        return centeredCoordinates(false);
    }
    public List<Vector2Int> centeredCoordinates(bool makeAllYPositive) {
        if (makeAllYPositive && lastPositiveCenteredCoordinates != null) { return lastPositiveCenteredCoordinates; }
        if (!makeAllYPositive && lastCenteredCoordinates != null) { return lastCenteredCoordinates; }

        centerBlock();
        int minY = int.MaxValue;
        List<Vector2Int> centeredCoords = new List<Vector2Int>();
        foreach (var coord in coordinates) {
            Vector2Int newCoord = new Vector2Int(coord.x, coord.y);
            newCoord.x -= lastCenterBlock.x;
            newCoord.y -= lastCenterBlock.y;
            centeredCoords.Add(newCoord);

            if (newCoord.y < minY) { minY = newCoord.y; }
        }

        int offset = Mathf.Abs(minY);
        List<Vector2Int> posCenteredCoords = new List<Vector2Int>();
        foreach (Vector2Int coord in centeredCoords) {
            posCenteredCoords.Add(new Vector2Int(coord.x, coord.y + offset));
        }

        lastPositiveCenteredCoordinates = posCenteredCoords;
        lastCenteredCoordinates = centeredCoords;
        return makeAllYPositive ? lastPositiveCenteredCoordinates : lastCenteredCoordinates;
    }


    // ----- ROTATION -----

    // Rotate LEFT around the center block
    public List<Vector2Int> rotateLeft() {
        if (!canRotate) { return null; }
        --numRotations;
        checkNumRotations();
        return rotate();
    }

    // Rotate RIGHT around the center block
    public List<Vector2Int> rotateRight() {
        if (!canRotate) { return null; }
        ++numRotations;
        checkNumRotations();
        return rotate();
    }

    // Rotate all coordinates <numRotations> times
    List<Vector2Int> rotate() {
        centeredCoordinates();
        List<Vector2Int> newCoords = CopyCoordinates(lastCenteredCoordinates);
        for (int i = 0; i < numRotations; ++i) {
            newCoords = rotateOnce(newCoords);
        }
        return newCoords;
    }

    // Rotate all coordinates ONCE to the left
    List<Vector2Int> rotateOnce(List<Vector2Int> coordinates) {
        for (int i = 0; i < coordinates.Count; ++i) {
            Vector2Int coord = coordinates[i];
            int x = coord.x, y = coord.y;

            if (y == 0) {
                coordinates[i] = swap(coord);
                continue;
            }

            if (x == 0) {
                coordinates[i] = (y < 0) ? swap(abs(coord)) : swap(neg(coord));
            }
            else if (x < 0) {
                coordinates[i] = (y < 0) ? negX(coord) : negY(coord);
            }
            else if (x > 0) {
                coordinates[i] = (y < 0) ? negY(coord) : negX(coord);
            }
        }
        return coordinates;
    }

    void checkNumRotations() {
        if (numRotations < 0)  { numRotations = 3; }
        if (numRotations >= 4) { numRotations = 0; }
        return;
    }


    // ----- UTILITIES -----

    // Get the estimated size of the block (on its highest dimensions)
    public int size() {
        if (lastSize != -1) { return lastSize; }
        centeredCoordinates();
        int maxX = -999, maxY = -999;
        foreach (var coord in lastCenteredCoordinates) {
            if (coord.x > maxX) { maxX = coord.x; }
            if (coord.y > maxY) { maxY = coord.y; }
        }
        if (maxX < -990 || maxY < -990) { return 0; }
        lastSize = (maxX > maxY) ? maxX : maxY;
        return lastSize;
    }

    // Get the width/height of the block
    public int width() {
        int minX = int.MaxValue, maxX = int.MinValue;
        foreach (var coord in coordinates) {
            if (coord.x < minX) { minX = coord.x; }
            if (coord.x > maxX) { maxX = coord.x; }
        }
        return maxX+1 - minX;
    }
    public int height() {
        int minY = int.MaxValue, maxY = int.MinValue;
        foreach (var coord in coordinates) {
            if (coord.y < minY) { minY = coord.y; }
            if (coord.y > maxY) { maxY = coord.y; }
        }
        return maxY+1 - minY;
    }

    // Copy the coordinates inputted to a new list
    public static List<Vector2Int> CopyCoordinates(List<Vector2Int> toCopy) {
        List<Vector2Int> newCoords = new List<Vector2Int>();
        foreach (var coord in toCopy) {
            newCoords.Add(new Vector2Int(coord.x, coord.y));
        }
        return newCoords;
    }
    public static GamePiece Copy(GamePiece toCopy) {
        return new GamePiece(toCopy);
    }

    // Constructors
    public GamePiece(GamePiece toCopy) {
        name = toCopy.name;
        color = toCopy.color;
        coordinates = new List<Vector2Int>();
        foreach (var coord in toCopy.coordinates) {
            coordinates.Add(new Vector2Int(coord.x, coord.y));
        }
        canRotate = toCopy.canRotate;
        numRotations = 0;
        center();
        centerBlock();
        centeredCoordinates();
    }
    public GamePiece(string name_in, BlockColor color_in, List<Vector2Int> coordinates_in, bool canRotate_in) {
        name = name_in;
        color = color_in;
        coordinates = coordinates_in;
        canRotate = canRotate_in;
        numRotations = 0;
        center();
        centerBlock();
        centeredCoordinates();
    }

    // String it out!
    public override string ToString() {
        string str = $"--- GamePiece: \"{name}\"\n";
        str += "--- Color: " + color.ToString() + "\n";
        str += "--- Pieces:\n";
        for (int i = 0; i < coordinates.Count; ++i) {
            str += $"{i+1} - ({coordinates[i].x}, {coordinates[i].y})\n";
        }
        str += "--- Centered Pieces:\n";
        for (int i = 0; i < centeredCoordinates().Count; ++i) {
            str += $"{i+1} - ({centeredCoordinates()[i].x}, {centeredCoordinates()[i].y})\n";
        }
        str += "--- Positive Centered Pieces:\n";
        for (int i = 0; i < centeredCoordinates(true).Count; ++i) {
            str += $"{i+1} - ({centeredCoordinates(true)[i].x}, {centeredCoordinates(true)[i].y})\n";
        }
        return str;
    }

    // ----- SWAP STUFF -----
    Vector2Int negX(Vector2Int coord) {
        coord.x *= -1;
        return coord;
    }
    Vector2Int negY(Vector2Int coord) {
        coord.y *= -1;
        return coord;
    }
    Vector2Int swap(Vector2Int coord) {
        int temp = coord.x;
        coord.x = coord.y;
        coord.y = temp;
        return coord;
    }
    Vector2Int neg(Vector2Int coord) {
        return negX(negY(coord));
    }
    Vector2Int abs(Vector2Int coord) {
        return new Vector2Int(Mathf.Abs(coord.x), Mathf.Abs(coord.y));
    }
}