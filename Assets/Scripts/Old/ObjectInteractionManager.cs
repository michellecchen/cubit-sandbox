using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectInteractionManager : MonoBehaviour
{

    [Header("Materials to indicate cursor hover & selection")]
    public Material hoverMat;
    public Material selectionMat;
    
    [SerializeField]
    private Transform _currentHover;
    [SerializeField]
    private Transform _currentSelection;
    private RaycastHit _raycastHit;

    private Camera _playerCam;

    // Layer mask for interactive objects
    private int _layerMask = 1 << 6;

    // Start is called before the first frame update
    void Start()
    {
        _playerCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        // TODO
        // Arguably could combine these two parts if thinking of them as one logic chain makes sense to you AND has justifiable efficiency.

        // PT. 1: handling hovers
    
        // if there's an existing hover --> reset
        // if (_currentHover != null) {

        //     _currentHover.gameObject.GetComponent<InteractiveObject>().SetObjectAsReset();      // reset to original material
        //     _currentHover = null;                                                               // clear the reference
        // }

        Ray ray = _playerCam.ScreenPointToRay(Input.mousePosition);

        // verify that the cursor ISN'T over a UI element
        // then check if the raycast has captured a gameobject
        // is the UI check necessary if we use layer masks? probably...?
        if (Physics.Raycast(ray, out _raycastHit, 1000f, _layerMask) && !EventSystem.current.IsPointerOverGameObject()) {

            Transform hovered = _raycastHit.transform;
            _currentHover = hovered.parent;

            // if we're currently hovering over an interactive object
            // AND we haven't yet selected this object

            if (_currentHover != null && _currentHover != _currentSelection) {

                _currentHover.gameObject.GetComponent<InteractiveObject>().SetObjectAsHovered(hoverMat);

                // if we've yet to apply hover mat to all cubits of this object grouping
                // if (!_currentHover.gameObject.GetComponent<InteractiveObject>().appliedHoverMat) {

                //     // set hover material
                //     _currentHover.gameObject.GetComponent<InteractiveObject>().SetObjectAsHovered(hoverMat);
                // }
            }
            else {
                // drop the reference
                // (don't need _currentHover...SetObjectAsReset()?)
                if (_currentHover != null) {
                    // _currentHover.gameObject.GetComponent<InteractiveObject>().SetObjectAsReset();
                    _currentHover = null;
                }
            }
        }
        else {
            if (_currentHover != null) {
                Debug.Log("Clear hover");
                _currentHover.gameObject.GetComponent<InteractiveObject>().SetObjectAsReset();
                _currentHover = null;
            }
        }

        // PT. 2: handling selection
        // Players select an interactive object w/ LMB-click

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            
            // clicked an InteractiveCubit
            if (Physics.Raycast(ray, out _raycastHit, 1000f, _layerMask)) {

                Debug.Log("Clicked on a cubit");

                Transform selected = _raycastHit.transform;

                if (_currentSelection == null) {
                    // select new cube
                    Debug.Log("No prev. selection; we make a new selection.");
                    _currentSelection = selected.parent;
                    _currentSelection.gameObject.GetComponent<InteractiveObject>().SetObjectAsSelected(selectionMat);
                } else {

                    // Deselect prev. selection
                    _currentSelection.gameObject.GetComponent<InteractiveObject>().SetObjectAsReset();
                    // clear reference if we clicked into our previous selection
                    if (_currentSelection == selected.parent) {
                        _currentSelection = null;
                        // ** also clear hover..?
                    }
                    // else, if we clicked into a new selection... simply update the ref
                    else {
                        _currentSelection = selected.parent;
                        _currentSelection.gameObject.GetComponent<InteractiveObject>().SetObjectAsSelected(selectionMat);
                    }

                    // Debug.Log("There was a prev. selection; we clear that selection.");
                    // // deselect the prev. selection
                    // _currentSelection.gameObject.GetComponent<InteractiveObject>().SetObjectAsReset();
                    // _currentSelection = null;
                    // // if we clicked into a new selection:
                    // if (_currentSelection != null && _currentSelection != selected.parent) {
                    //     Debug.Log("Clicked into a new selection (after clearing prev. selection). We make a new selection.");
                    //     _currentSelection = selected.parent;
                    //     _currentSelection.gameObject.GetComponent<InteractiveObject>().SetObjectAsSelected(selectionMat);
                    // }
                }
            }

            // didn't click an InteractiveCubit (i.e. clicked an overworld, non-UI element, or empty space)
            else {
                if (_currentSelection != null) {
                    Debug.Log("Clicked overworld/empty space and there was a prev. selection; we clear that selection.");
                    _currentSelection.gameObject.GetComponent<InteractiveObject>().SetObjectAsReset();
                    _currentSelection = null;
                }
            }
        }
    }
}
