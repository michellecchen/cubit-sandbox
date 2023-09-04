using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Display a fullscreen inventory upon pressing E key
/// </summary>
public class DisplayInventory : MonoBehaviour
{

    [SerializeField]
    private Camera UICamera;

    [SerializeField]
    //private GameObject _inventoryScreen;                // fullscreen inventory UI
    private GraphicRaycaster _raycaster;

    private List<InventorySlot_Screen> slots = new();

    [SerializeField]
    private GameObject slotPrefab;                      // inventory slot prefab
    private float slotSize = 100f;

    [SerializeField]
    private Transform _panel;
    private RectTransform _panelRT;

    [SerializeField]
    private InventoryManager _manager;

    [SerializeField]
    private Transform objectFocusParent;
    private Animator objectFocusAnim;
    private GameObject currentFocus;

    // key: objectID
    // value: Reference to 'objectFocus' GameObject
    [SerializeField]
    private ObjectDatabaseSO database;
    private Dictionary<int, GameObject> objectFocusDict = new();

    private LayerMask renderLayer;

    [Header("Info text")]

    [SerializeField] private GameObject objectInfo;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text color;
    [SerializeField] private TMP_Text size;

    [Header("Empty inventory prompt")]
    [SerializeField] private GameObject emptyPrompt;


    private void Start()
    {

        renderLayer = LayerMask.NameToLayer("UI_FullScreen");

        _panelRT = _panel.GetComponent<RectTransform>();

        objectFocusAnim = objectFocusParent.GetComponent<Animator>();

        // Set up inventory slots
        PopulateSlots();

        // Set up object focus renders
        PopulateFocus();

        Hide();

        objectInfo.SetActive(false);

        emptyPrompt.SetActive(true);
    }

    #region Camera culling mask manipulation

    // Turn on the bit using an OR operation
    private void Show()
    {
        UICamera.cullingMask |= 1 << LayerMask.NameToLayer("UI_FullScreen");
        _raycaster.enabled = true;
    }

    // Turn off the bit using an AND operation with the complement of the shifted int
    private void Hide()
    {
        UICamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI_FullScreen"));
        _raycaster.enabled = false;
    }

    // Toggle the bit using a XOR operation
    private void Toggle()
    {
        UICamera.cullingMask ^= 1 << LayerMask.NameToLayer("UI_FullScreen");
        _raycaster.enabled = (!_raycaster.enabled);
    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleFullscreenInventory();
        }
    }

    public void ToggleFullscreenInventory()
    {
        Toggle();
    }

    // "Empty prompt" reads "You currently have no objects in your inventory!"
    public void ToggleEmptyPrompt() {
        emptyPrompt.SetActive(!emptyPrompt.activeSelf);
    }

    private void PopulateFocus()
    {

        foreach (ObjectData obj in database.objects)
        {

            GameObject newFocus = Instantiate(obj.prefab, objectFocusParent, false);

            // Set instantiated object to proper render layer
            newFocus.layer = renderLayer;
            Transform cubits = newFocus.transform.Find("Cubits");
            foreach (Transform cubit in cubits)
            {
                cubit.gameObject.layer = renderLayer;
            }

            objectFocusDict[obj.ID] = newFocus;

            newFocus.SetActive(false);
        }
    }

    private void PopulateSlots()
    {

        int numSlots = _manager._maxSize;

        float panelWidth = _panelRT.rect.width;
        float panelHeight = _panelRT.rect.height;
        // Debug.Log("Panel has width " + panelWidth.ToString() + " and height " + panelHeight.ToString());

        // Calculate the number of rows and columns based on panel size and slot size
        int cols = Mathf.FloorToInt(panelWidth / slotSize);
        int rows = Mathf.CeilToInt((float)numSlots / cols);

        // Calculate dynamic horizontal and vertical spacing based on panel size and slot size
        float horizontalSpacing = (panelWidth - (cols * slotSize)) / (cols - 1);
        float verticalSpacing = horizontalSpacing;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {

                // Index of slot
                int i = r * cols + c;

                // Stop creating slots when we have enough
                if (i >= numSlots)
                {
                    return;
                }

                // Calculate newly created slot's position
                float xPos = c * (slotSize + horizontalSpacing) + (slotSize / 2);
                float yPos = -r * (slotSize + verticalSpacing) - (slotSize / 2);
                // Debug.Log("Placing slot at (" + xPos.ToString() + "," + yPos.ToString() + ")");

                // Instantiate a slot prefab (as a child of the panel)
                GameObject newSlot = Instantiate(slotPrefab, _panel);

                // Log the InventorySlot_Screen component of the newly created slot in the dictionary
                InventorySlot_Screen newSlotComp = newSlot.GetComponent<InventorySlot_Screen>();
                newSlotComp.displayer = this;
                slots.Add(newSlotComp);
                
                // Set its position
                RectTransform newTransform = newSlot.GetComponent<RectTransform>();
                newTransform.anchoredPosition = new Vector3(xPos, yPos, 0f);

            }
        }

        // Make sure the size of the inventory panel fits all slots
        _panelRT.sizeDelta = new Vector2(panelWidth, panelHeight);
    }

    // Render the object in focus, on the left-side of the screen
    public void HandleButtonPress(int objectID)
    {

        objectInfo.SetActive(true);
        SetInfoText(objectID);

        if (currentFocus != null) {                     // Swap out old focus for current focus
            // objectFocusAnim.SetBool("spinning", false);
            currentFocus.SetActive(false);
        }

        currentFocus = objectFocusDict[objectID];       // Make current focus visible
        currentFocus.SetActive(true);
        // objectFocusAnim.SetBool("spinning", true);
    }

    private void SetInfoText(int objectID) {
        ObjectData data = database.objects[objectID];
        title.text = data.name;
        size.text = data.size[0].ToString() + "x" + data.size[1].ToString() + "x" + data.size[2].ToString();
        // color.text = data.color;
        color.text = Enum.GetName(typeof(ObjectColor), data.color);
    }

    public void AddToSlot(int slotIndex, GameObject objectPrefab, int objectCount, int objectID)
    {
        slots[slotIndex].AddObject(objectPrefab, objectCount, objectID);
    }

    public void RemoveFromSlot(int slotIndex, int objectCount)
    {
        slots[slotIndex].RemoveObject(objectCount);
    }
}
