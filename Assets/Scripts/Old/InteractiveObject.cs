using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{

    // private ObjectInteractionManager _interactionManager;

    public bool activeSelection;

    public bool appliedHoverMat;
    public bool appliedSelectionMat;

    private Camera _playerCam;
    
    // Offset between this transform's position & mouse's world position
    // Calculated when initially selected
    // private Vector3 _offset;
    private Vector3 _initMousePos;
    private Vector3 _initObjectPos;

    // private Material _hoverMat;
    // private Material _selectionMat;

    // Cubit operating as a pivot for dragging this object grouping
    // public Transform selectionPivot;

    void Start() {

        // _interactionManager = GameObject.FindObjectOfType<ObjectInteractionManager>();
        // if (_interactionManager == null) {
        //     Debug.Log("ERROR: No central interaction manager found!");
        // }

        _playerCam = Camera.main;

        activeSelection = false;

        appliedHoverMat = false;
        appliedSelectionMat = false;

        // if (_interactionManager != null) {
        //     _hoverMat = _interactionManager.hoverMat;
        //     _selectionMat = _interactionManager.selectionMat;
        // }
    }

    // position not updating as quickly as mouse moves..?
    // use fixedupdate instead?
    void Update() {

        if (activeSelection) {
            
            // updated position = mouse world position + offset
            // **need to also recalculate offset

            // Calculate offset between initial mouse position and current mouse position
            // Vector3 offset = GetMouseWorldPos() - _initMousePos;
            Vector3 offset = _initMousePos - GetMouseWorldPos();

            // Apply offset to initial object position to ensure that object follows cursor as cursor moves
            transform.position = _initObjectPos + offset;
            // Vector3 updatedPos = GetMouseWorldPos() + _offset;
            // transform.position = new Vector3(updatedPos.x, updatedPos.y, transform.position.z);
            
        }
    }

    // void Update() {

    //     // If this object grouping is actively selected, then move it with the cursor -- by the selection pivot (aka, the specific cubit that was clicked).
    //     if (activeSelection) {
    //         transform.position = 
    //     }
    // }

    // Called by InteractiveCubit, when individual cubit belonging to this object grouping is hovered/selected/reset
    // Or by ObjectInteractionManager, when this object is already... (idk?)

    // A parent of a set of InteractiveCubits
    public void SetObjectAsHovered(Material hoverMat) {

        foreach (Transform childCubit in transform) {
            // childCubit.gameObject.GetComponent<InteractiveCubit>().SetHoverMaterial(_interactionManager.hoverMat);
            childCubit.gameObject.GetComponent<InteractiveCubit>().SetHoverMaterial(hoverMat);
        }

        appliedHoverMat = true;
    }

    public void SetObjectAsSelected(Material selectionMat) {

        Debug.Log("Set object as selected triggered in InteractiveObject.cs");

        foreach (Transform childCubit in transform) {
            // childCubit.gameObject.GetComponent<InteractiveCubit>().SetSelectionMaterial(_interactionManager.selectionMat);
            childCubit.gameObject.GetComponent<InteractiveCubit>().SetSelectionMaterial(selectionMat);
        }

        activeSelection = true;
        // _offset = transform.position - GetMouseWorldPos();
        _initMousePos = GetMouseWorldPos();
        _initObjectPos = transform.position;

        appliedSelectionMat = true;
        if (appliedHoverMat) appliedHoverMat = false;
    }

    public void SetObjectAsReset() {

        Debug.Log("Set object as reset triggered in InteractiveObject.cs");

        foreach (Transform childCubit in transform) {
            childCubit.gameObject.GetComponent<InteractiveCubit>().ResetMaterial();
        }

        if (activeSelection) {
            Debug.Log("Reset; clearing active selection from InteractiveObject.cs");
            activeSelection = false;
        }

        if (appliedHoverMat) {
            Debug.Log("Applied hover mat was previously true but is now going to be false");
            appliedHoverMat = false;
        }
        if (appliedSelectionMat) appliedSelectionMat = false;
    }

    private Vector3 GetMouseWorldPos() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -_playerCam.transform.position.z; // why?
        return _playerCam.ScreenToWorldPoint(mousePos);
    }

    // private Vector3 GetMouseWorldPos() {
    //     return _playerCam.ScreenToWorldPoint(Input.mousePosition);
    // }

    // // offset between mouse's world-space position & this transform, when it's the active selection
    // private Vector3 CalculateMousePosOffset(Vector3 mousePos) {
    //     // return transform.position - GetMouseWorldPos();
    //     return transform.position - mousePos;
    // }

}
