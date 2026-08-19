using UnityEngine;
using static PlacementModus;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    [Header("References")]
    public Transform buildOrigin;
    public Camera playerCamera;
    public LayerMask groundMask;
    public LayerMask buildableMask;

    [Header("Build Settings")]
    public GameObject[] buildPrefabs;

    private int selectedPrefabIndex = 0;

    public GameObject CurrentPrefab
    {
        get { return buildPrefabs[selectedPrefabIndex]; }
    }
    public PlacementMode placementMode = PlacementMode.Free;
    public float maxBuildDistance = 6f;
    public float autoSnapRange = 0.5f;
    public float rotationStep = 45f;
    public LayerMask deleteMask;

    public InventoryGridUI inventoryGridUI;
    public ItemData woodItem;

    public Material validPreviewMaterial;
    public Material invalidPreviewMaterial;
    public LayerMask collisionMask;
    public UIMessage uiMessage;
    public InventoryUI inventoryUI;

    public bool showSnapDebug = false;

    public bool IsBuildModeActive()
    {
        return buildModeActive;
    }

    private bool canPlace;

    private bool buildModeActive = false;

    private GameObject previewObject;
    private float currentRotation = 0f;
    private SnapPoint currentTargetSnap;
    private PlayerInventory inventory;




    void Start()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();
    }

    public void SetBuildModeActive(bool active)
    {
        buildModeActive = active;

        if (!buildModeActive && previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelBuildMode();
            return;
        }

        if (!buildModeActive)
        {
            if (previewObject != null)
            {
                Destroy(previewObject);
                previewObject = null;
            }

            return;
        }

        if (CurrentPrefab == null)
            return;

        if (previewObject == null)
            CreatePreview();

        HandleRotation();
        UpdatePreviewPosition();
        CheckCollision();

        if (Input.GetMouseButtonDown(0))
            PlaceObject();

        if (Input.GetMouseButtonDown(1))
            DeleteObject();
    }

    void HandleSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectPrefab(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectPrefab(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectPrefab(2);
    }

    public void SelectPrefab(int index)
    {
        if (index >= buildPrefabs.Length)
            return;

        selectedPrefabIndex = index;

        buildModeActive = true;

        if (previewObject != null)
            Destroy(previewObject);
    }

    void DeleteObject()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxBuildDistance, deleteMask))
        {
            BuildableObject buildable = hit.collider.GetComponentInParent<BuildableObject>();

            if (buildable != null)
            {
                BuildSupport[] allSupports = FindObjectsOfType<BuildSupport>();

                foreach (BuildSupport support in allSupports)
                {
                    if (support.supportedBy == buildable)
                    {
                        support.RemoveSupport();
                    }
                }

                BuildRefund refund = buildable.GetComponent<BuildRefund>();

                if (refund != null)
                    refund.Refund();

                Destroy(buildable.gameObject);
            }
        }
    }

    void CreatePreview()
    {
        previewObject = Instantiate(CurrentPrefab);
        DisableColliders(previewObject);
        SetPreviewMaterial(validPreviewMaterial);
    }

    void SetPreviewMaterial(Material material)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.material = material;
        }
    }

    void CheckCollision()
    {
        if (previewObject == null || !previewObject.activeSelf)
            return;

        BuildableObject buildable = previewObject.GetComponent<BuildableObject>();

        if (buildable == null || buildable.placementCheckCollider == null)
        {
            canPlace = true;
            SetPreviewMaterial(validPreviewMaterial);
            return;
        }

        Bounds bounds = buildable.placementCheckCollider.bounds;

        Collider[] hits = Physics.OverlapBox(
            bounds.center,
            bounds.extents,
            buildable.placementCheckCollider.transform.rotation,
            collisionMask
        );

        canPlace = true;

        foreach (Collider hit in hits)
        {
            if (hit.transform.IsChildOf(previewObject.transform))
                continue;

            if (currentTargetSnap != null)
            {
                BuildableObject snapParent =
                    currentTargetSnap.GetComponentInParent<BuildableObject>();

                if (snapParent != null && hit.transform.IsChildOf(snapParent.transform))
                    continue;
            }
            Debug.Log(hit.name);
            canPlace = false;
            break;
        }

        SetPreviewMaterial(canPlace ? validPreviewMaterial : invalidPreviewMaterial);
    }
    Bounds GetPreviewBounds()
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }

    void UpdatePreviewPosition()
    {
        UpdateFreePlacement();
    }

    void UpdateFreePlacement()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxBuildDistance, groundMask | buildableMask))
        {
            previewObject.SetActive(true);

            previewObject.transform.rotation =
                Quaternion.Euler(0, currentRotation, 0) * CurrentPrefab.transform.rotation;

            Vector3 freePosition = GetPositionWithBottomSnap(hit.point);

            previewObject.transform.position = freePosition;

            SnapPoint nearbySnap = FindNearbySnapPointNearOwnSnaps();

            SnapPoint FindNearbySnapPointNearOwnSnaps()
            {
                BuildableObject buildable = previewObject.GetComponent<BuildableObject>();

                if (buildable == null || buildable.snapPoints == null)
                    return null;

                SnapPoint closestSnap = null;
                float closestDistance = autoSnapRange;

                foreach (SnapPoint ownSnap in buildable.snapPoints)
                {
                    Collider[] colliders = Physics.OverlapSphere(
                        ownSnap.transform.position,
                        autoSnapRange,
                        buildableMask
                    );

                    foreach (Collider col in colliders)
                    {
                        SnapPoint targetSnap = col.GetComponentInParent<SnapPoint>();

                        if (targetSnap == null || targetSnap.occupied)
                            continue;

                        if (targetSnap.transform.IsChildOf(previewObject.transform))
                            continue;

                        SnapPoint matchingOwnSnap = FindMatchingSnapPoint(targetSnap);

                        if (matchingOwnSnap != ownSnap)
                            continue;

                        float distance = Vector3.Distance(
                            ownSnap.transform.position,
                            targetSnap.transform.position
                        );

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestSnap = targetSnap;
                        }
                    }
                }

                return closestSnap;
            }

            if (nearbySnap != null && !nearbySnap.occupied)
            {
                currentTargetSnap = nearbySnap;

                previewObject.transform.position =
                    GetPositionWithMatchingSnap(nearbySnap);
            }
            else
            {
                currentTargetSnap = null;

                previewObject.transform.position = freePosition;
            }
        }
        else
        {
            previewObject.SetActive(false);
        }
        Vector3 GetPositionWithBottomSnap(Vector3 targetPoint)
        {
            BuildableObject buildable = previewObject.GetComponent<BuildableObject>();

            if (buildable != null && buildable.bottomSnap != null)
            {
                Vector3 offset = previewObject.transform.position - buildable.bottomSnap.position;
                return targetPoint + offset;
            }

            return targetPoint;
        }
        SnapPoint FindNearbySnapPoint(Vector3 position)
        {
            Collider[] colliders = Physics.OverlapSphere(position, autoSnapRange, buildableMask);

            foreach (Collider col in colliders)
            {
                SnapPoint snap = col.GetComponentInParent<SnapPoint>();

                if (snap != null && !snap.occupied)
                {

                    return snap;
                }
            }

            return null;
        }
    }


    void PlaceObject()
    {


        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (previewObject == null || !previewObject.activeSelf)
            return;

        if (!canPlace)
            return;

        BuildCost cost = CurrentPrefab.GetComponent<BuildCost>();

        if (cost != null)
        {
            if (!inventoryGridUI.HasItem(woodItem, cost.woodCost))
            {
                uiMessage.ShowMessage("MATERIAL MISSING");
                return;
            }
        }
        GameObject placedObject = Instantiate(
            CurrentPrefab,
            previewObject.transform.position,
            previewObject.transform.rotation
        );

        if (cost != null)
        {
            inventoryGridUI.RemoveItem(woodItem, cost.woodCost);
        }

        BuildSupport buildSupport = placedObject.GetComponent<BuildSupport>();

        if (buildSupport != null && currentTargetSnap != null)
        {
            BuildableObject supportObject =
                currentTargetSnap.GetComponentInParent<BuildableObject>();

            buildSupport.SetSupport(supportObject);
        }
    }

    void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRotation = (currentRotation + rotationStep) % 360f;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentRotation = (currentRotation - rotationStep + 360f) % 360f;
        }
    }

    void DisableColliders(GameObject obj)
    {
        foreach (Collider col in obj.GetComponentsInChildren<Collider>())
            col.enabled = false;
    }
    SnapPoint FindMatchingSnapPoint(SnapPoint targetSnap)
    {
        BuildableObject buildable = previewObject.GetComponent<BuildableObject>();

        if (buildable == null || buildable.snapPoints == null)
            return null;

        foreach (SnapPoint ownSnap in buildable.snapPoints)
        {
            {
                foreach (string compatibleType in ownSnap.compatibleSnapTypes)
                {
                    if (compatibleType == targetSnap.snapType)
                    {
                        return ownSnap;
                    }
                }
            }
        }

        return null;
    }
    Vector3 GetPositionWithMatchingSnap(SnapPoint targetSnap)
    {
        SnapPoint ownSnap = FindMatchingSnapPoint(targetSnap);

        if (ownSnap == null)
            return targetSnap.transform.position;

        Vector3 offset = previewObject.transform.position - ownSnap.transform.position;

        return targetSnap.transform.position + offset;
    }
    public void CancelBuildMode()
    {
        buildModeActive = false;

        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        currentTargetSnap = null;
    }

}