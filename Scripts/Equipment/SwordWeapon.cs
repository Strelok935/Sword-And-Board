using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordWeapon : MonoBehaviour
{
[SerializeField] private WeaponBaseSO swordBaseSO; // Reference to the Sword ScriptableObject
[SerializeField] private Animator swordAnimator; // Reference to the Animator component
[SerializeField] private float attackCooldownDuration = 1.0f; // Cooldown duration for sword attack
[SerializeField] private Slider swordAttackSlider; // Reference to the SwordAttack slider in the HUD
[SerializeField] private CanvasGroup swordAttackSliderCanvasGroup; // CanvasGroup for fading the slider
[SerializeField] private AudioSource attackAudioSource; // AudioSource for attack sound
[SerializeField] private float fadeDelay = 3.0f; // Time before the slider fades away
[SerializeField] private float fadeDuration = 1.0f; // Duration of the fade effect

private bool canAttack = true;
private float cooldownTimer;
private Coroutine fadeCoroutine;

private void Awake()
{
if (swordAnimator == null)
{
Debug.LogError("Sword Animator not assigned!");
}

if (swordAttackSlider == null)
{
Debug.LogError("SwordAttack Slider not assigned!");
}

if (swordAttackSliderCanvasGroup == null)
{
Debug.LogError("SwordAttack Slider CanvasGroup not assigned!");
}

if (attackAudioSource == null)
{
Debug.LogError("Attack AudioSource not assigned!");
}

// Initialize the slider value to max (100%)
swordAttackSlider.value = swordAttackSlider.maxValue;
swordAttackSliderCanvasGroup.alpha = 0; // Start with the slider hidden
}
private void OnDisable()
{
HideSliderImmediately();
}

private void Update()
{
if (!canAttack)
{
cooldownTimer += Time.deltaTime;

// Update the slider value based on the cooldown timer
swordAttackSlider.value = Mathf.Clamp((cooldownTimer / attackCooldownDuration) * swordAttackSlider.maxValue, 0, swordAttackSlider.maxValue);

if (cooldownTimer >= attackCooldownDuration)
{
canAttack = true;
swordAttackSlider.value = swordAttackSlider.maxValue; // Reset slider to max value
}
}
}

public void Attack()
{
if (canAttack)
{
// Trigger the attack animation
if (swordAnimator != null)
{
swordAnimator.SetTrigger("AttackTrigger");
StartCoroutine(ResetAttackTrigger(swordAnimator, attackCooldownDuration));
}

// Play attack sound
if (attackAudioSource != null && swordBaseSO.attackSound != null)
{
attackAudioSource.clip = swordBaseSO.attackSound;
attackAudioSource.Play();
}

// Perform the attack logic
PerformAttack();

// Start cooldown
StartCooldown();

// Show the slider and cancel any ongoing fade coroutine
ShowSlider();
}
else
{
Debug.Log("Sword is on cooldown!");
}
}

private void PerformAttack()
{
// Detect enemies within the sword's range
Collider[] hitColliders = Physics.OverlapSphere(transform.position, swordBaseSO.range);

foreach (Collider hitCollider in hitColliders)
{
if (hitCollider.CompareTag("Enemy"))
{
EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
if (enemyHealth != null)
{
enemyHealth.TakeDamage(swordBaseSO.Damage);
Debug.Log($"Sword hit {hitCollider.name} for {swordBaseSO.Damage} damage.");

// Instantiate hit VFX at the hit point
if (swordBaseSO.hitVFXPrefab != null)
{
GameObject vfxInstance = Instantiate(swordBaseSO.hitVFXPrefab, hitCollider.transform.position, Quaternion.identity);

// Destroy the VFX after its duration
ParticleSystem vfxParticleSystem = vfxInstance.GetComponent<ParticleSystem>();
if (vfxParticleSystem != null)
{
Destroy(vfxInstance, vfxParticleSystem.main.duration);
}
else
{
Destroy(vfxInstance, 2f); // Default destroy time
}
}
}
}
}
}

private void StartCooldown()
{
canAttack = false;
cooldownTimer = 0f;
swordAttackSlider.value = 0; // Reset slider to 0
}

private IEnumerator ResetAttackTrigger(Animator animator, float duration)
{
yield return new WaitForSeconds(duration);
animator.ResetTrigger("AttackTrigger");

// Start fading the slider after a delay
if (fadeCoroutine != null)
{
StopCoroutine(fadeCoroutine);
}
fadeCoroutine = StartCoroutine(FadeSliderOut());
}

private void ShowSlider()
{
if (fadeCoroutine != null)
{
StopCoroutine(fadeCoroutine);
}

swordAttackSliderCanvasGroup.alpha = 1; // Make the slider visible
}

public void FadeSliderImmediately()
{
// Start fading the slider immediately
if (fadeCoroutine != null)
{
StopCoroutine(fadeCoroutine);
}
fadeCoroutine = StartCoroutine(FadeSliderOutImmediate());
}

public void HideSliderImmediately()
{
if (fadeCoroutine != null)
{
StopCoroutine(fadeCoroutine);
}

swordAttackSliderCanvasGroup.alpha = 0; // Ensure the slider is fully hidden
}

private IEnumerator FadeSliderOut()
{
yield return new WaitForSeconds(fadeDelay);

float elapsedTime = 0f;
while (elapsedTime < fadeDuration)
{
elapsedTime += Time.deltaTime;
swordAttackSliderCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
yield return null;
}

swordAttackSliderCanvasGroup.alpha = 0; // Ensure the slider is fully hidden
}

private IEnumerator FadeSliderOutImmediate()
{
float elapsedTime = 0f;
while (elapsedTime < fadeDuration)
{
elapsedTime += Time.deltaTime;
swordAttackSliderCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
yield return null;
}

swordAttackSliderCanvasGroup.alpha = 0; // Ensure the slider is fully hidden
}
}