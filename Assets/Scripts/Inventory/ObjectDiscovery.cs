using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectDiscovery : MonoBehaviour
{

    [SerializeField]
    // Dictionary
    //      - keys: object ID
    //      - values: object data (name, ID, size, prefab, etc... what we're interested in displaying)
    private Dictionary<int, ObjectData> discoveredObjects = new();

    // **Hover UI**

    // Displays the name of the object if it's previously been discovered;
    // and '???' if it hasn't.
    [SerializeField]
    private GameObject hoverUI;
    private RectTransform hoverRectTransform;
    [SerializeField]
    private TMP_Text hoverText;
    [SerializeField]
    private RectTransform canvasRectTransform;
    [SerializeField]
    private Vector2 offset = new Vector2(-2f, -2f);    // offset of popup UI from mouse position
    
    [SerializeField]
    private GameObject inventoryPrompt;

    private Camera _playerCam;

    [SerializeField]
    private int hoveredID = -1;      // ID of presently hovered object (-1 if nothing)

    void Start() {

        _playerCam = Camera.main;
        hoverRectTransform = hoverUI.GetComponent<RectTransform>();

        // All UI starts off as inactive
        hoverUI.SetActive(false);
        inventoryPrompt.SetActive(false);
    }

    void Update() {
        if (hoveredID != -1) {
            UpdateHoverUI();
        }
    }

    // Called every time an object is 'pocketed' in an inventory (aka our discovery mechanism)
    // Check to see if this object has prev. been encountered before
    public void UponPocketing(int objectID, ObjectData objectData) {
        if (!discoveredObjects.ContainsKey(objectID)) {                 // New object discovered
            discoveredObjects[objectID] = objectData;
        }
    }

    // Display name of object upon hovering over it
    // (only when we aren't in PlacementState or RemovalState)
    public void DisplayHoverInfo(GameObject hoveredObject, int objectID) {
        if (hoverUI != null && hoveredObject != null) {
            if (!hoverUI.activeSelf) hoverUI.SetActive(true);
            hoveredID = objectID;
        }
    }

    private void UpdateHoverUI() {

        // Update position
        Vector2 mousePos = Input.mousePosition;
        Vector2 offsetPos = new Vector2(mousePos.x + offset.x, mousePos.y + offset.y);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, offsetPos, null, out localPos);
        hoverRectTransform.localPosition = localPos;

        // Update text
        if (!discoveredObjects.ContainsKey(hoveredID)) {             // if unknown
            hoverText.text = "Undiscovered shape";
        } else {                                                    // if known
            hoverText.text = discoveredObjects[hoveredID].name;
        }
    }

    // Upon hovering away (mouse grid pos in unoccupied cell)
    public void HideHoverUI() {
        Debug.Log("hide hover UI");
        hoverUI.SetActive(false);
        hoveredID = -1;
    }

    public void ToggleInventoryPrompt(bool setAsActive) {
        inventoryPrompt.SetActive(setAsActive);
    }
}
