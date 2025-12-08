using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/WeaponBaseSO")]

public class WeaponBaseSO : ScriptableObject
{
    public GameObject hitVFXPrefab;
    [SerializeField] public float range = 10f;
    [SerializeField] public AudioClip attackSound;
    public int Damage = 10;
    public float fireRate = 2f;





}