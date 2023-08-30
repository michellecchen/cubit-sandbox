using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryState : I_BuildingState
{
    GameObject selectedObject;
    private PlacedObjectData selectedData;

    InputManager inputManager;

    // Grid
    Grid grid;
    GridData gridPlacementData;                 // Data for all placed objects wrt grid

    // Object-related functionalities
    PlaceObject placeObject;
    ObjectDiscovery objectDiscovery;

    InventoryManager inventory;

    AudioManager audioManager;

    // Constructor
    public InventoryState(GameObject selectedObject, InputManager inputManager, GridData gridPlacementData, PlaceObject placeObject, ObjectDiscovery objectDiscovery, InventoryManager inventory, AudioManager audioManager) {
        
        this.selectedObject = selectedObject;
        this.inputManager = inputManager;
        this.gridPlacementData = gridPlacementData;
        this.placeObject = placeObject;
        this.objectDiscovery = objectDiscovery;
        this.inventory = inventory;
        this.audioManager = audioManager;

        selectedData = selectedObject.GetComponent<PlacedObjectData>();
        selectedData.Enlarge();
        objectDiscovery.ToggleInventoryPrompt(true);
        // Play selection sound
        audioManager.PlayObjectSelectionSFX();
    }

    #region Interface methods

    public void UpdatePlacementState(Vector3Int gridPos) {
        // ...
    }

    // On escape
    public void ExitPlacementState() {
        ResetSelection();
    }

    // Handles clicks OUTSIDE the current selection;
    // RETURNS: True if the click was outside the current selection; false otherwise
    // Clicking anywhere outside of the current selection will deselect the object & end the inventory state.
    // In this context, true = end selection (click has been detected *outside* of currently selected object)
    public bool OnClickDetected(Vector3Int gridPos) {
        
        GameObject clickedObject = inputManager.GetClickedObject();
        PlacedObjectData clickedData = null;
        if (clickedObject != null) {
            clickedData = clickedObject.GetComponent<PlacedObjectData>();
            if (clickedData != null && clickedData.instanceID == selectedData.instanceID) {
                // Debug.Log("SAME OBJECT CLICKED");
                return false;
            }
        }

        return true;
    }

    public void OnRotateKeyPressed() {
        // ...do nothing
    }

    public void OnInventoryKeyPressed() {
        
        if (selectedObject != null) {
            
            // Extract relevant info from object detected via raycasted click
            ObjectPlacement placementData = selectedObject.GetComponent<PlacedObjectData>().placementData;
            Vector3Int occupiedCell = placementData.occupiedPositions[0];
            
            // Remove from data
            int removedObjectID = gridPlacementData.RemoveObjectAt(occupiedCell);
            // Remove from overworld
            placeObject.Remove(placementData.placedObjectIndex);
            // Play object removal sound
            audioManager.PlayObjectRemovalSFX();
            // Hide hover UI
            objectDiscovery.HideHoverUI();
            objectDiscovery.ToggleInventoryPrompt(false);
            
            // Add to inventory
            inventory.AddToInventory(removedObjectID);
        }
    }

    // Deselect the currently selected object
    private void ResetSelection() {
        if (selectedObject != null) {
            Debug.Log("Reset selection");
            // selectedObject.GetComponent<PlacedObjectData>().HideOutline();
            objectDiscovery.HideHoverUI();
            selectedData.HideOutline();
            selectedData.Shrink();
            selectedObject = null;
        }
    }

    #endregion
}
