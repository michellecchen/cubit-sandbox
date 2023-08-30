using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveCubit : MonoBehaviour
{
    private Renderer _renderer;
    [SerializeField]
    private Material _originalMat;
    // [SerializeField]
    // private InteractiveObject _interactiveParent;

    // [HideInInspector]
    // public bool hasHoverMat;
    // public bool hasSelectionMat;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _originalMat = _renderer.material;

        // _interactiveParent = transform.parent.gameObject.GetComponent<InteractiveObject>();
    }

    // Called by ObjectInteractionManager
    // Communicating updates with all cubit 'siblings' via their shared parent, InteractiveObject

    // public void OnHover() {
    //     _interactiveParent.SetObjectAsHovered();
    // }

    // public void OnSelection() {
    //     _interactiveParent.SetObjectAsSelected();
    // }

    // public void OnReset() {
    //     _interactiveParent.SetObjectAsReset();
    // }

    // Called by InteractiveObject

    public void SetHoverMaterial(Material hoverMat) {

        _renderer.material = hoverMat;

        // hasHoverMat = true;
        // if (hasSelectionMat) hasSelectionMat = false; // necessary? do we ever go from cubit being selected to just hovered? no, right?
    }

    public void SetSelectionMaterial(Material selectionMat) {
        
        _renderer.material = selectionMat;

        // hasSelectionMat = true;
        // if (hasHoverMat) hasHoverMat = false;
    }

    public void ResetMaterial() {

        Debug.Log("Resetting material from InteractiveCubit.cs");

        _renderer.material = _originalMat;

        // if (hasHoverMat) hasHoverMat = false;
        // if (hasSelectionMat) hasSelectionMat = false;
    }
}
