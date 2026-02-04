using UnityEngine;

public class CameraViewBob : MonoBehaviour
{
    public PlayerMovement player;

    [Header("Movement Bob")]
    public float moveBobFrequency = 7f;
    public float moveBobVerticalAmp = 0.05f;
    public float moveBobHorizontalAmp = 0.03f;

    [Header("Sprint Modifier")]
    public float sprintBobMultiplier = 1.6f;

    [Header("Idle Breathing Bob")]
    public float idleBobFrequency = 1.5f;
    public float idleBobVerticalAmp = 0.01f;
    public float idleBobHorizontalAmp = 0.005f;

    [Header("Smoothing")]
    public float smoothSpeed = 8f;

    private Vector3 startLocalPos;
    private float bobTimer;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }
    public void ResetBasePosition(Vector3 newBasePos)
    {
        startLocalPos = newBasePos;
    }


    void Update()
    {
        if (player == null) return;

        bool grounded = player.Grounded;
        bool moving = player.IsMoving;

        float frequency;
        float vAmp;
        float hAmp;

        if (grounded && moving)
        {
            frequency = moveBobFrequency;
            if (player.IsSprinting)
                frequency *= sprintBobMultiplier;

            vAmp = moveBobVerticalAmp;
            hAmp = moveBobHorizontalAmp;
        }
        else
        {
            frequency = idleBobFrequency;
            vAmp = idleBobVerticalAmp;
            hAmp = idleBobHorizontalAmp;
        }

        bobTimer += Time.deltaTime * frequency;

        float vertical = Mathf.Sin(bobTimer) * vAmp;
        float horizontal = Mathf.Sin(bobTimer * 0.5f) * hAmp;

        Vector3 bobOffset = new Vector3(horizontal, vertical, 0);
        Vector3 targetPos = startLocalPos + bobOffset;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothSpeed);
    }
}
