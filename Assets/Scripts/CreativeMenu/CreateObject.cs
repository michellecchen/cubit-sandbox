using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateObject : MonoBehaviour
{

    public int objectID = -1;          // ID in database
    public string objectName;          // name
    public GameObject objectIcon;      // 3D icon

    // To be assigned either in the Inspector or at start:
    [SerializeField]
    private Transform iconParent;
    private GameObject icon;
    [SerializeField]
    private TMP_Text nameText;

    [SerializeField]
    private LayerMask renderLayer;

    void Start() {
        // In case references to UI child elements are unassigned in Inspector
        if (iconParent == null) {
            iconParent = transform.Find("Icon").GetChild(0);
        }
        if (nameText == null) {
            iconParent = transform.Find("Text (TMP)");
        }
        renderLayer = LayerMask.NameToLayer("RenderTexture");
    }

    // on startup
    public void SetInfo(int ID, GameObject prefab, string name) {
        
        objectID = ID;
        objectIcon = prefab;
        objectName = name;

        // set name
        nameText.text = objectName;
        // create 3D icon
        icon = Instantiate(prefab, iconParent, false);
        icon.layer = renderLayer;
        Transform cubits = icon.transform.Find("Cubits");       // find child named 'Cubits'
        foreach (Transform cubit in cubits) {
            cubit.gameObject.layer = renderLayer;
        }

    }
}
