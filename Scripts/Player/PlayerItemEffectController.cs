using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemEffectController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerStats playerStats;

    private MovementBaseStats baseStats;
    private readonly List<ActiveEffect> activeEffects = new List<ActiveEffect>();

    private void Awake()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }
    }

    private void Start()
    {
        CacheBaseStats();
    }

    public bool TryApplyItem(ItemInventoryData itemData)
    {
        if (itemData == null || !itemData.HasUsableEffect())
        {
            return false;
        }

        bool applied = false;

        if (playerStats != null)
        {
            if (itemData.useEffect.healAmount != 0f)
            {
                playerStats.RestoreHealth(itemData.useEffect.healAmount);
                applied = true;
            }

            if (itemData.useEffect.staminaAmount != 0f)
            {
                playerStats.RestoreStamina(itemData.useEffect.staminaAmount);
                applied = true;
            }
        }

        if (playerMovement != null && itemData.useEffect.HasMovementChanges())
        {
            ApplyTimedEffect(itemData.useEffect);
            applied = true;
        }

        return applied;
    }

    private void CacheBaseStats()
    {
        if (playerMovement == null)
        {
            return;
        }

        baseStats = new MovementBaseStats
        {
            speed = playerMovement.speed,
            sprintSpeed = playerMovement.sprintSpeed,
            crouchSpeed = playerMovement.crouchSpeed,
            jumpHeight = playerMovement.jumpHeight,
            gravity = playerMovement.gravity,
            mouseSensitivity = playerMovement.mouseSensitivity,
            crouchTransitionSpeed = playerMovement.crouchTransitionSpeed,
            ladderClimbSpeed = playerMovement.ladderClimbSpeed,
            slideSpeed = playerMovement.slideSpeed,
            leanSpeed = playerMovement.leanSpeed,
            leanAngle = playerMovement.leanAngle
        };
    }

    private void ApplyTimedEffect(ItemUseEffect effect)
    {
        var active = new ActiveEffect(effect);
        activeEffects.Add(active);
        RecalculateMovementStats();
        StartCoroutine(RemoveEffectAfterDuration(active));
    }

    private IEnumerator RemoveEffectAfterDuration(ActiveEffect active)
    {
        yield return new WaitForSeconds(active.DurationSeconds);
        activeEffects.Remove(active);
        RecalculateMovementStats();
    }

    private void RecalculateMovementStats()
    {
        if (playerMovement == null)
        {
            return;
        }

        float speedAdd = 0f;
        float speedMultiplier = 1f;
        float sprintSpeedAdd = 0f;
        float sprintSpeedMultiplier = 1f;
        float crouchSpeedAdd = 0f;
        float crouchSpeedMultiplier = 1f;
        float jumpHeightAdd = 0f;
        float jumpHeightMultiplier = 1f;
        float gravityAdd = 0f;
        float gravityMultiplier = 1f;
        float mouseSensitivityAdd = 0f;
        float mouseSensitivityMultiplier = 1f;
        float crouchTransitionSpeedAdd = 0f;
        float crouchTransitionSpeedMultiplier = 1f;
        float ladderClimbSpeedAdd = 0f;
        float ladderClimbSpeedMultiplier = 1f;
        float slideSpeedAdd = 0f;
        float slideSpeedMultiplier = 1f;
        float leanSpeedAdd = 0f;
        float leanSpeedMultiplier = 1f;
        float leanAngleAdd = 0f;
        float leanAngleMultiplier = 1f;

        foreach (var activeEffect in activeEffects)
        {
            speedAdd += activeEffect.SpeedAdd;
            speedMultiplier *= activeEffect.SpeedMultiplier;
            sprintSpeedAdd += activeEffect.SprintSpeedAdd;
            sprintSpeedMultiplier *= activeEffect.SprintSpeedMultiplier;
            crouchSpeedAdd += activeEffect.CrouchSpeedAdd;
            crouchSpeedMultiplier *= activeEffect.CrouchSpeedMultiplier;
            jumpHeightAdd += activeEffect.JumpHeightAdd;
            jumpHeightMultiplier *= activeEffect.JumpHeightMultiplier;
            gravityAdd += activeEffect.GravityAdd;
            gravityMultiplier *= activeEffect.GravityMultiplier;
            mouseSensitivityAdd += activeEffect.MouseSensitivityAdd;
            mouseSensitivityMultiplier *= activeEffect.MouseSensitivityMultiplier;
            crouchTransitionSpeedAdd += activeEffect.CrouchTransitionSpeedAdd;
            crouchTransitionSpeedMultiplier *= activeEffect.CrouchTransitionSpeedMultiplier;
            ladderClimbSpeedAdd += activeEffect.LadderClimbSpeedAdd;
            ladderClimbSpeedMultiplier *= activeEffect.LadderClimbSpeedMultiplier;
            slideSpeedAdd += activeEffect.SlideSpeedAdd;
            slideSpeedMultiplier *= activeEffect.SlideSpeedMultiplier;
            leanSpeedAdd += activeEffect.LeanSpeedAdd;
            leanSpeedMultiplier *= activeEffect.LeanSpeedMultiplier;
            leanAngleAdd += activeEffect.LeanAngleAdd;
            leanAngleMultiplier *= activeEffect.LeanAngleMultiplier;
        }

        playerMovement.speed = (baseStats.speed + speedAdd) * speedMultiplier;
        playerMovement.sprintSpeed = (baseStats.sprintSpeed + sprintSpeedAdd) * sprintSpeedMultiplier;
        playerMovement.crouchSpeed = (baseStats.crouchSpeed + crouchSpeedAdd) * crouchSpeedMultiplier;
        playerMovement.jumpHeight = (baseStats.jumpHeight + jumpHeightAdd) * jumpHeightMultiplier;
        playerMovement.gravity = (baseStats.gravity + gravityAdd) * gravityMultiplier;
        playerMovement.mouseSensitivity = (baseStats.mouseSensitivity + mouseSensitivityAdd) * mouseSensitivityMultiplier;
        playerMovement.crouchTransitionSpeed = (baseStats.crouchTransitionSpeed + crouchTransitionSpeedAdd) * crouchTransitionSpeedMultiplier;
        playerMovement.ladderClimbSpeed = (baseStats.ladderClimbSpeed + ladderClimbSpeedAdd) * ladderClimbSpeedMultiplier;
        playerMovement.slideSpeed = (baseStats.slideSpeed + slideSpeedAdd) * slideSpeedMultiplier;
        playerMovement.leanSpeed = (baseStats.leanSpeed + leanSpeedAdd) * leanSpeedMultiplier;
        playerMovement.leanAngle = (baseStats.leanAngle + leanAngleAdd) * leanAngleMultiplier;
    }

    private struct MovementBaseStats
    {
        public float speed;
        public float sprintSpeed;
        public float crouchSpeed;
        public float jumpHeight;
        public float gravity;
        public float mouseSensitivity;
        public float crouchTransitionSpeed;
        public float ladderClimbSpeed;
        public float slideSpeed;
        public float leanSpeed;
        public float leanAngle;
    }

    private class ActiveEffect
    {
        public float DurationSeconds { get; }
        public float SpeedMultiplier { get; }
        public float SpeedAdd { get; }
        public float SprintSpeedMultiplier { get; }
        public float SprintSpeedAdd { get; }
        public float CrouchSpeedMultiplier { get; }
        public float CrouchSpeedAdd { get; }
        public float JumpHeightMultiplier { get; }
        public float JumpHeightAdd { get; }
        public float GravityMultiplier { get; }
        public float GravityAdd { get; }
        public float MouseSensitivityMultiplier { get; }
        public float MouseSensitivityAdd { get; }
        public float CrouchTransitionSpeedMultiplier { get; }
        public float CrouchTransitionSpeedAdd { get; }
        public float LadderClimbSpeedMultiplier { get; }
        public float LadderClimbSpeedAdd { get; }
        public float SlideSpeedMultiplier { get; }
        public float SlideSpeedAdd { get; }
        public float LeanSpeedMultiplier { get; }
        public float LeanSpeedAdd { get; }
        public float LeanAngleMultiplier { get; }
        public float LeanAngleAdd { get; }

        public ActiveEffect(ItemUseEffect effect)
        {
            DurationSeconds = effect.durationSeconds;
            SpeedMultiplier = effect.speedMultiplier;
            SpeedAdd = effect.speedAdd;
            SprintSpeedMultiplier = effect.sprintSpeedMultiplier;
            SprintSpeedAdd = effect.sprintSpeedAdd;
            CrouchSpeedMultiplier = effect.crouchSpeedMultiplier;
            CrouchSpeedAdd = effect.crouchSpeedAdd;
            JumpHeightMultiplier = effect.jumpHeightMultiplier;
            JumpHeightAdd = effect.jumpHeightAdd;
            GravityMultiplier = effect.gravityMultiplier;
            GravityAdd = effect.gravityAdd;
            MouseSensitivityMultiplier = effect.mouseSensitivityMultiplier;
            MouseSensitivityAdd = effect.mouseSensitivityAdd;
            CrouchTransitionSpeedMultiplier = effect.crouchTransitionSpeedMultiplier;
            CrouchTransitionSpeedAdd = effect.crouchTransitionSpeedAdd;
            LadderClimbSpeedMultiplier = effect.ladderClimbSpeedMultiplier;
            LadderClimbSpeedAdd = effect.ladderClimbSpeedAdd;
            SlideSpeedMultiplier = effect.slideSpeedMultiplier;
            SlideSpeedAdd = effect.slideSpeedAdd;
            LeanSpeedMultiplier = effect.leanSpeedMultiplier;
            LeanSpeedAdd = effect.leanSpeedAdd;
            LeanAngleMultiplier = effect.leanAngleMultiplier;
            LeanAngleAdd = effect.leanAngleAdd;
        }
    }
}
