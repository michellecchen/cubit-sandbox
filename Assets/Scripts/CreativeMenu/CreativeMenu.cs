using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// For adding/deleting objects from the overworld.
public class CreativeMenu : MonoBehaviour
{
    
    [SerializeField]
    private ObjectDatabaseSO database;
    
    [SerializeField]
    private CreateObject[] creatableObjects;

    [SerializeField]
    private GameObject dropdown;

    [SerializeField]
    private PlacementManager placementManager;

    [SerializeField]
    private Toggle createToggle;
    [SerializeField]
    private Toggle deleteToggle;

    [Header("For toggle UI")]
    public Color defaultColor;
    public Color highlightedColor;

    void Start() {

        // Populate list of creatable objects
        for (int i = 0; i < database.objects.Count; i++) {
            int ID = database.objects[i].ID;
            string name = database.objects[i].name;
            GameObject prefab = database.objects[i].prefab;
            creatableObjects[i].SetInfo(ID, prefab, name);
        }

        // Dropdown begins as invisible
        dropdown.SetActive(false);

        // For color handling
        createToggle.onValueChanged.AddListener(OnCreateValueChanged);
        deleteToggle.onValueChanged.AddListener(OnDeleteValueChanged);
        // Set initial colors (both start as off by default)
        HandleToggleValueChanged(createToggle, false);
        HandleToggleValueChanged(deleteToggle, false);
    }

    // Color handling

    private void OnCreateValueChanged(bool toggleValue) {
        HandleToggleValueChanged(createToggle, toggleValue);
    }

    private void OnDeleteValueChanged(bool toggleValue) {
        HandleToggleValueChanged(deleteToggle, toggleValue);
    }

    private void HandleToggleValueChanged(Toggle thisToggle, bool isOn) {
        ColorBlock cb = thisToggle.colors;
        if (!isOn) {
            cb.normalColor = defaultColor;
            cb.highlightedColor = defaultColor;
            cb.selectedColor = defaultColor;
        } else {
            cb.normalColor = highlightedColor;
            cb.highlightedColor = highlightedColor;
            cb.selectedColor = highlightedColor;
        }
        thisToggle.colors = cb;
    }

    // For functionality (opening/closing object dropdown menu, etc.)

    // Called when Create operation is pressed (paintbrush icon)
    public void HandleCreateToggle(bool toggledOn) {
        dropdown.SetActive(toggledOn);
    }

    // Called when Delete operation is pressed (trashcan icon)
    public void HandleDeleteToggle(bool toggledOn) {
        if (toggledOn) placementManager.StartRemoval();
        else placementManager.CancelPlacement();
    }

    // When 'esc' is pressed while in the InventoryState
    // CALLER: PlacementManager
    public void OnExitPlacementState() {
        createToggle.isOn = false;
        dropdown.SetActive(false);
    }

    // When 'esc' is pressed while in the RemovalState
    // CALLER: PlacementManager
    public void OnExitRemovalState() {
        deleteToggle.isOn = false;
    }

    // // Display dropdown w/ all options
    // public void ExpandMenu() {
    //     dropdown.SetActive(true);
    // }

    // public void HideMenu() {
    //     dropdown.SetActive(false);
    // }
}

