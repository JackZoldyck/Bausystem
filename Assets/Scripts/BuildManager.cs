using UnityEngine;
using static PlacementModus;

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

    public Material validPreviewMaterial;
    public Material invalidPreviewMaterial;
    public LayerMask collisionMask;

    private bool canPlace;

    private GameObject previewObject;
    private float currentRotation;
    private SnapPoint currentTargetSnap;



    void Update()
    {
        HandleSelection();

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

    void SelectPrefab(int index)
    {
        if (index >= buildPrefabs.Length)
            return;

        selectedPrefabIndex = index;

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


            SnapPoint nearbySnap = FindNearbySnapPoint(freePosition);

            if (nearbySnap != null && !nearbySnap.occupied)
            {
                currentTargetSnap = nearbySnap;

                previewObject.transform.position =
                    GetPositionWithBottomSnap(nearbySnap.transform.position);
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
                    return snap;
            }

            return null;
        }
    }


    void PlaceObject()
    {
        if (previewObject == null || !previewObject.activeSelf)
            return;

        if (!canPlace)
            return;

        Instantiate(CurrentPrefab, previewObject.transform.position, previewObject.transform.rotation);
    }

    void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRotation += rotationStep;
        }
    }

    void DisableColliders(GameObject obj)
    {
        foreach (Collider col in obj.GetComponentsInChildren<Collider>())
            col.enabled = false;
    }

}