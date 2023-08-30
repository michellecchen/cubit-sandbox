using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObjectData : MonoBehaviour
{
    [HideInInspector]
    public ObjectPlacement placementData;
    [HideInInspector]
    public ObjectData objectData;

    // Assigned upon instantiation
    public int instanceID;

    // Outline around object
    private bool isHighlighted = false;
    private Outline outline;
    private Color defaultColor = Color.white;
    private Color selectionColor = Color.red;

    // How much to scale up the object by when selected
    [SerializeField]
    private GameObject scalePivot = null;
    [SerializeField]
    private Vector3 center;
    private float scaleFactor = 1.05f;
    private float scaleTime = 0.05f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 defaultScale;                   // when regular size
    private Vector3 enlargedScale;                  // when enlarged
    [SerializeField]
    private bool isEnlarged = false;

    [SerializeField]
    private Transform cubits;
    [SerializeField]
    private Transform terrain;

    void Start() {

        // Used to help distinguish btwn. two separate instances spawned from the same prefab
        instanceID = GetInstanceID();

        center = transform.position + new Vector3(objectData.size.x*0.5f, objectData.size.y*0.5f, objectData.size.z*0.5f);
        // int objectHeight = objectData.size.y;

        // For scaling
        defaultScale = transform.localScale;
        enlargedScale = defaultScale * scaleFactor;

        cubits = transform.Find("Cubits");
        terrain = transform.Find("Terrain");

    }

    // Visualize an outline (using QuickOutline)
    // Called upon hover
    public void DrawOutline() {
        if (!isHighlighted) {
            if (outline == null) {                                  // on first hover
                outline = gameObject.AddComponent<Outline>();
                outline.OutlineWidth = 8.0f;
            }
            else {                                                  // on repeat hover
                outline.enabled = true;
            }
            isHighlighted = true;
        }
    }

    public void ToggleSelection(bool selected) {
        outline.OutlineColor = selected ? selectionColor : defaultColor;
    }

    // Hide the outline
    // Called when mouse hovers away
    public void HideOutline() {
        if (isHighlighted) {
            if (outline != null) {
                outline.enabled = false;
            }
            isHighlighted = false;
        }
    }

    public void Enlarge() {
        if (scalePivot == null && !isEnlarged) {
            Debug.Log("Enlarging");
            StartCoroutine(ScaleUp());
        }
    }

    public void Shrink() {
        if (scalePivot != null && isEnlarged) {
            Debug.Log("Shrinking");
            StartCoroutine(ScaleDown());
        }
    }

    private IEnumerator ScaleUp() {

        scalePivot = new GameObject("ScalePivot");
        scalePivot.transform.position = center;

        cubits.SetParent(scalePivot.transform, true);
        terrain.SetParent(scalePivot.transform, true);
        scalePivot.transform.SetParent(transform, true);

        // Scale pivot by factor
        if (scalePivot != null) {
            yield return SmoothScale(scalePivot.transform, defaultScale, enlargedScale);
            scalePivot.transform.localScale = enlargedScale;
            isEnlarged = true;
        }
    }

    private IEnumerator ScaleDown() {

        // Start the coroutine in reverse
        yield return SmoothScale(scalePivot.transform, enlargedScale, defaultScale);
        scalePivot.transform.localScale = defaultScale;
        isEnlarged = false;
        
        cubits.SetParent(transform, true);
        terrain.SetParent(transform, true);
        scalePivot.transform.SetParent(null, true);

        // Destroy the scale pivot & reset the reference
        GameObject temp = scalePivot;
        scalePivot = null;
        Destroy(temp);

    }

    // Smooth scaling animation using SmoothDamp()
    // Can scale up or scale down -- just swap initScale & targetScale
    private IEnumerator SmoothScale(Transform target, Vector3 initScale, Vector3 targetScale) {

        // Snap to start value
        target.localScale = initScale;
        
        float elapsedTime = 0.0f;
        while (elapsedTime < scaleTime) {
            target.localScale = Vector3.SmoothDamp(target.localScale, targetScale, ref velocity, scaleTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to final scale value
        target.localScale = targetScale;

    }
}
