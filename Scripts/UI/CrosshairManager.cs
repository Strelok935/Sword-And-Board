// 7/24/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    public enum CrosshairState
    {
        Normal,
        Interactable,
        Item
    }

    [Header("Crosshair Sprites")]
    public Sprite normalCrosshair;
    public Sprite interactableCrosshair;
    public Sprite itemCrosshair;

    [Header("Crosshair UI")]
    public Image crosshairImage;

    private CrosshairState currentState = CrosshairState.Normal;

    public void SetCrosshairState(CrosshairState state)
    {
        if (currentState == state) return; // Avoid unnecessary updates

        currentState = state;

        switch (state)
        {
            case CrosshairState.Normal:
                crosshairImage.sprite = normalCrosshair;
                break;
            case CrosshairState.Interactable:
                crosshairImage.sprite = interactableCrosshair;
                break;
            case CrosshairState.Item:
                crosshairImage.sprite = itemCrosshair;
                break;
        }
    }
}
