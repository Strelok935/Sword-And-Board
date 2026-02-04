using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Weapon : MonoBehaviour
{
    [SerializeField] private ParticleSystem MuzzleFlash;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource dryFireAudioSource; // Second AudioSource for dry-firing sound
    [SerializeField] private bool usesAmmo; // Flag to determine if the weapon uses ammo
    [SerializeField] private bool hasMuzzleFlash; // Flag to determine if the weapon has a muzzle flash
    [SerializeField] private ItemInventoryData ammoType; // The type of ammo required
    [SerializeField] private int ammoPerShot = 1; // Amount of ammo required per shot
    [SerializeField] private Animator weaponAnimator; // Reference to the Animator component
    [SerializeField] private float attackAnimationDuration = 1.208333f; // Duration of the attack animation


    private InventoryHolder playerInventoryHolder; // Reference to the player's inventory holder

    private void Awake()
    {
        // Find the player's inventory holder in the scene
        playerInventoryHolder = FindObjectOfType<InventoryHolder>();
        if (playerInventoryHolder == null)
        {
            Debug.LogError("Player InventoryHolder not found in the scene!");
        }
    }

    public void Shooting(WeaponBaseSO weaponBaseSo, RaycastHit? hit = null)
    {
        if (usesAmmo)
        {
            if (ammoType == null)
            {
                Debug.LogError("Weapon is set to use ammo, but no ammo type is assigned.");
                PlayDryFireSound();
                return;
            }

            int ammoNeeded = Mathf.Max(1, ammoPerShot);

            // Check if the player has enough ammo
            if (playerInventoryHolder.InventorySystem.ContainsItem(ammoType, out List<InventorySlot> ammoSlots))
            {
                int totalAmmo = ammoSlots.Sum(slot => Mathf.Max(0, slot.StackSize));

                if (totalAmmo >= ammoNeeded)
                {
                    // Deduct ammo and fire the weapon
                    playerInventoryHolder.InventorySystem.RemoveItemsFromInventory(ammoType, ammoNeeded);
                    FireWeapon(weaponBaseSo, hit);
                }
                else
                {
                    // Play dry-firing sound if not enough ammo
                    PlayDryFireSound();
                }
            }
            else
            {
                // Play dry-firing sound if no ammo found
                PlayDryFireSound();
            }
        }
        else
        {
            // Fire the weapon without ammo check
            FireWeapon(weaponBaseSo, hit);
        }
    }

    private void FireWeapon(WeaponBaseSO weaponBaseSo, RaycastHit? hit)
    {
        // Play muzzle flash if the weapon has it
        if (hasMuzzleFlash && MuzzleFlash != null)
        {
            MuzzleFlash.Play();
        }

        // Play the firing sound
        audioSource.PlayOneShot(weaponBaseSo.attackSound);
        // Trigger the attack animation
        if (weaponAnimator != null)
        {
            weaponAnimator.SetTrigger("AttackTrigger");

            // Reset the AttackTrigger after the animation finishes
            StartCoroutine(ResetAttackTrigger(weaponAnimator, attackAnimationDuration));
        }

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

    public float GetAttackAnimationDuration()
    {
    return attackAnimationDuration;
    }

        private void PlayDryFireSound()
    {
        if (usesAmmo && dryFireAudioSource != null && dryFireAudioSource.enabled)
        {
            // STOP any real firing sound first
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();

            dryFireAudioSource.Play();
        }
        else if (usesAmmo)
        {
            Debug.LogWarning("Dry-fire sound is not assigned or the AudioSource is disabled!");
        }
    }


    private System.Collections.IEnumerator ResetAttackTrigger(Animator animator, float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.ResetTrigger("AttackTrigger");
    }
}
