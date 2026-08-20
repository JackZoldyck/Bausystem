using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform cameraPivot;
    public PlayerStats playerStats;
    public Animator animator;
    public PlayerZoomCamera playerZoomCamera;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 12f;

    [Header("Jump")]
    public float jumpHeight = 1.5f;
    public float jumpStaminaCost = 20f;

    [Header("Stamina")]
    public float minStaminaToSprint = 1f;
    public float sprintStaminaCostPerSecond = 18f;

    [Header("Gravity")]
    public float gravity = -9.81f;

    [Header("Camera")]
    public bool lookEnabled = true;
    public float mouseSensitivity = 100f;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private Vector3 velocity;

    private float xRotation;
    private float thirdPersonCameraYaw;

    private bool isSprinting;
    private bool sprintLockedUntilShiftRelease;

    private float airborneMoveSpeed;

    private float movementReferenceYaw;
    private Vector3 lockedThirdPersonMoveDirection;
    private Vector2 previousMoveInput;


    private void Start()
    {
        if (controller == null)
        {
            controller =
                GetComponent<CharacterController>();
        }

        airborneMoveSpeed = walkSpeed;

        if (cameraPivot != null)
        {
            thirdPersonCameraYaw =
                cameraPivot.eulerAngles.y;
        }

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }


    private void Update()
    {
        if (controller == null)
            return;

        bool firstPerson = true;

        if (playerZoomCamera != null)
        {
            firstPerson =
                playerZoomCamera.IsFirstPerson;
        }

        bool shiftPressed =
            Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed;


        if (sprintLockedUntilShiftRelease)
        {
            isSprinting = false;

            if (!shiftPressed)
            {
                sprintLockedUntilShiftRelease = false;
            }
        }
        else
        {
            bool wantsToSprint =
                shiftPressed &&
                moveInput.y > 0f;

            if (wantsToSprint)
            {
                if (playerStats != null &&
                    playerStats.currentStamina >
                    minStaminaToSprint)
                {
                    isSprinting = true;
                }
                else
                {
                    isSprinting = false;
                    sprintLockedUntilShiftRelease = true;
                }
            }
            else
            {
                isSprinting = false;
            }
        }

        Vector3 moveDirection;

        if (firstPerson)
        {
            moveDirection =
                transform.right * moveInput.x +
                transform.forward * moveInput.y;
        }
        else
        {
            Vector3 cameraForward =
                cameraPivot.forward;

            cameraForward.y = 0f;
            cameraForward.Normalize();

            bool hasMovementInput =
                moveInput.sqrMagnitude > 0.01f;

            if (!hasMovementInput)
            {
                moveDirection = Vector3.zero;

                lockedThirdPersonMoveDirection =
                    Vector3.zero;
            }
            else
            {
                // W:
                // Immer live in aktuelle Kamerablickrichtung.
                if (moveInput.y > 0f &&
                    Mathf.Abs(moveInput.x) < 0.01f)
                {
                    moveDirection =
                        cameraForward;

                    lockedThirdPersonMoveDirection =
                        Vector3.zero;
                }
                else
                {
                    // A / S / D:
                    // Richtung nur beim Beginn bzw.
                    // bei geändertem Input neu bestimmen.
                    bool movementStarted =
                        previousMoveInput.sqrMagnitude <= 0.01f;

                    bool inputChanged =
                        Vector2.Distance(
                            moveInput.normalized,
                            previousMoveInput.normalized
                        ) > 0.1f;

                    if (movementStarted ||
                        inputChanged ||
                        lockedThirdPersonMoveDirection.sqrMagnitude < 0.01f)
                    {
                        float angleOffset =
                            Mathf.Atan2(
                                moveInput.x,
                                moveInput.y
                            ) * Mathf.Rad2Deg;

                        Quaternion directionRotation =
                            Quaternion.AngleAxis(
                                angleOffset,
                                Vector3.up
                            );

                        lockedThirdPersonMoveDirection =
                            directionRotation *
                            cameraForward;

                        lockedThirdPersonMoveDirection.Normalize();
                    }

                    moveDirection =
                        lockedThirdPersonMoveDirection;
                }

                if (moveDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation =
                        Quaternion.LookRotation(
                            moveDirection
                        );

                    transform.rotation =
                        Quaternion.Slerp(
                            transform.rotation,
                            targetRotation,
                            rotationSpeed *
                            Time.deltaTime
                        );
                }
            }
        }

        previousMoveInput = moveInput;

        previousMoveInput = moveInput;

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        float currentSpeed;

        if (controller.isGrounded)
        {
            currentSpeed =
                isSprinting
                    ? sprintSpeed
                    : walkSpeed;

            airborneMoveSpeed =
                currentSpeed;
        }
        else
        {
            currentSpeed =
                airborneMoveSpeed;
        }

        controller.Move(
            moveDirection *
            currentSpeed *
            Time.deltaTime
        );

        if (isSprinting &&
            controller.isGrounded &&
            playerStats != null)
        {
            playerStats.UseStamina(
                sprintStaminaCostPerSecond *
                Time.deltaTime
            );

            if (playerStats.currentStamina <=
                minStaminaToSprint)
            {
                playerStats.currentStamina = 0f;

                isSprinting = false;
                sprintLockedUntilShiftRelease = true;
            }
        }

        if (controller.isGrounded &&
            velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y +=
            gravity * Time.deltaTime;

        controller.Move(
            velocity *
            Time.deltaTime
        );

        if (lookEnabled &&
            cameraPivot != null)
        {
            float mouseX =
                lookInput.x *
                mouseSensitivity *
                Time.deltaTime;

            float mouseY =
                lookInput.y *
                mouseSensitivity *
                Time.deltaTime;


            xRotation -= mouseY;

            xRotation =
                Mathf.Clamp(
                    xRotation,
                    -90f,
                    90f
                );


            if (firstPerson)
            {
                cameraPivot.localRotation =
                    Quaternion.Euler(
                        xRotation,
                        0f,
                        0f
                    );

                transform.Rotate(
                    Vector3.up *
                    mouseX
                );

                thirdPersonCameraYaw =
                    transform.eulerAngles.y;
            }
            else
            {
                thirdPersonCameraYaw +=
                    mouseX;

                cameraPivot.rotation =
                    Quaternion.Euler(
                        xRotation,
                        thirdPersonCameraYaw,
                        0f
                    );
            }
        }

        if (animator != null)
        {
            float animationSpeed =
                moveInput.magnitude;

            if (isSprinting)
            {
                animationSpeed = 2f;
            }

            animator.SetFloat(
                "Speed",
                animationSpeed
            );

            animator.SetBool(
                "IsGrounded",
                controller.isGrounded
            );
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput =
            value.Get<Vector2>();
    }


    public void OnLook(InputValue value)
    {
        lookInput =
            value.Get<Vector2>();
    }


    public void OnJump(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (controller == null ||
            !controller.isGrounded)
        {
            return;
        }

        if (playerStats == null)
            return;

        if (playerStats.currentStamina <
            jumpStaminaCost)
        {
            return;
        }


        playerStats.UseStamina(
            jumpStaminaCost
        );


        airborneMoveSpeed =
            isSprinting
                ? sprintSpeed
                : walkSpeed;


        velocity.y =
            Mathf.Sqrt(
                jumpHeight *
                -2f *
                gravity
            );


        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }
    }
}