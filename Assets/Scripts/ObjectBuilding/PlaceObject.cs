using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObject : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> placedObjects = new();     // List of all currently placed objects

    [SerializeField]
    private ObjectTerrainManager objectTerrain;

    // Place the object at the specified position in the world,
    //      with the specified number of 90-degree rotations around the y-axis
    // Return: Index of the newly placed object in the list of all placed objects
    public int Place(GameObject objectPrefab, Vector3 worldPosition, int numRotations) {

        GameObject newObject = Instantiate(objectPrefab);       // Instantiate GameObject via the provided prefab

        newObject.transform.position = worldPosition;           // Set its position on the grid, in world space
        Rotate(newObject.transform, numRotations);
        
        placedObjects.Add(newObject);                           // Add the newly placed object to the list of placed objects

        // Send information to ObjectTerrainManager
        //      - the newly instantiated GameObject
        //      - the index of this object in the placedObjects list
        int objectIndex = placedObjects.Count - 1;              // Since the object is added by default to the end of the list, its index is simply count-1
        objectTerrain.AddToTerrain(newObject, objectIndex);

        return objectIndex;
    }

    public void AddPlacementDataToObject(int placementIndex, ObjectPlacement placementData, ObjectData objectData) {
        GameObject thisObject = placedObjects[placementIndex];
        // essentially a wrapper
        PlacedObjectData objectDataComp = thisObject.AddComponent<PlacedObjectData>();
        objectDataComp.placementData = placementData;
        objectDataComp.objectData = objectData;
    }

    // Apply a specified number of 90-degree rotations to the object
    public void Rotate(Transform newObject, int numRotations) {
        // Create a rotation pivot; place it at the bottom-center of the object's origin
        //      (which, by default, is at the bottom-left corner of the shape)
        GameObject rotPivot = new GameObject("RotationPivot_PlacedObject");
        Vector3 pivotPos = newObject.position + new Vector3(0.5f, 0.0f, 0.5f);
        rotPivot.transform.position = pivotPos;

        // Parent (temporarily) the object to the pivot
        newObject.parent = rotPivot.transform;

        // Rotate the pivot by 90 deg. around y-axis
        rotPivot.transform.Rotate(Vector3.up, numRotations * 90.0f);

        // Unparent the object from the pivot
        // newObject.parent = null;
        newObject.SetParent(null, true);        // maintain world position

        // Destroy the rotation pivot, now that we're done with it
        if (rotPivot != null) Destroy(rotPivot);
    }

    // TODO: Update to 'place in inventory'
    // PlaceInOverworld vs. PlaceInInventory
    public void Remove(int gameObjectIndex) {
        
        if (placedObjects.Count <= gameObjectIndex || placedObjects[gameObjectIndex] == null) {
            return;
        }

        // Before destroying the object to be removed -- update the object terrain.
        // (reactivate previously inactive/collided terrain cells of neighboring objects)
        objectTerrain.RemoveFromTerrain(gameObjectIndex);

        Destroy(placedObjects[gameObjectIndex]);            // destroy from scene
        placedObjects[gameObjectIndex] = null;              // clear from list
        
    }
}
