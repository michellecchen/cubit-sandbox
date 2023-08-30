using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectDatabaseSO : ScriptableObject
{
    public List<ObjectData> objects;
}

// Data for a single object, made accessible — and exclusively editable — via the Inspector
[Serializable]
public class ObjectData {

    // Data properties

    [field: SerializeField]
    public string name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public Vector3Int size { get; private set; } = Vector3Int.one;

    [field: SerializeField]
    public GameObject prefab { get; private set; }

    // // new: inventory
    // [field: SerializeField]
    // public Texture icon {get; private set; }
}