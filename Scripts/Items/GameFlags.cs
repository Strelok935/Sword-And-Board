using System.Collections.Generic;
using UnityEngine;

public class GameFlags : MonoBehaviour
{
    public static GameFlags Instance;

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // SET flag explicitly true or false
    public void SetFlag(string flag, bool value)
    {
        flags[flag] = value;
    }

    // Read flag (defaults to FALSE if it doesn't exist)
    public bool GetFlag(string flag)
    {
        return flags.TryGetValue(flag, out bool value) && value;
    }

    // Optional: check if flag exists at all
    public bool HasFlag(string flag)
    {
        return flags.ContainsKey(flag);
    }
}
