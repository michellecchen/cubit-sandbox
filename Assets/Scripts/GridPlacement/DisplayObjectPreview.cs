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

        cellIndicator.SetActive(false);                                     // Start with invisible indicator
        cellRenderer = cellIndicator.GetComponentInChildren<Renderer>();    // Set a reference to its renderer

    }

    // Display placement preview
    public void DisplayPreview(GameObject objectPrefab, Vector3Int objectSize) {

        objectPreview = Instantiate(objectPrefab);

        // Assign preview material to object preview
        Renderer[] renderers = objectPreview.GetComponentsInChildren<Renderer>();
        foreach (Renderer cubitRenderer in renderers) {
            cubitRenderer.material = _matInstance;
        }

        if (useCellIndicator) {
            DisplayCellIndicator(objectSize);
        }
    }

    public void HidePreview() {
        
        if (objectPreview != null) {
            Destroy(objectPreview);
        }

        cellIndicator.SetActive(false);
    }

    // Update positions of object & cell previews
    public void UpdatePreview(Vector3 placementPos, bool legalPlacement) {
        
        // Update object preview -- if it exists
        if (objectPreview != null) {
            UpdateObjectPreviewPosition(placementPos);
            UpdateObjectPreviewColor(legalPlacement);
        }

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
            child.SetParent(rotationPivot.transform, true);             // maintain world position
        }

        return rotationPivot;
        
    }

    private void RemoveRotationPivotFromObject(GameObject rotationPivot, Transform objectTransform) {
        foreach (Transform child in rotationPivot.transform) {
            child.SetParent(objectTransform, true);                     // maintain world position
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

    // Position updates

    private void UpdateObjectPreviewPosition(Vector3 placementPos) {
        objectPreview.transform.position = new Vector3(placementPos.x, placementPos.y + yOffset, placementPos.z);
    }

    private void UpdateCellIndicatorPosition(Vector3 placementPos) {
        cellIndicator.transform.position = placementPos;
    }

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

    // Display a preview over the cells that the previewed object would be placed onto
    private void DisplayCellIndicator(Vector3Int objectSize) {

        // Adjust scale & tiling of the indicator
        if (objectSize.x > 0 || objectSize.z > 0) {             // just a trivial check

            cellIndicator.transform.localScale = new Vector3(objectSize.x, 1, objectSize.z);

            // Adjust tiling of texture to reflect previewed object's size
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
