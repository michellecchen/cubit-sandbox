using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to every placed object
public class ObjectTerrain : MonoBehaviour
{

    // Dictionary of inactive terrain cells for this object
    //      - keys: Index of the neighboring object in placedObjects list
    //          * (neighboring = has a cubit that neighbors this cubit & thus prohibits placement on a terrain cell on the overlapping face)
    //      - values: List of this object's terrain cells that have been deactivated as a result of this neighboring object
    Dictionary<int, List<GameObject>> inactiveCells = new();

    // Set when object is placed in PlaceObject
    public int placedObjectIndex = -1;

    // Allows us to specifically check for collisions with active terrain cells of placed objects
    // (whose ObjectTerrains have already been set up)
    private int terrainLayerNum = 7;
    [SerializeField]
    private LayerMask terrainLayerMask;

    void Start()
    {
        // Get LayerMask via bitwise left shift operation
        terrainLayerMask = 1 << terrainLayerNum;
    }

    // Create terrain for this object
    //      1. Activate all terrain cells
    //      2. Respond to collisions detected btwn. this object's terrain cells & neighboring objects' terrain cells
    //          - Store neighbor information (incl. affected cells) in inactiveCells
    //          - Deactivative affected cells
    public void CreateObjectTerrain(int objectIndex)
    {
        placedObjectIndex = objectIndex;
        // CheckForCollisions();
        foreach (Transform childCell in transform) {
            childCell.gameObject.layer = terrainLayerNum;
        }

    }

    // NOTE: Currently not in use.
    private void CheckForCollisions() {
        foreach (ObjectTerrainCell childCell in GetComponentsInChildren<ObjectTerrainCell>()) {
            int neighborIndex = childCell.CheckForCollisions();
            if (neighborIndex != -1) {
                Debug.Log("detected collision with " + neighborIndex.ToString());
                inactiveCells[neighborIndex].Add(childCell.gameObject);
                childCell.gameObject.SetActive(false);
            } else {
                // no collisions detected
                childCell.SetToTerrainLayer();
            }
        }
    }
}
