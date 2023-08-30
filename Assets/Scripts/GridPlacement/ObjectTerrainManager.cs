using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTerrainManager : MonoBehaviour
{

    // List<ObjectTerrain> terrainList = new();

    // Dictionary of all object-based terrain
    //      - keys: index of placed object w/ this terrain
    //      - values: ObjectTerrain
    Dictionary<int, ObjectTerrain> terrainDict = new();

    // public bool objectTerrainOff = false;

    // // Stores information re: the inactive terrain cells for every object
    // //      - keys:     index of the object in the placedObjects list (in PlaceObject.cs)
    // //      - values:   list of currently deactivated terrain cells for the GameObject
    // private Dictionary<int, List<GameObject>> inactiveTerrain;

    // private Dictionary<int, int[]> neighborsDictionary;

    // Adds an object to the terrain.
    // Takes as input:
    //      - the newly placed GameObject
    //      - its index in the list of placed objects
    public void AddToTerrain(GameObject newObject, int placedObjectIndex)
    {
        Debug.Log("Adding object with index " + placedObjectIndex.ToString() + " to terrain");
        // newObject.GetComponent<DynamicTerrain>().objectID = placedObjectIndex;
        ToggleTerrainOf(newObject, true);
        ObjectTerrain newTerrain = newObject.GetComponentInChildren<ObjectTerrain>();
        newTerrain.CreateObjectTerrain(placedObjectIndex);
        // terrainList.Add(newTerrain);
        terrainDict[placedObjectIndex] = newTerrain;
    }

    // Removes an object from the terrain.
    public void RemoveFromTerrain(int placedObjectIndex)
    {
        // ...
        // Somehow need to find a way to remove from terrainList accordingly, too
        terrainDict.Remove(placedObjectIndex);
    }

    // // Activate all terrain cells (which begin as inactive)
    // private void CheckForCollisions(GameObject thisObject) {
    //     ToggleTerrainOf(thisObject, true);                      // activate terrain
    // }

    public void ToggleAllTerrain(bool setAsActive)
    {
        // foreach (ObjectTerrain terrain in terrainList) {
        //     terrain.gameObject.SetActive(setAsActive);
        // }
        foreach (var terrain in terrainDict.Values)
        {
            terrain.gameObject.SetActive(setAsActive);
        }
    }

    private void ToggleTerrainOf(GameObject thisObject, bool terrainBecomesActive)
    {
        thisObject.transform.Find("Terrain").gameObject.SetActive(terrainBecomesActive);
    }
}
