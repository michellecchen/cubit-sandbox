using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State for removing objects from the overworld.
// Entered through the Creative Menu's Delete option.
public class RemovalState : I_BuildingState
{
    // Index of the object from the placed objects list
    private int gameObjectIndex = -1;

    // Input manager - for detecting clicks on removable objects
    InputManager inputManager;

    Grid grid;                                  // Grid
    GridData gridPlacementData;                 // Data for all placed objects wrt grid

    // Object-related functionalities
    PlaceObject placeObject;
    DisplayObjectPreview previewObject;

    AudioManager audioManager;

    // Constructor
    public RemovalState(InputManager inputManager, Grid grid, GridData gridPlacementData, PlaceObject placeObject, DisplayObjectPreview previewObject, AudioManager audioManager) {
        
        this.inputManager = inputManager;
        this.grid = grid;
        this.gridPlacementData = gridPlacementData;
        this.placeObject = placeObject;
        this.previewObject = previewObject;
        this.audioManager = audioManager;

        previewObject.DisplayRemovalPreview();
    }

    #region Interface methods

    // TODO: EDIT
    public void UpdatePlacementState(Vector3Int gridPos) {
        // Validate if the clicked position contains a removable object
        bool isRemovable = RemovableObjectAt(gridPos);
        
        Vector3 worldPos = grid.CellToWorld(gridPos);
        previewObject.UpdatePreview(worldPos, isRemovable);
    }

    public void ExitPlacementState() {
        previewObject.HidePreview();
    }

    public bool OnClickDetected(Vector3Int gridPos) {

        GameObject clickedObject = inputManager.GetClickedObject();
        
        if (clickedObject == null) {
            return false;
        }

        // Extract relevant info from object detected via raycasted click
        ObjectPlacement placementData = clickedObject.GetComponent<PlacedObjectData>().placementData;
        Vector3Int occupiedCell = placementData.occupiedPositions[0];
        // Remove from data
        int removedObjectID = gridPlacementData.RemoveObjectAt(occupiedCell);
        // Remove from overworld
        placeObject.Remove(placementData.placedObjectIndex);
        // Play object removal sound
        audioManager.PlayObjectRemovalSFX();

        return true;
    }

    public void OnRotateKeyPressed() {
        // ...do nothing
    }

    public void OnInventoryKeyPressed() {
        //...do nothing
    }

    #endregion

    // Check if there's a removable object that exists at the clicked position
    // Returns true if there is a removable object, and false if there isn't
    private bool RemovableObjectAt(Vector3Int gridPos) {
        // Here, we take advantage of the fact that an 'illegal' placement means the space is already occupied by an object.
        // If an object exists at this space, that means it's removable.
        return !(gridPlacementData.LegalPlacementAt(gridPos, Vector3Int.one));
    }
}
