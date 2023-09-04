using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot_Screen : MonoBehaviour
{

    [HideInInspector]
    public DisplayInventory displayer;

    public int objectID = -1;

    // [SerializeField]
    private Button _button;
    // [SerializeField]
    private TMP_Text _textCounter;

    // [SerializeField]
    private Transform iconParent;
    private GameObject icon;
    private GameObject iconPrefab;

    // [SerializeField]
    private LayerMask renderLayer;

    private void Start() {

        renderLayer = LayerMask.NameToLayer("UI_FullScreen");

        iconParent = transform.Find("IconParent");
        if (iconParent == null) {
            Debug.Log("WARNING: Icon parent for this inventory slot not found!");
        }

        _textCounter = GetComponentInChildren<TMP_Text>();
        if (_textCounter == null) {
            Debug.Log("WARNING: Text counter for this inventory slot not found!");
        }
        UpdateCounter(0);                                       // Set the initial text counter to be empty (0)
        
        _button = GetComponent<Button>();
        if (_button == null) {
            Debug.Log("WARNING: Button for this inventory slot not found!");
        }
        _button.interactable = false;                           // Disable the button interaction initially, since the slot is empty
    }

    // CALLER: DisplayInventory
    // Add an object to this slot
    public void AddObject(GameObject prefab, int count, int ID) {
        UpdateCounter(count);
        if (count == 1) {           // newly created
            SetIcon(prefab);
            _button.interactable = true;
            objectID = ID;
        }
    }

    public void RemoveObject(int count) {
        UpdateCounter(count);
        if (count == 0) {
            ClearIcon();
            _button.interactable = false;
            objectID = -1;
        }
    }

    public void UpdateCounter(int count) {
        Debug.Log("Count in UpdateCounter(...) is: " + count.ToString());
        if (count > 0) {
            _textCounter.SetText(count.ToString());
        } else {
            _textCounter.SetText("");
        }
    }

    // Called by the button attached to the GameObject that this slot represents
    public void OnButtonPress() {
        displayer.HandleButtonPress(objectID);
    }

    // Handling the 3D icon of the stored object
    // All logic lifted from InventorySlot

    private void SetIcon(GameObject objectPrefab) {
        iconPrefab = objectPrefab;
        icon = Instantiate(objectPrefab, iconParent, false);
        icon.layer = renderLayer;
        Transform cubits = icon.transform.Find("Cubits");
        foreach (Transform cubit in cubits) {
            cubit.gameObject.layer = renderLayer;
        }
    }

    private void ClearIcon() {
        if (icon != null) {
            GameObject thisIcon = icon;
            icon = null;
            Destroy(thisIcon);
            
            iconPrefab = null;
        }
    }
}
