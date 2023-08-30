using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For adding/deleting objects from the overworld.
public class CreativeMenu : MonoBehaviour
{
    
    [SerializeField]
    private ObjectDatabaseSO database;
    
    [SerializeField]
    private CreateObject[] creatableObjects;

    [SerializeField]
    private GameObject dropdown;

    void Start() {

        for (int i = 0; i < database.objects.Count; i++) {
            int ID = database.objects[i].ID;
            string name = database.objects[i].name;
            GameObject prefab = database.objects[i].prefab;
            creatableObjects[i].SetInfo(ID, prefab, name);
        }

        // Dropdown begins as invisible
        dropdown.SetActive(false);

    }

    // Display dropdown w/ all options
    public void ExpandMenu() {
        dropdown.SetActive(true);
    }
    public void HideMenu() {
        dropdown.SetActive(false);
    }
}

