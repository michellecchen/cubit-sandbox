using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{

    public int objectID = -1;

    // Number of objects with the same ID that are currently stored at this slot
    [HideInInspector]
    public int objectCounter = 0;

    // In inventory panel, at the bottom of the screen
    private RectTransform _rectTransform;
    private Vector3 defaultScale;           // when unselected
    private Vector3 selectedScale;          // when selected

    private Button _button;
    private TMP_Text _textCounter;

    [SerializeField]
    private Transform iconParent;
    private GameObject icon;
    private LayerMask renderLayer;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        _textCounter = GetComponentInChildren<TMP_Text>();

        selectedScale = (Vector3.one * 1.1f);
        defaultScale = (Vector3.one * 0.85f);

        _button.interactable = false;                           // Disable the button interaction initially, since the slot is empty
        UpdateCounter();                                        // Set the initial text counter to be empty (0)

        renderLayer = LayerMask.NameToLayer("RenderTexture");
    }

    public void AddObjectToSlot(GameObject obj, int objID) {

        if (SlotIsEmpty()) {
            SetIcon(obj);
            objectID = objID;                   // update object ID
            _button.interactable = true;        // Enable button interaction now that the slot is filled
        }

        // Update object count
        objectCounter += 1;                     // update internally tracked counter
        UpdateCounter();                        // update UI counter

    }

    // Returns the ID of the object that was just removed
    public int RemoveObjectFromSlot() {

        if (!SlotIsEmpty()) {                   // if the slot can be emptied...

            objectCounter -= 1;
            UpdateCounter();

            if (objectCounter > 0) {            // if there are still objects left
                // ..
            }
            else {                              // if there are no objects left
                objectID = -1;                      // reset object ID
                _button.interactable = false;       // disable button interactions again (empty slot)
                DeselectUI();
                ClearIcon();

            }
            int removedID = objectID;           // ID of the object that will be imminently removed
            return removedID;

        }
        else {
            return -1;                          // slot was already empty
        }
    }

    public int GetObjectCount() {
        return objectCounter;
    }

    private bool SlotIsEmpty() {
        return (objectID == -1);
    }

    private void UpdateCounter() {
        if (objectCounter > 0) {
            _textCounter.SetText(objectCounter.ToString());
        } else {
            _textCounter.SetText("");                           // empty string (will be invisible)
        }
    }

    private void SetIcon(GameObject objectPrefab) {
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
        }
    }

    // Update the UI to reflect this current slot's selected state,
    // where player is actively extracting objects from this slot
    public void SelectUI() {
        _rectTransform.localScale = selectedScale;
    }

    public void DeselectUI() {
        _rectTransform.localScale = defaultScale;
    }
}
