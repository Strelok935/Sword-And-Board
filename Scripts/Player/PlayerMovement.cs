using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 45f;
    public float jumpHeight = 2f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2f;

    [Header("Crouch Settings")]
    public float crouchHeight = 0.9f;
    public float standHeight = 2f;
    public float crouchTransitionSpeed = 6f;
    public bool isCrouching = false;
    private float targetHeight;

    [Header("Jump Settings")]
    public bool BunnyHopEnabled = false;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public Vector3 standingCamPos = new Vector3(0, 1.676f, 0); 
    public Vector3 crouchingCamPos = new Vector3(0, 0.9f, 0);
    private bool isCursorLocked = true;

    [Header("Item Pickup Settings")]
    public float pickupRange = 3f; // Maximum range for picking up items
    public LayerMask itemLayer; // Layer for items that can be picked up
    private GameObject hoveredItem; // The currently hovered item

    [Header("Interaction Settings")]
    public float interactionRange = 3f; // Maximum range for interaction
    public LayerMask interactableLayer; // Layer for interactable objects
    private Camera playerCamera; // Reference to the player's camera
    private Interactor interactor;
    private IInteractable currentInteractable; // Currently focused interactable object


    [Header("Camera Bobbing")]
    public float bobFrequency = 6f;
    public float bobAmplitude = 0.05f;
    public float crouchBobAmplitude = 0.025f;
    public float sprintBobAmplitude = 0.08f;
    private float bobTimer = 0f;
    public float bobHorizontalAmplitude = 0.03f; // Intensity of left-right bobbing
    public float bobHorizontalFrequency = 6f;    // Speed of left-right bobbing
    

    [Header("Fall Damage")]
    public float fallDamageThreshold = 5f;
    public float fallDamageMultiplier = 10f;
    private float fallStartY;
    private bool isFalling = false;
    private bool softLanding = false;
    private DamageZone currentDamageZone; 


    [Header("Vault Settings")]
    public float vaultUpDuration = 0.3f;
    public float vaultForwardDuration = 0.3f;
    public Vector3 vaultOffset = new Vector3(0, 2.0f, 3f); // Adjusted Y offset for higher clearance
    private bool isVaulting = false;
    private Vector3 vaultStart;
    private Vector3 vaultUpTarget;
    private Vector3 vaultForwardTarget;
    private float vaultTimer = 0f;
    private enum VaultPhase { None, Up, Forward }
    private VaultPhase vaultPhase = VaultPhase.None;
    private bool vaultReady = false;
    private Vector3 lastVaultPoint;

    [Header("Ladder Settings")]
    public float ladderClimbSpeed = 3f;
    public bool isOnLadder = false;
    private Vector3 ladderCenter;

    [Header("Environmental Effects")]
    public float damagePerSecond = 10f;
    public bool inDamageZone = false;
    public bool inSlowZone = false;
    public float slowMultiplier = 0.5f;
    public bool inFrictionZone = false;
    public float frictionMultiplier = 0.5f;

    [Header("Wall Sliding")]
    public float slideSpeed = 5f;
    private bool isSliding = false;

    [Header("Leaning")]
    public float leanAngle = 15f; // Maximum angle to lean
    public float leanSpeed = 6f;  // Speed of leaning
    private float currentLean = 0f; // Current lean angle
    private float targetLean = 0f; // Target lean angle


    [Header("Leaning Camera Offset")]
    public Vector3 leanCameraOffset = new Vector3(0.3f, 0f, 0f); // Offset applied to the camera when leaning



    // Input & Movement
    private CharacterController controller;
    private CapsuleCollider capsuleCollider;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isSprinting = false;
    private bool jumpQueued = false;

    private PlayerStats playerStats;

    // Helper
    private bool inputLocked => isVaulting;



    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        controls.Player.Sprint.performed += ctx =>
        {
            if (playerStats != null && playerStats.currentStamina > 0)
            {
                isSprinting = true; // Only allow sprinting if stamina is greater than 0
            }
        };
        controls.Player.Sprint.canceled += ctx => isSprinting = false;

        controls.Player.Jump.performed += ctx =>
        {
            jumpQueued = true;

            // Trigger vault only if in vault zone and vault is ready
            if (vaultReady && !isVaulting)
            {
                StartVault(lastVaultPoint); // Use the stored vault point
            }
        };

        controls.Player.Crouch.performed += ctx => ToggleCrouch();

        // Fix leaning directions
        controls.Player.LeanLeft.performed += ctx => targetLean = leanAngle;  // Q (Lean Left)
        controls.Player.LeanLeft.canceled += ctx => targetLean = 0f;

        controls.Player.LeanRight.performed += ctx => targetLean = -leanAngle;  // E (Lean Right)
        controls.Player.LeanRight.canceled += ctx => targetLean = 0f;

        // Interaction input (bound to F)

        controls.Player.Interact.started += ctx => HandleInteraction(); // Trigger interaction instantly

        // Mouse lock bindings
        //controls.Player.ToggleCursor.performed += ctx => HandleMouseLock();
        //controls.Player.LockCursor.performed += ctx => HandleMouseLock();
        
        // Inventory toggle
        // controls.Player.ToggleInventory.performed += ctx => ToggleInventory();
         
        }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();

    void Start()
    {
        controller = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        targetHeight = standHeight;
        controller.height = standHeight;
        controller.center = new Vector3(0, controller.height / 2f, 0);
        cameraTransform.localPosition = standingCamPos;
        playerStats = GetComponent<PlayerStats>();
        playerCamera = Camera.main;
        interactor = GetComponent<Interactor>(); // Get the Interactor component
        if (interactor == null)
        {
            Debug.LogError("Interactor component is missing from the player GameObject.");
        }
        //LockCursor();
        controller.detectCollisions = false; // Disable built-in collision detection
    }

    void Update()
    {
        if (isVaulting)
        {
            HandleVault();
            return;
        }

        if (isOnLadder)
        {
            HandleLadderMovement();
            HandleLook(); // Allow looking while on ladder
            return;
        }

       // HandleMouseLock();
        HandleLook();
        HandleMovement();
        HandleGravityAndJump();
        HandleCrouch();
        HandleWallSliding();
        HandleFallDamage();
        HandleCameraBobbing();
        HandleLeaning();

        DetectInteractable();

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleCursorLock();
        }

        if (inDamageZone && currentDamageZone != null)
        {
            float damage = currentDamageZone.damagePerSecond * Time.deltaTime;

            // Apply the damage to the player's health
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }

            Debug.Log($"Taking {damage} damage from {currentDamageZone.name}.");
        }
    }
    
        // ---------------- Interaction ----------------
   void DetectInteractable()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Camera Transform is not assigned in the PlayerMovement script.");
            return;
        }

        // Perform a raycast from the camera's position
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            // Check if the hit object implements the IInteractable interface
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                return;
            }
        }

        // Clear the interactable object if nothing is hit
        currentInteractable = null;
    }

    void HandleInteraction()
    {
       // Check if the player presses the interaction key (e.g., "F")
        if (Keyboard.current.fKey.wasPressedThisFrame && interactor.CurrentInteractable != null)
        {
            // Interact with the current interactable object
            interactor.CurrentInteractable.Interact(interactor, out bool interactionSuccess);

            if (interactionSuccess)
            {
                Debug.Log("Interaction successful!");
            }
            else
            {
                Debug.Log("Interaction failed.");
            }
        }
    }
    // ---------------- Look ----------------
    void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // ---------------- Movement ----------------
    void HandleMovement()
    {
        if (inputLocked) return;

        // Check stamina before sprinting
        if (isSprinting && playerStats != null && playerStats.currentStamina <= 0)
        {
            isSprinting = false; // Disable sprinting if stamina is 0
        }

        float currentSpeed = isSprinting ? sprintSpeed : speed;
        if (isCrouching) currentSpeed = crouchSpeed;
        if (inFrictionZone) currentSpeed *= frictionMultiplier;
        if (inSlowZone) currentSpeed *= slowMultiplier;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Drain stamina while sprinting
        if (isSprinting && playerStats != null)
        {
            playerStats.UseStamina(playerStats.staminaDrainRate * Time.deltaTime);
        }

        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    

    void HandleGravityAndJump()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, controller.height / 2 + 0.1f);
        

        if (isGrounded)
        {
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = 0f;
                jumpQueued = false;
                
            }

            if (jumpQueued)
            {
                if (playerStats != null && playerStats.currentStamina > 0)
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    playerStats.UseStamina(15f);
                    Debug.Log("Jump executed");
                }
                else
                {
                    Debug.Log("Not enough stamina to jump.");
                }

                if (!BunnyHopEnabled) jumpQueued = false;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);

        if (!isGrounded && !BunnyHopEnabled)
        {
            jumpQueued = false;
        }
    }

    void HandleWallSliding()
    {
        isSliding = false;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle >= controller.slopeLimit)
            {
                Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                controller.Move(slideDirection * slideSpeed * Time.deltaTime);
                isSliding = true;
                isFalling = false; // Reset falling state when sliding.
            }
        }
    }

    void HandleFallDamage()
    {
        if (!controller.isGrounded && !isFalling && velocity.y < 0)
        {
            isFalling = true;
            fallStartY = transform.position.y; // Record the height where the fall started
        }

        if (controller.isGrounded && isFalling)
        {
            if (!softLanding)
            {
                float fallDistance = fallStartY - transform.position.y; // Calculate the fall distance
                if (fallDistance > fallDamageThreshold)
                {
                    float damage = (fallDistance - fallDamageThreshold) * fallDamageMultiplier;

                    // Apply the damage to the player's health
                    if (playerStats != null)
                    {
                        playerStats.TakeDamage(damage);
                    }

                    Debug.Log($"Took {damage} fall damage from {fallDistance} meters.");
                }
            }
            else
            {
                Debug.Log("Soft Landing!");
            }

            isFalling = false;
            softLanding = false;
        }
    }

    // ---------------- Leaning ----------------


      
    void HandleLeaning()
    {
        // Smoothly interpolate the current lean angle to the target lean angle
        currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);

        // Apply the lean rotation to the camera
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, currentLean);

        // Calculate the camera's offset based on the lean direction
        Vector3 offset = Vector3.zero; // Default offset is zero (no leaning)
        if (Mathf.Abs(targetLean) > 0.01f) // Only apply offset if leaning
        {
            offset = targetLean > 0 ? leanCameraOffset : -leanCameraOffset;
        }

        // Apply the offset to the camera's position
        Vector3 targetCamPos = (isCrouching ? crouchingCamPos : standingCamPos) + offset;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetCamPos, Time.deltaTime * leanSpeed);
    }

// ---------------- Vault ----------------
   void StartVault(Vector3 vaultPoint)
{
    if (playerStats != null && playerStats.currentStamina <= 0)
    {
        Debug.Log("Not enough stamina to vault.");
        return; // Prevent vaulting if stamina is 0
    }

    // Drain stamina for vaulting
    if (playerStats != null)
    {
        playerStats.UseStamina(20f); // Adjust the stamina cost for vaulting
    }

    vaultStart = transform.position;

    // Adjust the upward target to ensure the player clears the wall
    vaultUpTarget = new Vector3(vaultStart.x, vaultPoint.y + vaultOffset.y, vaultStart.z);
    vaultForwardTarget = new Vector3(vaultPoint.x, vaultUpTarget.y, vaultPoint.z);

    vaultTimer = 0f;
    vaultPhase = VaultPhase.Up;
    isVaulting = true;
    controller.enabled = false;
}

    void HandleVault()
        {
            vaultTimer += Time.deltaTime;

            if (vaultPhase == VaultPhase.Up)
            {
                float t = Mathf.Clamp01(vaultTimer / vaultUpDuration);
                transform.position = Vector3.Lerp(vaultStart, vaultUpTarget, t);

                if (t >= 1f)
                {
                    vaultPhase = VaultPhase.Forward;
                    vaultTimer = 0f;
                }
            }
            else if (vaultPhase == VaultPhase.Forward)
            {
                float t = Mathf.Clamp01(vaultTimer / vaultForwardDuration);
                transform.position = Vector3.Lerp(vaultUpTarget, vaultForwardTarget, t);

            if (t >= 1f)
            {
                isVaulting = false;
                controller.enabled = true;
                vaultPhase = VaultPhase.None;
                controller.transform.position = transform.position;
                }
            }
        }

    // ---------------- Ladder ----------------
    void HandleLadderMovement()
    {
        Vector3 lockedPosition = new Vector3(ladderCenter.x, transform.position.y, ladderCenter.z);
        transform.position = lockedPosition;

        velocity = Vector3.zero;

        float vertical = moveInput.y;
        Vector3 climb = new Vector3(0, vertical * ladderClimbSpeed, 0);
        controller.Move(climb * Time.deltaTime);

        if (jumpQueued)
        {
            isOnLadder = false;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpQueued = false;
        }
    }

    // ---------------- Crouch ----------------
        void HandleCrouch()
    {
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        controller.center = new Vector3(0, controller.height / 2f, 0);

        Vector3 targetCamPos = isCrouching ? crouchingCamPos : standingCamPos;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetCamPos, Time.deltaTime * crouchTransitionSpeed);

        // Reset height and center when standing
        if (controller.isGrounded && !isCrouching)
        {
            controller.height = standHeight;
            controller.center = new Vector3(0, standHeight / 2f, 0);
        }
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        targetHeight = isCrouching ? crouchHeight : standHeight;
    }

    // ---------------- Camera Bobbing ----------------
    void HandleCameraBobbing()
{
    if (moveInput.magnitude > 0 && controller.isGrounded)
    {
        // Increment the bob timer based on movement and sprinting/crouching states
        bobTimer += Time.deltaTime * bobFrequency * (isSprinting ? 1.5f : (isCrouching ? 0.75f : 1f));

        // Calculate vertical bobbing offset
        float verticalAmplitude = isSprinting ? sprintBobAmplitude : (isCrouching ? crouchBobAmplitude : bobAmplitude);
        float verticalOffset = Mathf.Sin(bobTimer) * verticalAmplitude;

        // Calculate horizontal bobbing offset (alternates left and right)
        float horizontalOffset = Mathf.Sin(bobTimer + Mathf.PI / 2) * bobHorizontalAmplitude;

        // Apply both vertical and horizontal bobbing to the camera's position
        Vector3 targetCamPos = (isCrouching ? crouchingCamPos : standingCamPos) + new Vector3(horizontalOffset, verticalOffset, 0);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetCamPos, Time.deltaTime * 8f);
    }
    else
    {
        // Reset the bob timer and smoothly return the camera to its default position
        bobTimer = 0;
        Vector3 targetCamPos = isCrouching ? crouchingCamPos : standingCamPos;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetCamPos, Time.deltaTime * 8f);
    }
}

    private void ToggleCursorLock()
    {
        isCursorLocked = !isCursorLocked;

        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
            Cursor.visible = false; // Hide the cursor
        }
        else if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Confined; // Confine the cursor to the game window
            Cursor.visible = true; // Show the cursor
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Show the cursor
        }
}

    // ---------------- Trigger Zones ----------------
    void OnTriggerEnter(Collider other)
    {
         if (other.CompareTag("Vault"))
    {
        vaultReady = true;

        // Store the vault point for later use
        lastVaultPoint = other.transform.position;
    }

        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
            ladderCenter = other.bounds.center;
            velocity = Vector3.zero;
        }

        if (other.CompareTag("DamageZone"))
    {
        inDamageZone = true;

        // Get the DamageZone component from the object
        currentDamageZone = other.GetComponent<DamageZone>();
    }
        if (other.CompareTag("FrictionZone")) inFrictionZone = true;
        if (other.CompareTag("SlowZone")) inSlowZone = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Vault"))
        {
            vaultReady = false;
        }

        if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
        }

         if (other.CompareTag("DamageZone"))
    {
        inDamageZone = false;

        // Clear the reference to the DamageZone
        currentDamageZone = null;
    }
        if (other.CompareTag("FrictionZone")) inFrictionZone = false;
        if (other.CompareTag("SlowZone")) inSlowZone = false;
    }
}
