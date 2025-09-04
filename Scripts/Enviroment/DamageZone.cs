// 7/16/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerSecond = 10f; // Damage dealt per second

    // Optional: Add visual or audio effects for the damage zone
    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // Red color for the damage zone
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
