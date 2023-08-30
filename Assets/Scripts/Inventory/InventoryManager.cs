using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    // Stored object dictionary to aggregate objects with the same ID in a single inventory slot
    //      - Keys: object IDs
    //      - Values: slot index where objects of this ID are being stored
    private Dictionary<int, int> storedObjectDict = new();

    [SerializeField]
    private InventorySlot[] slots;          // Optionally assigned in the inspector, but can be autopopulated (see below)
    [SerializeField]
    private Transform slotParent;           // Parent for all InventorySlots

    // Tracks the number of object TYPES (i.e. objects of specific ID) currently in the inventory
    // (aka the number of unique object IDs)
    // Starts as 0, to reflect an empty inventory
    private int _numOccupiedSlots = 0;

    // Maximum number of objects that can be stored in the inventory
    private int _maxSize;

    [SerializeField]
    private ObjectDatabaseSO database;

    [SerializeField]
    private PlacementManager placementManager;

    // For 'discovering' objects;
    // Every time an object is first pocketed in the inventory,
    // Player gains information about it & thus 'discovers' it
    [SerializeField]
    private ObjectDiscovery objectDiscovery;

    void Start() {
        if (slots.Length > 0) {             // The array is already populated in the inspector.
            _maxSize = slots.Length;
        }
        else {                              // The array is not yet populated in the inspector.
            Debug.Log("Array of InventorySlots is not yet populated in the Inspector. Autopopulating...");
            PopulateInventoryWithSlots();
        }

        // Some insurance, in case these references haven't been appropariately assigned in the Inspector...
        // Locate them via traversing the hierarchy - but only for objects tagged as managers
        // Much more efficient than base GameObject.FindObjectOfType (which is super slow)
        if (placementManager == null) {
            Debug.Log("WARNING: PlacementManager not initially assigned in InventoryManager. Consider assigning.");
            placementManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<PlacementManager>();
        }
        if (objectDiscovery == null) {
            Debug.Log("WARNING: ObjectDiscovery not initially assigned in InventoryManager. Consider assigning.");
            objectDiscovery = GameObject.FindGameObjectWithTag("Manager").GetComponent<ObjectDiscovery>();
        }
    }

    // Autopopulates the inventory of slots with InventorySlot
    private void PopulateInventoryWithSlots() {
        int numSlots = slotParent.childCount;
        _maxSize = numSlots;
        slots = new InventorySlot[numSlots];
        for (int i = 0; i < numSlots; i++) {
            slots[i] = slotParent.GetChild(i).gameObject.GetComponent<InventorySlot>();
        }
    }

    public void AddToInventory(int objectID) {
        
        // If inventory isn't full yet...
        if (_numOccupiedSlots < _maxSize) {
            
            int slotIndex = -1;
            
            if (storedObjectDict.ContainsKey(objectID)) {           // existing object type/ID
                slotIndex = storedObjectDict[objectID];
            }
            else {                                                  // new object type/ID
                
                // If an object already exists in the inventory (dictionary), then it is
                // already in the discovery dictionary.
                // So, we do this - then perform a unique key check for discovery dictionary
                // to handle the case where we've pocketed and then removed an object from our inventory.
                objectDiscovery.UponPocketing(objectID, database.objects[objectID]);

                slotIndex = _numOccupiedSlots;                      // automatically add to the first unoccupied slot (re:left-to-right)
                storedObjectDict[objectID] = slotIndex;             // update dictionary to track newly stored object type
                
                _numOccupiedSlots += 1;
            }

            AddToSlot(database.objects[objectID].prefab, objectID, slotIndex);
            
        }
        else {
            Debug.Log("Inventory is full. Couldn't add object.");
        }
    }

    // Called upon button click
    // Enter the 'inventory placement' state, where we place objects in the overworld
    // by removing them from the inventory
    public void StartRemovingFromInventory(int slotIndex) {
        
        int removedObjectID = slots[slotIndex].objectID;
        
        if (removedObjectID != -1) {
            
            // Allow player to then PLACE the object
            if (placementManager != null) {
                int numRemainingObjects = slots[slotIndex].GetObjectCount();
                placementManager.StartInventoryPlacement(removedObjectID, slotIndex, numRemainingObjects);       // slotIndex to allow us to return the object to this slot in case player exits placement
            }
            else {
                Debug.Log("Placement manager is null; make sure it's been assigned in the inspector.");
            }
        }
        else {
            Debug.Log("Object was not successfully removed from the slot, as the slot was empty.");
        }
    }

    // Edge case handler
    //      - Handles the specific case wherein the player 'temporarily' extracts the final remaining object from a slot
    //      - This method ensures that if the player escapes before placing object in overworld, the slot repopulates with the object.
    // CALLER: PlacementManager
    public void ReturnToInventory(int objectID, int slotIndex) {
        AddToSlot(database.objects[objectID].prefab, objectID, slotIndex);
    }

    // UI updater
    private void AddToSlot(GameObject objectPrefab, int objectID, int slotIndex) {
        slots[slotIndex].AddObjectToSlot(objectPrefab, objectID);
    }

    // CALLER: PlacementManager
    public int RemoveFromSlot(int slotIndex) {
        slots[slotIndex].RemoveObjectFromSlot();
        return slots[slotIndex].GetObjectCount();
    }

}
