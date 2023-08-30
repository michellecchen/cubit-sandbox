using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayObjectPreview : MonoBehaviour
{

    public Color legalPlacementColor;
    public Color illegalPlacementColor;
    public float objectPreviewAlpha;

    [SerializeField]
    private float yOffset = 0.05f;

    // Cell placement indicator
    [SerializeField]
    private GameObject cellIndicator;
    private Renderer cellRenderer;
    
    private GameObject objectPreview;

    [SerializeField]
    private Material previewMat;
    private Material _matInstance;

    private GameObject objectRotationPivot;
    private GameObject cellRotationPivot;

    [Header("Use the cell indicator display?")]
    public bool useCellIndicator;

    void Start() {

        // Create a material instance so that the original material data remains unaltered
        _matInstance = new Material(previewMat);

        // Start with invisible indicator
        cellIndicator.SetActive(false);
        // Set a reference to its renderer
        cellRenderer = cellIndicator.GetComponentInChildren<Renderer>();

    }

    // Display placement preview
    public void DisplayPreview(GameObject objectPrefab, Vector3Int objectSize) {

        objectPreview = Instantiate(objectPrefab);
        
        // // Enable outline
        // Outline previewOutline = objectPreview.GetComponentInChildren<Outline>();
        // previewOutline.OutlineColor = legalPlacementColor;

        // Assign preview material to object preview
        Renderer[] renderers = objectPreview.GetComponentsInChildren<Renderer>();
        foreach (Renderer cubitRenderer in renderers) {
            cubitRenderer.material = _matInstance;
        }

        if (useCellIndicator) {
            DisplayCellIndicator(objectSize);
        }
        // DisplayCellIndicator(objectSize);
    }

    public void HidePreview() {
        
        if (objectPreview != null) {
            Destroy(objectPreview);
        }

        // if (rotationPivot != null) {
        //     cellIndicator.transform.SetParent(null, true);
        //     cellIndicator.transform.rotation = Quaternion.identity;
        //     Destroy(rotationPivot);
        //     Debug.Log("Destroying rotation pivot in HidePreview()");
        // } else {
        //     Debug.Log("rotation pivot null, did not destroy pivot");
        // }

        // if (objectRotationPivot != null) {
        //     RemoveRotationPivotFromObject(objectRotationPivot, objectPreview.transform);
        // }

        // if (cellRotationPivot != null) {
        //     RemoveRotationPivotFromObject(cellRotationPivot, cellIndicator.transform);
        //     // Undo all rotations from cell indicator
        //     cellIndicator.transform.GetChild(0).rotation = Quaternion.identity;
        // }

        cellIndicator.SetActive(false);
    }

    // Update positions of object & cell previews
    public void UpdatePreview(Vector3 placementPos, bool legalPlacement) {
        
        // Update object preview -- if it exists
        if (objectPreview != null) {
            UpdateObjectPreviewPosition(placementPos);
            UpdateObjectPreviewColor(legalPlacement);
        }

        // // Update cell indicator preview
        // UpdateCellIndicatorPosition(placementPos);
        // UpdateCellIndicatorColor(legalPlacement);
        if (useCellIndicator) {
            UpdateCellIndicatorPosition(placementPos);
            UpdateCellIndicatorColor(legalPlacement);
        }

    }

    // Returns true if rotation was successful
    public bool UpdatePreviewRotation() {
        if (objectPreview != null) {
            RotateAroundPivot();
            return true;
        }
        return false;
    }

    private GameObject CreateRotationPivotForObject(Transform objectTransform) {

        Transform[] childrenToRotate = new Transform[objectTransform.childCount];
        for (int i = 0; i < objectTransform.childCount; i++) {
            childrenToRotate[i] = objectTransform.GetChild(i);
        }

        GameObject rotationPivot = new GameObject("RotationPivot");
        rotationPivot.transform.SetParent(objectTransform, false);
        rotationPivot.transform.Translate(new Vector3(0.5f, 0.0f, 0.5f));

        foreach (Transform child in childrenToRotate) {
            child.SetParent(rotationPivot.transform, true);          // preserve world position
        }

        return rotationPivot;
        
    }

    private void RemoveRotationPivotFromObject(GameObject rotationPivot, Transform objectTransform) {
        foreach (Transform child in rotationPivot.transform) {
            child.SetParent(objectTransform, true);             // maintain world position
        }
        if (rotationPivot.transform.childCount == 0) {
            Debug.Log("All children safely removed from pivot");
            Destroy(rotationPivot);
        }
    }

    private void RotateAroundPivot() {

        if (objectRotationPivot == null) {
            objectRotationPivot = CreateRotationPivotForObject(objectPreview.transform);
        }
        // if (cellRotationPivot == null) {
        //     cellRotationPivot = CreateRotationPivotForObject(cellIndicator.transform);
        // }

        // Rotate both pivots by 90 deg. around y-axis
        objectRotationPivot.transform.Rotate(Vector3.up, 90.0f);
        // cellRotationPivot.transform.Rotate(Vector3.up, 90.0f);

    }

    // // Rotate the object preview AND cell indicator by 90 deg. around the y-axis
    // private void RotateAroundPivot() {

    //     if (rotationPivot == null) {

    //         // Create a rotation pivot; place it at the bottom-center of the object (preview)'s origin
    //         //      (which, by default, is at the bottom-left corner of the shape)
    //         rotationPivot = new GameObject("ObjectPreview_RotationPivot");
            
    //         // Vector3 pivotPos = objectPreview.transform.position + new Vector3(0.5f, 0.0f, 0.5f);
    //         // rotationPivot.position = new Vector3(objectPreview.transform.position.x + 0.5f, objectPreview.transform.position.y, objectPreview.transform.position.z + 0.5f);
            
    //         rotationPivot.transform.SetParent(objectPreview.transform, false);

    //         Vector3 pivotTranslation = new Vector3(0.5f, 0.0f, 0.5f);
    //         rotationPivot.transform.Translate(pivotTranslation);

    //         // Parent everything else to rotation pivot
    //         foreach (Transform child in objectPreview.)
            
    //         // rotationPivot.transform.position = pivotPos;
    //         Debug.Log("Created new rotation pivot");
    //     }

    //     // // Parent the object preview to the pivot
    //     // objectPreview.transform.parent = rotationPivot.transform;
    //     // // Parent the cell indicator to the pivot
    //     // cellIndicator.transform.parent = rotationPivot.transform;

    //     objectPreview.transform.SetParent(rotationPivot.transform, true);
    //     cellIndicator.transform.SetParent(rotationPivot.transform, true);

    //     // Rotate the pivot by 90 deg. around y-axis
    //     rotationPivot.transform.Rotate(Vector3.up, 90.0f);
    //     // Debug.Log(rotationPivot.transform.rotation);

    //     // Vector3 tempPosition = objectPreview.transform.position;
    //     // Debug.Log(tempPosition);
    //     // // should be same for cellIndicator
    //     // objectPreview.transform.SetParent(null, true);
    //     // objectPreview.transform.position = tempPosition;
    //     // cellIndicator.transform.SetParent(null, true);
    //     // cellIndicator.transform.position = tempPosition;

    //     // // Unparent the object preview from the pivot
    //     // objectPreview.transform.parent = null;
    //     // // Unparent the cell indicator from the pivot
    //     // cellIndicator.transform.parent = null;

    //     // objectPreview.transform.SetParent(null, false);
    //     // cellIndicator.transform.SetParent(null, false);

    //     // Destroy the rotation pivot, now that we're done with it
    //     // if (rotationPivot != null) Destroy(rotationPivot);
    // }

    // Position updates

    private void UpdateObjectPreviewPosition(Vector3 placementPos) {
        objectPreview.transform.position = new Vector3(placementPos.x, placementPos.y + yOffset, placementPos.z);
    }

    private void UpdateCellIndicatorPosition(Vector3 placementPos) {
        cellIndicator.transform.position = placementPos;
    }

    // private void UpdatePreviewPositions(Vector3 placementPos, bool legalPlacement) {
    //     cellIndicator.transform.position = placementPos;                    // update cell(s) preview
    //     objectPreview.transform.position = new Vector3(placementPos.x, placementPos.y + yOffset, placementPos.z);
    // }

    // Color updates

    private void UpdateObjectPreviewColor(bool legalPlacement) {
        Color previewColor = legalPlacement ? legalPlacementColor : illegalPlacementColor;
        previewColor.a = objectPreviewAlpha;
        _matInstance.color = previewColor;
    }

    private void UpdateCellIndicatorColor(bool legalPlacement) {
        Color previewColor = legalPlacement ? Color.white : Color.red;
        cellRenderer.material.color = previewColor;  
    }

    // private void UpdatePreviewColors(bool legalPlacement) {
    //     Color previewColor = legalPlacement ? Color.white : Color.red;
    //     cellRenderer.material.color = previewColor;                         // update cell(s) preview
    //     previewColor.a = 0.5f;
    //     _matInstance.color = previewColor;                                  // update object preview
    // }

    // Display a preview over the cells that the previewed object would be placed onto
    private void DisplayCellIndicator(Vector3Int objectSize) {

        // Adjust scale & tiling of the indicator
        if (objectSize.x > 0 || objectSize.z > 0) { // just a trivial check

            cellIndicator.transform.localScale = new Vector3(objectSize.x, 1, objectSize.z);

            // Adjust tiling of texture to reflect previewed object's size
            // cellRenderer.material.mainTextureScale = objectSize;
            cellRenderer.material.mainTextureScale = new Vector2(objectSize.x, objectSize.z);
        }

        // Display the indicator
        cellIndicator.SetActive(true);
    }

    public void DisplayRemovalPreview() {
        // cellIndicator.SetActive(true);
        // DisplayCellIndicator(Vector3Int.one);
        // UpdateCellIndicatorColor(false);        // default to red
    }

}
