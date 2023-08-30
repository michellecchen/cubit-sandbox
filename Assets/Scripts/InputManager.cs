using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Mouse input manager
public class InputManager : MonoBehaviour
{

    private Camera _playerCam;

    // Last mouse position
    private Vector3 lastPos;

    [Header("Layer mask for distinguishing terrain")]
    [SerializeField]
    private LayerMask terrainLayerMask;

    void Start() {
        _playerCam = Camera.main;
    }

    public event Action OnClick, OnRotate, OnExit, OnInventory;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {          // Make selection
            OnClick?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.R)) {          // Rotate active selecion
            OnRotate?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {     // Exit current operation
            OnExit?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.I)) {          // Place selection in inventory
            OnInventory?.Invoke();
        }
    }

    public bool IsPointerOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Called by PlacementManager.cs
    public Vector3 GetGridSelectionPos() {

        Vector3 mousePos = Input.mousePosition;
        
        // Raycast
        Ray ray = _playerCam.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, terrainLayerMask)) {
            lastPos = hit.point;
        }
        
        return lastPos;
    }

    public GameObject GetClickedObject() {

        GameObject clickedObject = null;
        
        Vector3 mousePos = Input.mousePosition;
        
        // Raycast
        Ray ray = _playerCam.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, terrainLayerMask)) {
            lastPos = hit.point;
            clickedObject = hit.transform.root.gameObject;
        }

        return clickedObject;
    }
}
