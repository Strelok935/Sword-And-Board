using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
[ExecuteInEditMode]

public class IdSystem : MonoBehaviour
{
    [ReadOnly, SerializeField] private string _id;
    [SerializeField] private static SerializableDictionary<string, GameObject> idDatabase = new SerializableDictionary<string, GameObject>();

    public string Id => _id;


    private void Awake()
    {
       if(idDatabase == null) idDatabase =
       new SerializableDictionary<string, GameObject>();

        if (idDatabase.ContainsKey(_id)) GenerateNewId();
        if (!idDatabase.ContainsKey(_id))
            idDatabase.Add(_id, this.gameObject);
    }

    private void OnDestroy()
    {
        if (idDatabase.ContainsKey(_id))
        {
            idDatabase.Remove(_id);
        }
    }

    [ContextMenu("Generate New ID")]

    private void GenerateNewId()
    {
        _id = Guid.NewGuid().ToString();
        idDatabase.Add(_id, this.gameObject);
        Debug.Log("ID Database Count: " + idDatabase.Count);

    }
}
