using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State for placing objects in the overworld through the Creative Menu
// Inherits from a 'building state' interface
public class PlacementState : I_BuildingState
{

    // Index of the actively selected object; -1 when no object is selected
    private int selectionIndex = -1;

    int objectID;
    ObjectDatabaseSO database;

    // Grid
    Grid grid;
    GridData gridPlacementData;                 // Data for all placed objects wrt grid

    // Object-related functionalities
    PlaceObject placeObject;
    DisplayObjectPreview previewObject;

    int numRotations;

    AudioManager audioManager;
    CreativeMenu creativeMenu;

    // Constructor
    public PlacementState(int objectID, ObjectDatabaseSO database, Grid grid, GridData gridData, PlaceObject placeObject, DisplayObjectPreview previewObject, AudioManager audioManager, CreativeMenu creativeMenu) {
        
        this.objectID = objectID;
        this.database = database;
        this.grid = grid;
        this.gridPlacementData = gridData;
        this.placeObject = placeObject;
        this.previewObject = previewObject;
        this.audioManager = audioManager;
        this.creativeMenu = creativeMenu;

        selectionIndex = database.objects.FindIndex(objectData => objectData.ID == objectID);

        numRotations = 0;

        // Found a corresponding object matching the specified ID
        if (selectionIndex > -1) {
            // Display preview
            ObjectData selectedObject = database.objects[selectionIndex];
            previewObject.DisplayPreview(selectedObject.prefab, selectedObject.size);
        }
        else {
            Debug.Log("Invalid object ID; no object corresponding with this ID was found.");
        }
    }

    public void UpdatePlacementState(Vector3Int gridPos) {

        bool placementIsLegal = CanPlaceObjectAt(gridPos, selectionIndex);

        // Update preview
        Vector3 worldPos = grid.CellToWorld(gridPos);
        previewObject.UpdatePreview(worldPos, placementIsLegal);
    }

    public void ExitPlacementState() {
        // Stop displaying preview
        previewObject.HidePreview();
        // Reset num rotations
        numRotations = 0;
        // Close dropdown
        // creativeMenu.HideMenu();
        creativeMenu.OnExitPlacementState();
    }

    // MODIFIED: Now returns false if not placed, true if placed
    public bool OnClickDetected(Vector3Int gridPos) {
        
        // Check if this grid position is unoccupied (aka if the attempted object placement would be legal)
        // Proceed if legal placement -- stop if illegal
        if (!CanPlaceObjectAt(gridPos, selectionIndex)) {
            // TODO: Can't place object SFX
            return false;
        }

        // Place object SFX
        audioManager.PlayObjectPlacementSFX();

        // (1) PLACE OBJECT IN OVERWORLD
        // (& retrieve its list index for our dictionary recordkeeping)
        ObjectData thisObject = database.objects[selectionIndex];
        Vector3 worldPos = grid.CellToWorld(gridPos);
        int objIndex = placeObject.Place(thisObject.prefab, worldPos, numRotations);              // returns an index

        // (2) ADD OBJECT TO DICTIONARY
        ObjectPlacement placementData = gridPlacementData.AddObjectAt(gridPos, thisObject.size, thisObject.ID, objIndex);

        // * Add data to object
        placeObject.AddPlacementDataToObject(objIndex, placementData, thisObject);
        previewObject.UpdatePreview(grid.CellToWorld(gridPos), false);

        return true;
    }

    public void OnRotateKeyPressed() {
        if (previewObject.UpdatePreviewRotation()) {    // if rotation was successful...
            numRotations += 1;
        }
    }

    public void OnInventoryKeyPressed() {
        //...do nothing
    }

    private bool CanPlaceObjectAt(Vector3Int gridPos, int objectIndex) {
        
        ObjectData selectedObject = database.objects[objectIndex];
        
        return gridPlacementData.LegalPlacementAt(gridPos, selectedObject.size);

    }

}
