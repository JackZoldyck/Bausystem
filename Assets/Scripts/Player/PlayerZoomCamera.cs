using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerZoomCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform characterVisualRoot;

    [Header("Zoom")]
    [SerializeField, Min(0f)]
    private float startingDistance = 3f;

    [SerializeField, Min(0f)]
    private float minimumDistance = 0f;

    [SerializeField, Min(0f)]
    private float maximumDistance = 6f;

    [SerializeField, Min(0.01f)]
    private float zoomStep = 0.75f;

    [SerializeField, Min(0.01f)]
    private float zoomSmoothTime = 0.08f;

    [Header("Third Person Offset")]
    [SerializeField]
    private float shoulderOffset = 0.25f;

    [SerializeField]
    private float heightOffset = 0.15f;

    [Header("First Person")]
    [SerializeField, Min(0f)]
    private float firstPersonThreshold = 0.15f;

    [SerializeField]
    private bool hideCharacterInFirstPerson = true;

    [Header("Camera Collision")]
    [SerializeField]
    private bool useCameraCollision = true;

    [SerializeField, Min(0.01f)]
    private float collisionRadius = 0.2f;

    [SerializeField, Min(0f)]
    private float collisionPadding = 0.08f;

    [SerializeField]
    private LayerMask collisionMask = ~0;

    [Header("First Person Objects")]
    [SerializeField] private GameObject firstPersonToolHolder;
    [SerializeField] private GameObject thirdPersonToolHolder;
    [SerializeField] private PlayerTool playerTool;

    private float targetDistance;
    private float currentDistance;
    private float distanceVelocity;

    private Renderer[] characterRenderers;

    private bool characterVisible = true;

    public bool IsFirstPerson =>
        currentDistance <= firstPersonThreshold;

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

        if (characterVisualRoot != null)
        {
            characterRenderers =
                characterVisualRoot.GetComponentsInChildren<Renderer>(
                    true
                );
        }

        minimumDistance = Mathf.Max(0f, minimumDistance);

        maximumDistance = Mathf.Max(
            minimumDistance,
            maximumDistance
        );

        targetDistance = Mathf.Clamp(
            startingDistance,
            minimumDistance,
            maximumDistance
        );

        currentDistance = targetDistance;
    }

    private void Start()
    {
        ApplyCameraPosition(true);
    }

    private void Update()
    {
        ReadZoomInput();
    }

    private void LateUpdate()
    {
        if (cameraPivot == null ||
            playerCamera == null)
        {
            return;
        }

        currentDistance = Mathf.SmoothDamp(
            currentDistance,
            targetDistance,
            ref distanceVelocity,
            zoomSmoothTime
        );

        ApplyCameraPosition(false);
        UpdateCharacterVisibility();
    }

    private void ReadZoomInput()
    {
        if (Mouse.current == null)
            return;

        float scroll =
            Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) < 0.01f)
            return;

        // Mausrad hoch: näher heran.
        // Mausrad runter: weiter weg.
        targetDistance -=
            Mathf.Sign(scroll) * zoomStep;

        targetDistance = Mathf.Clamp(
            targetDistance,
            minimumDistance,
            maximumDistance
        );

        // Direkt sauber auf First Person einrasten.
        if (targetDistance <= firstPersonThreshold)
        {
            targetDistance = 0f;
        }
    }

    private void ApplyCameraPosition(bool immediate)
    {
        Vector3 targetPosition =
            CalculateCameraWorldPosition();

        if (immediate)
        {
            playerCamera.transform.position =
                targetPosition;
        }
        else
        {
            playerCamera.transform.position =
                targetPosition;
        }

        // Blickrichtung wird vollständig vom CameraPivot übernommen.
        playerCamera.transform.rotation =
            cameraPivot.rotation;
    }

    private Vector3 CalculateCameraWorldPosition()
    {
        if (currentDistance <= firstPersonThreshold)
        {
            return cameraPivot.position;
        }

        Vector3 localOffset = new Vector3(
            shoulderOffset,
            heightOffset,
            -currentDistance
        );

        Vector3 desiredPosition =
            cameraPivot.TransformPoint(localOffset);

        if (!useCameraCollision)
            return desiredPosition;

        Vector3 direction =
            desiredPosition - cameraPivot.position;

        float desiredLength =
            direction.magnitude;

        if (desiredLength <= Mathf.Epsilon)
            return cameraPivot.position;

        direction /= desiredLength;

        if (Physics.SphereCast(
            cameraPivot.position,
            collisionRadius,
            direction,
            out RaycastHit hit,
            desiredLength,
            collisionMask,
            QueryTriggerInteraction.Ignore))
        {
            float safeDistance = Mathf.Max(
                0f,
                hit.distance - collisionPadding
            );

            return cameraPivot.position +
                   direction * safeDistance;
        }

        return desiredPosition;
    }

    private void UpdateCharacterVisibility()
    {
        bool isFirstPerson = IsFirstPerson;

        if (hideCharacterInFirstPerson &&
            characterRenderers != null)
        {
            bool shouldShowCharacter = !isFirstPerson;

            if (shouldShowCharacter != characterVisible)
            {
                characterVisible = shouldShowCharacter;

                foreach (Renderer characterRenderer
                         in characterRenderers)
                {
                    if (characterRenderer != null)
                    {
                        characterRenderer.enabled =
                            shouldShowCharacter;
                    }
                }
            }
        }

        if (playerTool != null)
            playerTool.SetFirstPerson(isFirstPerson);
    }
}