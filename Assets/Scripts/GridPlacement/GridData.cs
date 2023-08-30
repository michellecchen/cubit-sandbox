using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{

    // Dictionary containing data for all objects currently placed on the grid
    // - keys: cell positions
    // - values: placement data for that cell (is it currently occupied by an object?)
    Dictionary<Vector3Int, ObjectPlacement> placementDict = new();

    // Class methods

    // Attempt to place an object at a specified target position in the grid
    // CALLER: PlacementManager
    // RETURNS: Data of placed object
    public ObjectPlacement AddObjectAt(Vector3Int targetGridPos, Vector3Int objectSize, int objectID, int objectIndex) {
        
        // Determine which cells would be occupied by this object, were it to be placed
        List<Vector3Int> occupiedCells = CalculateOccupiedCells(targetGridPos, objectSize);
        ObjectPlacement placedObjectData = new ObjectPlacement(occupiedCells, objectID, objectIndex);
        
        // Check if the targeted placement for this object is "legal" (all relevant cells are currently empty/unoccupied)
        foreach (var pos in occupiedCells) {
            
            // If "illegal," then log a warning
            if (placementDict.ContainsKey(pos)) {
                Debug.Log("PLACEMENT FAILED: One of the cells targeted by this object's placement is already occupied.");
                return null;
            }
            
            // If "legal," register this newly placed object in the dictionary through which we're tracking all placements
            placementDict[pos] = placedObjectData;
        }

        return placedObjectData;

    }

    // Return ID of the removed object
    public int RemoveObjectAt(Vector3Int gridPos) {

        // Remove all keys representing this object from dictionary (AKA all cells the object currently occupies)
        int removedObjectID = placementDict[gridPos].objectID;
        List<Vector3Int> occupiedCells = placementDict[gridPos].occupiedPositions;

        foreach (var cellPos in occupiedCells) {
            placementDict.Remove(cellPos);
        }

        return removedObjectID;
    }

    public bool LegalPlacementAt(Vector3Int targetGridPos, Vector3Int objectSize) {
        
        List<Vector3Int> occupiedCells = CalculateOccupiedCells(targetGridPos, objectSize);
        
        foreach (var cellPos in occupiedCells) {
            if (placementDict.ContainsKey(cellPos)) {           // Illegal: found one conflicting/already-occupied cell
                return false;
            }
        }
        return true;                                            // Legal
    }

    // Helper method for 'LegalPlacementAt(...)'
    // Determines which cell positions would be occupied if this object were to be placed in this proposed position
    private List<Vector3Int> CalculateOccupiedCells(Vector3Int targetGridPos, Vector3Int objectSize) {

        List<Vector3Int> occupiedCells = new();

        for (int x = 0; x < objectSize.x; x++) {
            for (int y = 0; y < objectSize.y; y++) {
                for (int z = 0; z < objectSize.z; z++) {                
                    occupiedCells.Add(targetGridPos + new Vector3Int(x, y, z));
                }
            }
        }
        return occupiedCells;
    }

    public int GetObjectIndexAt(Vector3Int gridPos) {
        if (!placementDict.ContainsKey(gridPos)) {              // If there's no object at this position in the grid, return -1
            return -1;
        }
        return placementDict[gridPos].placedObjectIndex;        // Otherwise, return the index of the object
    }

    // CALLER: PlacementManager
    public int GetObjectIDAt(Vector3Int gridPos) {
        if (!placementDict.ContainsKey(gridPos)) {              // If there's no object occupying this grid position, return -1
            return -1;
        }
        return placementDict[gridPos].objectID;                 // Otherwise, return the ID of the object
    }
}

// Class for storing placement data for an individual OBJECT
public class ObjectPlacement {

    // Cell positions occupied by this object in its placement
    public List<Vector3Int> occupiedPositions;

    // ID of the object; helpful for future saving/loading of data
    public int objectID { get; private set; }

    // Index of the object
    public int placedObjectIndex { get; private set; }

    // Constructor
    public ObjectPlacement(List<Vector3Int> occupiedPositions, int dataID, int placedObjIndex) {
        this.occupiedPositions = occupiedPositions;
        objectID = dataID;
        placedObjectIndex = placedObjIndex;
        
    }
}
