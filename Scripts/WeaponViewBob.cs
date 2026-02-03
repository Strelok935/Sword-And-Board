using UnityEngine;

public class WeaponViewBob : MonoBehaviour
{
    public PlayerMovement player;

    [Header("Base Bob")]
    public float bobSpeed = 6f;
    public float verticalAmount = 0.04f;
    public float horizontalAmount = 0.05f;

    [Header("Sprint Modifier")]
    public float sprintSpeedMultiplier = 1.6f;
    public float sprintBobMultiplier = 1.8f;

    public float idleReturnSpeed = 6f;

    private float bobTimer;
    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (player == null) return;

        bool moving = player.IsMoving && player.Grounded;
        bool sprinting = player.IsSprinting && moving;

        float speed = sprinting ? bobSpeed * sprintSpeedMultiplier : bobSpeed;
        float vertAmt = sprinting ? verticalAmount * sprintBobMultiplier : verticalAmount;
        float horizAmt = sprinting ? horizontalAmount * sprintBobMultiplier : horizontalAmount;

        if (moving)
        {
            bobTimer += Time.deltaTime * speed;

            float vertical = Mathf.Sin(bobTimer) * vertAmt;
            float horizontal = Mathf.Cos(bobTimer * 0.5f) * horizAmt;

            Vector3 targetPos = startLocalPos + new Vector3(horizontal, vertical, 0);
            transform.localPosition = targetPos;
        }
        else
        {
            bobTimer = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, startLocalPos, Time.deltaTime * idleReturnSpeed);
        }
    }
}
