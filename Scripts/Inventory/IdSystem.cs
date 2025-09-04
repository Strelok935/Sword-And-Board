using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
[ExecuteInEditMode]

public class IdSystem : MonoBehaviour
{
    [ReadOnly, SerializeField] private string _id = Guid.NewGuid().ToString();
    [SerializeField] private static SerializableDictionary<string, GameObject> idDatabase = new SerializableDictionary<string, GameObject>();

    public string Id => _id;


    private void OnValidate()
    {
        if (idDatabase.ContainsKey(_id))
        {
            GenerateNewId();
        }
        else
        {
            idDatabase.Add(_id, this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (idDatabase.ContainsKey(_id))
        {
            idDatabase.Remove(_id);
        }
    }

    private void GenerateNewId()
    {
        _id = Guid.NewGuid().ToString();

    }
}
