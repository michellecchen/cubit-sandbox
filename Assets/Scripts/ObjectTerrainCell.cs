using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTerrainCell : MonoBehaviour
{
    // public bool checkingForCollisions = false;

    // -1 by default; indicates no neighboring/colliding terrain cell
    // public GameObject neighborCell;
    public int neighborIndex = -1;

    // For drawing Physics.OverlapBox

    private Vector3 _boxCenter;
    private Vector3 _boxHalfExtents;
    private Quaternion _boxRotation;

    [SerializeField]
    private int _terrainLayerMask;
    private int terrainLayerNum = 7;


    void Start() {

        // Calculate params for overlap box (drawn 0.25 units in the forward direction of the plane)
        _boxCenter = transform.position + transform.forward * 0.25f;
        _boxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f);
        _boxRotation = transform.rotation;

        // Set the terrain layer mask (using bitwise left shift operator)
        _terrainLayerMask = 1 << terrainLayerNum;
    }

    // Check for collisions w/ a neighboring cell
    public int CheckForCollisions() {
        // Activate collider component
        // GetComponent<Collider>().enabled = true;
        
        Collider[] neighboringColliders = Physics.OverlapBox(_boxCenter, _boxHalfExtents, _boxRotation, _terrainLayerMask);
        Debug.Log("Number of neighboring colliders found: " + neighboringColliders.Length.ToString());

        if (neighboringColliders.Length > 0) {
            GameObject neighborCell = neighboringColliders[0].gameObject;
            neighborIndex = neighborCell.transform.parent.gameObject.GetComponent<ObjectTerrain>().placedObjectIndex;
        } else {
            neighborIndex = -1; // not sure if necessary?
        }
        return neighborIndex;

            //         GameObject neighborCell = other.gameObject;
    //         neighborIndex = neighborCell.transform.parent.gameObject.GetComponent<ObjectTerrain>().placedObjectIndex;
    //         Debug.Log("neighbor index: " + neighborIndex.ToString());
    }

    // private void OnDrawGizmosSelected() {
    //     // Draw a wireframe cube gizmo to visualize the box
    //     Gizmos.color = Color.yellow;
    //     Gizmos.matrix = Matrix4x4.TRS(_boxCenter, _boxRotation, _boxHalfExtents);
    //     Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
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

    public void SetToTerrainLayer() {
        // Debug.Log("Set to terrain layer");
        gameObject.layer = terrainLayerNum;
    }

    // private void OnTriggerEnter(Collider other) {

    //     if (other.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
    //         Debug.Log("Found collision with terrain");
    //         // transform.parent.gameObject.SendMessage("HandleCollision", gameObject, other.gameObject);
    //         // Debug.Log("collision detected");
            
    //         // transform.parent.gameObject.GetComponent<ObjectTerrain>().HandleCollision(gameObject, other.gameObject);
    //         GameObject neighborCell = other.gameObject;
    //         neighborIndex = neighborCell.transform.parent.gameObject.GetComponent<ObjectTerrain>().placedObjectIndex;
    //         Debug.Log("neighbor index: " + neighborIndex.ToString());

    //     }
    // }
}
