using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{

    [SerializeField]
    private PlaceObject placer;

    [SerializeField]
    private CreativeMenu creativeMenu;

    private InputManager _inputManager;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private ObjectDiscovery objectDiscovery;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectDatabaseSO database;
    // private int selectionIndex = -1;

    // Visualized grid, with tiles representing cells
    [SerializeField]
    private GameObject gridPreview;

    private GridData gridPlacementData;

    // private List<GameObject> placedObjects = new();

    [SerializeField]
    private DisplayObjectPreview preview;

    // Resets when placement is stopped
    private Vector3Int lastDetectedPos = Vector3Int.zero;

    I_BuildingState buildingState;

    // INVENTORY

    [SerializeField]
    private InventoryManager inventory;

    [SerializeField]
    private ObjectTerrainManager terrainManager;


    private int _tempInventoryIndex = -1;

    // Hover detection
    [SerializeField]
    private GameObject hoveredObject;
    [SerializeField]
    private PlacedObjectData hoveredObjectData;

    void Start()
    {

        // Set up a reference to the InputManager
        _inputManager = GameObject.FindObjectOfType<InputManager>();
        if (_inputManager == null)
        {
            Debug.Log("WARNING: No input manager found in scene.");
        }

        // Start with a clean slate
        CancelPlacement();

        // Instantiate data
        gridPlacementData = new();

    }

    void Update()
    {

        if (buildingState == null) {
            CheckForHoveredObject();
            if (hoveredObject != null && hoveredObjectData != null && Input.GetMouseButtonDown(0)) {     // clicking the hovered object -> open up options
                hoveredObjectData.ToggleSelection(true);
                StartInventory();
            }
        }
        else {
            // Calculate mouse position
            Vector3 mousePos = _inputManager.GetGridSelectionPos();
            // Calculate grid position from mouse position
            Vector3Int gridPos = grid.WorldToCell(mousePos);

            // Only update the placement state if the mouse has moved from the last frame
            // Reduces computational expenses - prevents against unnecessary updates
            if (lastDetectedPos != gridPos) {
                buildingState.UpdatePlacementState(gridPos);
                lastDetectedPos = gridPos;                              // Update last detected position
            }
        }
    }

    // When player mouses over a new object
    private void OnHoverInto(GameObject newObject, PlacedObjectData newData) {
        if (newObject != null && newData != null) {
            int objectID = newData.placementData.objectID;
            objectDiscovery.DisplayHoverInfo(newObject, objectID);
            newData.DrawOutline();
            hoveredObjectData = newData;
            hoveredObject = newObject;
        }
    }

    // When player mouses away from a previously hovered object
    private void OnHoverAway(GameObject newObject, PlacedObjectData newData) {
        objectDiscovery.HideHoverUI();
        if (hoveredObject != null && hoveredObjectData != null) hoveredObjectData.HideOutline();
        hoveredObject = newObject;
        hoveredObjectData = newData;
    }

    private void CheckForHoveredObject() {

        GameObject newObject = _inputManager.GetClickedObject();    // Get the GameObject, if any, that the player's mouse is currently hovering over
        PlacedObjectData newData = null;
        if (newObject != null) {
            newData = newObject.GetComponent<PlacedObjectData>();
        }

        bool hoveredAway = false;
        bool hoveredInto = false;

        if (newData != null) {
            if (hoveredObject == null) {                                        // Case 1: Hovered from empty space into new object
                hoveredInto = true;
            } else {
                if (hoveredObjectData != null && newData.instanceID != hoveredObjectData.instanceID) {       // Case 2: Hovered from one object into another object
                    hoveredInto = true;
                    hoveredAway = true;
                }
            }
        }
        else {
            if (hoveredObject != null) {                                        // Case 3: Hovered from object into empty space
                hoveredAway = true;
            }
        }

        if (hoveredAway) {
            OnHoverAway(newObject, newData);
        }
        if (hoveredInto && newData != null) {
            OnHoverInto(newObject, newData);
        }
    }

    public void StartInventory() {

        // Debug.Log("start inventory");

        if (hoveredObject != null && hoveredObject.GetComponent<PlacedObjectData>() != null) {
            
            // Instantiate the inventory state
            buildingState = new InventoryState(hoveredObject, _inputManager, gridPlacementData, placer, objectDiscovery, inventory, audioManager);
            
            // Attach listeners to events
            _inputManager.OnInventory += PlaceInInventory;
            _inputManager.OnClick += TryCancelInventory;
            _inputManager.OnExit += CancelInventory;
        }
        // else {
        //     Debug.Log("tried to start inventory but smth was null");
        // }
        
        // // Attach listeners to events
        // _inputManager.OnInventory += PlaceInInventory;
        // _inputManager.OnClick += TryCancelInventory;
        // _inputManager.OnExit += CancelInventory;
    }

    // Clicking *outside* the currently selected object
    private void TryCancelInventory() {
        // Debug.Log("Trying to cancel inventory (object was deselected/clicked outside of)");
        if (buildingState != null) {
            bool clickOutcome = buildingState.OnClickDetected(Vector3Int.one);
            // Debug.Log(clickOutcome);
            if (clickOutcome) {
                CancelInventory();
            }
        }
    }

    private void CancelInventory() {

        if (buildingState == null) {
            return;
        }

        Debug.Log("Cancel inventory");

        hoveredObjectData.ToggleSelection(false);
        objectDiscovery.HideHoverUI();

        // Exit the placement state
        buildingState.ExitPlacementState();

        // Detach listeners from events
        _inputManager.OnInventory -= PlaceInInventory;
        _inputManager.OnClick -= TryCancelInventory;
        _inputManager.OnExit -= CancelInventory;

        // Reset state
        hoveredObject = null;
        hoveredObjectData = null;
        buildingState = null;
    }

    private void PlaceInInventory() {
        if (buildingState != null) {
            // Debug.Log("I key pressed / PlaceInInventory()");
            buildingState.OnInventoryKeyPressed();      // handle inventory placement logic in InventoryState.cs
            CancelInventory();
        }
    }

    public void StartRemoval() {
        
        // Cancel object placement, if ongoing
        CancelPlacement();

        // Display the grid overlay
        gridPreview.SetActive(true);

        // Instantiate the removal state
        buildingState = new RemovalState(_inputManager, grid, gridPlacementData, placer, preview, audioManager);

        // Disable all object terrain
        // terrainManager.ToggleAllTerrain(false);

        // Attach listeners to events
        _inputManager.OnClick += PlaceObject;
        _inputManager.OnExit += CancelPlacement;

    }

    public void StartPlacement(int objectID)
    {

        CancelPlacement();

        // Display a preview of the transparent grid overlay
        gridPreview.SetActive(true);

        buildingState = new PlacementState(objectID, database, grid, gridPlacementData, placer, preview, audioManager, creativeMenu);

        // Enable all object terrain
        terrainManager.ToggleAllTerrain(true);

        // Attach listeners to these events
        _inputManager.OnClick += PlaceObject;
        _inputManager.OnRotate += RotateObject;
        _inputManager.OnExit += CancelPlacement;

    }

    // Called by InventoryManager
    // Enter the placement state from the inventory (finite object placement)
    // Place an object up to a certain amount (determined by the number of objects, of objectID, left in the specified inventorySlotIndex)
    public void StartInventoryPlacement(int objectID, int inventorySlotIndex, int objectCount) {
        
        CancelPlacement();

        // Display a preview of the transparent grid overlay
        gridPreview.SetActive(true);

        buildingState = new PlacementState(objectID, database, grid, gridPlacementData, placer, preview, audioManager, creativeMenu);

        // NEW: Enable all object terrain
        terrainManager.ToggleAllTerrain(true);

        _tempInventoryIndex = inventorySlotIndex;

        // Attach **MODIFIED** listeners to these events
        _inputManager.OnClick += PlaceObject_UpdateInventory;
        _inputManager.OnRotate += RotateObject;
        _inputManager.OnExit += CancelPlacement_ReturnToInventory;

    }

    // Cancel the process of placing an object
    private void CancelPlacement()
    {

        if (buildingState == null)
        {
            return;
        }

        // Reset visualizations, i.e. object placement preview + cell placement indicator
        gridPreview.SetActive(false);

        // Exit the placement state
        buildingState.ExitPlacementState();

        // Detach listeners to these events
        _inputManager.OnClick -= PlaceObject;
        _inputManager.OnRotate -= RotateObject;
        _inputManager.OnExit -= CancelPlacement;

        // Reset the last detected position
        lastDetectedPos = Vector3Int.zero;

        // Reset the placement state**
        buildingState = null;
    }

    private void CancelPlacement_ReturnToInventory() {

        // Debug.Log("Cancel placement & return to inventory");
        CancelPlacement();

        // Detach listeners from events
        _inputManager.OnClick -= PlaceObject_UpdateInventory;
        _inputManager.OnRotate -= RotateObject;
        _inputManager.OnExit -= CancelPlacement_ReturnToInventory;

        // Reset temporary inventory index
        _tempInventoryIndex = -1;
    }

    // Place an object
    // POLISH: This is a misleading name because it's essentially
    // detecting a click on an object -- and letting the active 'building state'
    // handle it accordingly
    // the click leads to an object placement if that's the active state,
    // and an object removal if *that's* the active state
    private void PlaceObject()
    {

        // First, ensure that the player isn't clicking on any UI elements
        if (_inputManager.IsPointerOverUI() || buildingState == null)
        {
            return;
        }

        // Calculate where mouse is currently positioned on the grid
        Vector3 mousePos = _inputManager.GetGridSelectionPos();
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        buildingState.OnClickDetected(gridPos);
    }

    private void PlaceObject_UpdateInventory() {
        
        // First, ensure that the player isn't clicking on any UI elements
        if (_inputManager.IsPointerOverUI() || buildingState == null)
        {
            return;
        }

        // Calculate grid position from mouse click position
        Vector3 mousePos = _inputManager.GetGridSelectionPos();
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        if (buildingState.OnClickDetected(gridPos)) {           // only on successful placement
            
            // Update inventory with just-removed object,
            // and check how many objects there are remaining in the slot
            int numRemainingObjects = inventory.RemoveFromSlot(_tempInventoryIndex);

            // If no objects remaining, exit placement
            if (numRemainingObjects == 0) {

                // Debug.Log("Inventory slot has been emptied. No objects remaining; exiting placement.");
                
                CancelPlacement();

                // Detach listeners
                _inputManager.OnClick -= PlaceObject_UpdateInventory;
                _inputManager.OnRotate -= RotateObject;
                _inputManager.OnExit -= CancelPlacement_ReturnToInventory;
            } else {
                // Debug.Log("There are " + numRemainingObjects.ToString() + " objects left in this slot.");
            }
        }
    }

    private void RotateObject() {
        if (buildingState != null) {
            buildingState.OnRotateKeyPressed();
        }
    }
}
