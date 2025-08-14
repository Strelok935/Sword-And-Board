using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseItemData : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemCountText;


    private void Awake()
    {
        itemImage.color = Color.clear;
        itemCountText.text = string.Empty;
    }
}
