using UnityEngine;

public class WeaponViewBob : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement player;

    [Header("Movement Bob")]
    public float bobFrequency = 7f;
    public float bobVerticalAmplitude = 0.04f;
    public float bobHorizontalAmplitude = 0.025f;
    public float sprintMultiplier = 1.6f;

    [Header("Idle Breathing Bob")]
    public float idleFrequency = 1.2f;
    public float idleAmplitude = 0.01f;

    [Header("Momentum Lean")]
    public float leanAmount = 6f;
    public float leanSmooth = 6f;

    private float bobTimer;
    private float idleTimer;

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;

    private float currentLeanZ;

    void Start()
    {
        if (!player) player = GetComponentInParent<PlayerMovement>();

        // Store ORIGINAL pose (critical fix)
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;
    }

    void Update()
    {
        if (!player) return;

        bool moving = player.IsMoving && player.Grounded;
        bool sprinting = player.IsSprinting;

        float speedMultiplier = sprinting ? sprintMultiplier : 1f;

        Vector3 bobOffset = Vector3.zero;

        // -------- MOVEMENT BOB --------
        if (moving)
        {
            bobTimer += Time.deltaTime * bobFrequency * speedMultiplier;

            float vertical = Mathf.Sin(bobTimer) * bobVerticalAmplitude;
            float horizontal = Mathf.Sin(bobTimer * 0.5f) * bobHorizontalAmplitude;

            bobOffset += new Vector3(horizontal, vertical, 0);
        }
        else
        {
            bobTimer = 0;
        }

        // -------- IDLE BREATHING --------
        idleTimer += Time.deltaTime * idleFrequency;
        float idleY = Mathf.Sin(idleTimer) * idleAmplitude;
        bobOffset += new Vector3(0, idleY, 0);

        // Apply POSITION relative to original
        transform.localPosition = initialLocalPos + bobOffset;

        // -------- MOMENTUM LEAN --------
        Vector2 moveInput = player.MoveInput;
        float targetLean = -moveInput.x * leanAmount;
        currentLeanZ = Mathf.Lerp(currentLeanZ, targetLean, Time.deltaTime * leanSmooth);

        // Apply ROTATION relative to original
        Quaternion leanRot = Quaternion.Euler(0, 0, currentLeanZ);
        transform.localRotation = initialLocalRot * leanRot;
    }
}
