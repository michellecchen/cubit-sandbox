using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTerrainManager : MonoBehaviour
{

    // Dictionary of all object-based terrain
    //      - keys: index of placed object w/ this terrain
    //      - values: ObjectTerrain
    Dictionary<int, ObjectTerrain> terrainDict = new();

    // Adds an object to the terrain.
    // Takes as input:
    //      - the newly placed GameObject
    //      - its index in the list of placed objects
    public void AddToTerrain(GameObject newObject, int placedObjectIndex)
    {
        // Debug.Log("Adding object with index " + placedObjectIndex.ToString() + " to terrain");
        ToggleTerrainOf(newObject, true);
        ObjectTerrain newTerrain = newObject.GetComponentInChildren<ObjectTerrain>();
        newTerrain.CreateObjectTerrain(placedObjectIndex);
        terrainDict[placedObjectIndex] = newTerrain;
    }

    // Removes an object from the terrain.
    public void RemoveFromTerrain(int placedObjectIndex)
    {
        terrainDict.Remove(placedObjectIndex);
    }

    public void ToggleAllTerrain(bool setAsActive)
    {
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
