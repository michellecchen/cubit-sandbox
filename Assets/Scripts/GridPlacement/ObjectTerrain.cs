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

        // // Everything starts as inactive
        // foreach (Transform childCell in transform) {
        //     childCell.gameObject.SetActive(false);
        // }
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
        // for now just automatically set to terrain layer
        foreach (Transform childCell in transform) {
            childCell.gameObject.layer = terrainLayerNum;
        }

    }

    private void CheckForCollisions() {
        foreach (ObjectTerrainCell childCell in GetComponentsInChildren<ObjectTerrainCell>()) {
            // childCell.gameObject.GetComponent<ObjectTerrainCell>().checkingForCollisions = true;
            // childCell.checkingForCollisions = true;
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

    // public void HandleCollision(GameObject childCell, GameObject otherCell) {
    //     int neighborIndex = otherCell.transform.parent.gameObject.GetComponent<ObjectTerrain>().placedObjectIndex;
    //     Debug.Log("Neighbor index: " + neighborIndex.ToString());
    //     childCell.gameObject.GetComponent<ObjectTerrainCell>().checkingForCollisions = false;
    // }

    //             // GameObject neighbor = neighborCell.transform.parent.gameObject;
    //             // int neighborIndex = neighbor.GetComponent<ObjectTerrain>().placedObjectIndex;
    //             Debug.Log("Neighbor index: " + neighborIndex.ToString());

    //             if (neighborIndex != -1)
    //             {

    //                 // if (inactiveCells.ContainsKey(neighborIndex))
    //                 // {
    //                 //     Debug.Log("Existing neighbor found");
    //                 // }
    //                 // else
    //                 // {
    //                 //     Debug.Log("New neighbor registered");
    //                 // }

    //                 inactiveCells[neighborIndex].Add(childCell);
    //                 childCell.SetActive(false);
    //             }
    //             else
    //             {
    //                 Debug.Log("ERROR: Invalid neighboring object index.");
    //             }


    //         }

    //         // no collisions detected -> keep cell active, change to non-trigger, move to 'terrain'
    //         else
    //         {
    //             childCollider.isTrigger = false;
    //             childCell.layer = terrainLayerNum;
    //         }
    // }

    // private Collider[] GetNeighboringColliders(BoxCollider collider) {
    //     Collider[] neighboringColliders = Physics.OverlapBox(
    //         center: collider.transform.position + (collider.transform.rotation * collider.center),
    //         halfExtents: Vector3.Scale(collider.size * 0.5f, collider.transform.lossyScale),
    //         orientation: collider.transform.rotation,
    //         layerMask: terrainLayerMask
    //     );
    //     Debug.Log($"{neighboringColliders.Length} colliders detected");
    //     return neighboringColliders;
    // }

    // // Check all terrain cells for collisions with neighboring cells
    // public void CheckForCollisions()
    // {

    //     BoxCollider[] childColliders = transform.GetComponentsInChildren<BoxCollider>();
    //     Debug.Log("Number of child colliders: " + childColliders.Length);
    //     foreach (BoxCollider childCollider in childColliders)
    //     {
    //         // Get all overlapping colliders from neighbors
    //         // Collider[] neighborColliders = Physics.OverlapBox(childCollider.bounds.center, childCollider.bounds.extents, childCollider.transform.rotation, terrainLayerMask);
    //         Collider[] neighborColliders = GetNeighboringColliders(childCollider);

    //         // (there should only be one?)
    //         // Debug.Log("Number of neighbor colliders detected: " + neighborColliders.Length);

    //         GameObject childCell = childCollider.gameObject;

    //         // collision detected -> record in dictionary & then deactivate cell
    //         if (neighborColliders.Length > 0)
    //         {

    //             // Debug.Log("Neighbor is terrain");
    //             int neighborIndex = neighborColliders[0].transform.parent.gameObject.GetComponent<ObjectTerrain>().placedObjectIndex;
    //             // GameObject neighbor = neighborCell.transform.parent.gameObject;
    //             // int neighborIndex = neighbor.GetComponent<ObjectTerrain>().placedObjectIndex;
    //             Debug.Log("Neighbor index: " + neighborIndex.ToString());

    //             if (neighborIndex != -1)
    //             {

    //                 // if (inactiveCells.ContainsKey(neighborIndex))
    //                 // {
    //                 //     Debug.Log("Existing neighbor found");
    //                 // }
    //                 // else
    //                 // {
    //                 //     Debug.Log("New neighbor registered");
    //                 // }

    //                 inactiveCells[neighborIndex].Add(childCell);
    //                 childCell.SetActive(false);
    //             }
    //             else
    //             {
    //                 Debug.Log("ERROR: Invalid neighboring object index.");
    //             }


    //         }

    //         // no collisions detected -> keep cell active, change to non-trigger, move to 'terrain'
    //         else
    //         {
    //             childCollider.isTrigger = false;
    //             childCell.layer = terrainLayerNum;
    //         }
    //     }

    // }
}
