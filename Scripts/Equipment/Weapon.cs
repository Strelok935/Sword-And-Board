// 12/2/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private ParticleSystem MuzzleFlash;
    [SerializeField] private AudioSource audioSource;

    public void Shooting(WeaponBaseSO weaponBaseSo, RaycastHit? hit = null)
    {
        // Play muzzle flash and sound regardless of hit
        MuzzleFlash.Play();
        audioSource.clip = weaponBaseSo.attackSound; // Assign the attack sound to the AudioSource
        audioSource.Play(); // Play the sound

        if (hit.HasValue)
        {
            // If the hit object is an enemy, apply damage
            if (hit.Value.collider.gameObject.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.Value.collider.GetComponent<EnemyHealth>();
                enemyHealth?.TakeDamage(weaponBaseSo.Damage);
            }

            // Instantiate hit VFX at the hit point
            GameObject vfxInstance = Instantiate(weaponBaseSo.hitVFXPrefab, hit.Value.point, Quaternion.identity);

            // Destroy the VFX after its duration
            ParticleSystem vfxParticleSystem = vfxInstance.GetComponent<ParticleSystem>();
            if (vfxParticleSystem != null)
            {
                Destroy(vfxInstance, vfxParticleSystem.main.duration);
            }
            else
            {
                // If no ParticleSystem is found, destroy after a default time
                Destroy(vfxInstance, 2f);
            }

            Debug.Log($"Hit object: {hit.Value.collider.name}");
        }
        else
        {
            Debug.Log("No hit detected.");
        }
    }
}