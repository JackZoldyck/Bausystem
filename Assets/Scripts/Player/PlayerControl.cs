using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraPivot;
    public PlayerStats playerStats;

    public bool lookEnabled = true;

    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;

    private bool isSprinting;

    private bool sprintLockedUntilShiftRelease = false;

    public float gravity = -9.81f;
    public float mouseSensitivity = 100f;
    public float jumpHeight = 1.5f;

    public float minStaminaToSprint = 1f;
    public float jumpStaminaCost = 20f;
    public float sprintStaminaCostPerSecond = 18f;

    public Animator animator;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float xRotation;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
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
                if (playerStats.currentStamina <= minStaminaToSprint)
                {
                    isSprinting = false;
                    sprintLockedUntilShiftRelease = true;
                }
                else
                {
                    isSprinting = true;
                }
            }
            else
            {
                isSprinting = false;
            }
        }

        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        float currentSpeed =
            isSprinting ? sprintSpeed : walkSpeed;

        controller.Move(
            move * currentSpeed * Time.deltaTime
        );

        if (isSprinting)
        {
            playerStats.UseStamina(
                sprintStaminaCostPerSecond *
                Time.deltaTime
            );

            if (playerStats.currentStamina <= 0f)
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

        velocity.y += gravity * Time.deltaTime;

        controller.Move(
            velocity * Time.deltaTime
        );

        if (lookEnabled)
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

            xRotation = Mathf.Clamp(
                xRotation,
                -90f,
                90f
            );

            cameraPivot.localRotation =
                Quaternion.Euler(
                    xRotation,
                    0f,
                    0f
                );

            transform.Rotate(
                Vector3.up * mouseX
            );
        }

        float speed = moveInput.magnitude;

        if (isSprinting)
            speed = 2f;

        if (animator != null)
        {
            animator.SetFloat(
                "Speed",
                speed
            );
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (!controller.isGrounded)
            return;

        if (playerStats.currentStamina <
            jumpStaminaCost)
        {
            return;
        }

        playerStats.UseStamina(
            jumpStaminaCost
        );

        velocity.y =
            Mathf.Sqrt(
                jumpHeight *
                -2f *
                gravity
            );
    }
}