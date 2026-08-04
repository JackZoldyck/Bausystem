using UnityEngine;
using UnityEngine.InputSystem;

public class CameraModeController : MonoBehaviour
{
    public enum CameraMode
    {
        FirstPerson,
        ThirdPerson
    }

    [Header("References")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;

    [Tooltip("Das sichtbare Third-Person-Charaktermodell.")]
    [SerializeField] private GameObject playerVisual;

    [Header("Starting Mode")]
    [SerializeField]
    private CameraMode startingMode =
        CameraMode.FirstPerson;

    [Header("First Person")]
    [SerializeField]
    private Vector3 firstPersonPosition =
        Vector3.zero;

    [Header("Third Person")]
    [SerializeField]
    private Vector3 thirdPersonPosition =
        new Vector3(0.55f, 0.25f, -4f);

    [Header("Transition")]
    [SerializeField, Min(0f)]
    private float transitionSpeed = 12f;

    [Header("Camera Collision")]
    [SerializeField] private bool useCameraCollision = true;

    [SerializeField, Min(0.01f)]
    private float collisionRadius = 0.25f;

    [SerializeField, Min(0f)]
    private float collisionPadding = 0.1f;

    [SerializeField]
    private LayerMask cameraCollisionMask = ~0;

    private CameraMode currentMode;
    private Vector3 currentVelocity;

    public Camera ActiveCamera => playerCamera;
    public CameraMode CurrentMode => currentMode;
    public bool IsFirstPerson =>
        currentMode == CameraMode.FirstPerson;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        if (cameraPivot == null &&
            playerCamera != null)
        {
            cameraPivot = playerCamera.transform.parent;
        }
    }

    private void Start()
    {
        SetMode(startingMode, true);
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.vKey.wasPressedThisFrame)
        {
            ToggleCameraMode();
        }
    }

    private void LateUpdate()
    {
        if (cameraPivot == null ||
            playerCamera == null)
        {
            return;
        }

        Vector3 targetWorldPosition =
            GetTargetWorldPosition();

        playerCamera.transform.position =
            Vector3.SmoothDamp(
                playerCamera.transform.position,
                targetWorldPosition,
                ref currentVelocity,
                GetSmoothTime()
            );

        playerCamera.transform.rotation =
            cameraPivot.rotation;
    }

    public void ToggleCameraMode()
    {
        CameraMode nextMode =
            currentMode == CameraMode.FirstPerson
                ? CameraMode.ThirdPerson
                : CameraMode.FirstPerson;

        SetMode(nextMode, false);
    }

    public void SetMode(
        CameraMode newMode,
        bool applyImmediately)
    {
        currentMode = newMode;

        if (playerVisual != null)
        {
            playerVisual.SetActive(
                currentMode == CameraMode.ThirdPerson
            );
        }

        if (applyImmediately &&
            cameraPivot != null &&
            playerCamera != null)
        {
            playerCamera.transform.position =
                GetTargetWorldPosition();

            playerCamera.transform.rotation =
                cameraPivot.rotation;

            currentVelocity = Vector3.zero;
        }
    }

    private Vector3 GetTargetWorldPosition()
    {
        Vector3 localTarget =
            currentMode == CameraMode.FirstPerson
                ? firstPersonPosition
                : thirdPersonPosition;

        Vector3 desiredWorldPosition =
            cameraPivot.TransformPoint(localTarget);

        if (currentMode == CameraMode.FirstPerson ||
            !useCameraCollision)
        {
            return desiredWorldPosition;
        }

        Vector3 pivotPosition =
            cameraPivot.position;

        Vector3 direction =
            desiredWorldPosition - pivotPosition;

        float desiredDistance =
            direction.magnitude;

        if (desiredDistance <= Mathf.Epsilon)
        {
            return pivotPosition;
        }

        direction /= desiredDistance;

        if (Physics.SphereCast(
            pivotPosition,
            collisionRadius,
            direction,
            out RaycastHit hit,
            desiredDistance,
            cameraCollisionMask,
            QueryTriggerInteraction.Ignore))
        {
            float safeDistance =
                Mathf.Max(
                    0f,
                    hit.distance - collisionPadding
                );

            return pivotPosition +
                   direction * safeDistance;
        }

        return desiredWorldPosition;
    }

    private float GetSmoothTime()
    {
        if (transitionSpeed <= 0f)
        {
            return 0.0001f;
        }

        return 1f / transitionSpeed;
    }
}