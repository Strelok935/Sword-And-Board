using UnityEngine;

public class CameraViewBob : MonoBehaviour
{
    public PlayerMovement player;
    public float frequency = 6f;
    public float amplitude = 0.05f;
    public float sprintMultiplier = 1.5f;
    public float smooth = 8f;

    Vector3 startPos;
    float timer;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        if (player == null) return;

        bool moving = player.MoveInput.magnitude > 0.1f;
        bool Grounded = player.Grounded;

        if (moving && Grounded)
        {
            float speedFactor = player.IsSprinting ? sprintMultiplier : 1f;

            timer += Time.deltaTime * frequency * speedFactor;

            float y = Mathf.Sin(timer) * amplitude;
            float x = Mathf.Cos(timer * 0.5f) * amplitude * 0.5f;

            Vector3 bob = startPos + new Vector3(x, y, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, bob, Time.deltaTime * smooth);
        }
        else
        {
            timer = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * smooth);
        }
    }
}
