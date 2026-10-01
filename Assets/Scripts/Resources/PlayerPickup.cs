using TMPro;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InventoryGridUI inventoryGrid;
    [SerializeField] private TMP_Text pickupPromptText;
    [SerializeField] private TMP_Text inventoryFullText;
    [SerializeField] private TMP_Text pickupContentText;
    [SerializeField] private UIStateManager uiStateManager;

    [Header("Pickup Detection")]
    [Tooltip("Maximale Entfernung zwischen Player und Pickup.")]
    [SerializeField, Min(0.1f)]
    private float pickupRange = 3f;

    [Tooltip("Reichweite des Suchstrahls von der Kamera.")]
    [SerializeField, Min(0.1f)]
    private float cameraSearchRange = 10f;

    [SerializeField, Min(0.01f)]
    private float sphereCastRadius = 0.3f;

    [Tooltip("Player-Layer hier ausschlieﬂen.")]
    [SerializeField]
    private LayerMask detectionMask = ~0;

    private PickupItem currentPickup;
    private BerryBushHarvest currentBerryBush;
    private LootContainer currentLootContainer;

    private Coroutine inventoryFullCoroutine;

    private void Update()
    {
        if (uiStateManager != null &&
       uiStateManager.IsAnyMenuOpen())
        {
            ClearCurrentPickup();
            HideInventoryFull();
            return;
        }

        CheckForPickup();


        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentPickup != null)
            {
                PickupItem pickupToCollect =
                    currentPickup;

                int requestedAmount =
                    pickupToCollect.amount;

                int addedAmount =
                    pickupToCollect.Pickup(
                        inventoryGrid
                    );

                if (addedAmount < requestedAmount)
                {
                    ShowInventoryFull();
                }

                return;
            }

            if (currentLootContainer != null)
            {
                LootContainer containerToCollect =
                    currentLootContainer;

                bool everythingCollected =
                    containerToCollect.Pickup(
                        inventoryGrid
                    );

                if (!everythingCollected)
                {
                    ShowInventoryFull();
                }

                return;
            }

            if (currentBerryBush != null)
            {
                BerryBushHarvest bushToHarvest =
                    currentBerryBush;

                ClearCurrentPickup();

                bushToHarvest.Harvest(
                    inventoryGrid
                );
            }
        }
    }

    private void CheckForPickup()
    {
        if (playerCamera == null)
        {
            ClearCurrentPickup();
            return;
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (!Physics.SphereCast(
            ray,
            sphereCastRadius,
            out RaycastHit hit,
            cameraSearchRange,
            detectionMask,
            QueryTriggerInteraction.Collide))
        {
            ClearCurrentPickup();
            return;
        }

        PickupItem pickup =
            hit.collider.GetComponentInParent<PickupItem>();

        if (pickup != null)
        {
            float distanceFromPlayer =
                Vector3.Distance(
                    transform.position,
                    pickup.transform.position
                );

            if (distanceFromPlayer <= pickupRange)
            {
                SetCurrentPickup(pickup);
                return;
            }
        }

        LootContainer lootContainer =
            hit.collider.GetComponentInParent<LootContainer>();

        if (lootContainer != null)
        {
            float distanceFromPlayer =
                Vector3.Distance(
                    transform.position,
                    lootContainer.transform.position
                );

            if (distanceFromPlayer <= pickupRange)
            {
                SetCurrentLootContainer(
                    lootContainer
                );

                return;
            }
        }

        BerryBushHarvest berryBush =
            hit.collider.GetComponentInParent<BerryBushHarvest>();

        if (berryBush != null &&
            berryBush.HasBerries())
        {
            float distanceFromPlayer =
                Vector3.Distance(
                    transform.position,
                    berryBush.transform.position
                );

            if (distanceFromPlayer <= pickupRange)
            {
                SetCurrentBerryBush(
                    berryBush
                );

                return;
            }
        }

        ClearCurrentPickup();
    }

    private void SetCurrentPickup(
    PickupItem pickup
)
    {
        currentBerryBush = null;
        currentLootContainer = null;
        currentPickup = pickup;

        if (pickupContentText != null)
        {
            pickupContentText.gameObject.SetActive(
                false
            );
        }

        if (pickupPromptText == null)
            return;

        pickupPromptText.text =
            pickup.GetPromptText();

        pickupPromptText.gameObject.SetActive(true);
    }

    private void SetCurrentLootContainer(
    LootContainer lootContainer
)
    {
        currentPickup = null;
        currentBerryBush = null;
        currentLootContainer = lootContainer;

        if (pickupContentText != null)
        {
            pickupContentText.text =
                lootContainer.GetContentText();

            pickupContentText.gameObject.SetActive(
                true
            );
        }

        if (pickupPromptText == null)
            return;

        pickupPromptText.text =
            lootContainer.GetPromptText();

        pickupPromptText.gameObject.SetActive(
            true
        );
    }

    private void SetCurrentBerryBush(
    BerryBushHarvest berryBush
)
    {
        currentPickup = null;
        currentLootContainer = null;
        currentBerryBush = berryBush;

        if (pickupContentText != null)
        {
            pickupContentText.gameObject.SetActive(
                false
            );
        }

        if (pickupPromptText == null)
            return;

        pickupPromptText.text =
            berryBush.GetPromptText();

        pickupPromptText.gameObject.SetActive(true);
    }

    private void ClearCurrentPickup()
    {
        currentPickup = null;
        currentBerryBush = null;
        currentLootContainer = null;

        if (pickupPromptText != null)
        {
            pickupPromptText.gameObject.SetActive(
                false
            );
        }

        if (pickupContentText != null)
        {
            pickupContentText.gameObject.SetActive(
                false
            );
        }
    }

    private void ShowInventoryFull()
    {
        if (inventoryFullText == null)
            return;

        if (inventoryFullCoroutine != null)
        {
            StopCoroutine(
                inventoryFullCoroutine
            );
        }

        inventoryFullCoroutine =
            StartCoroutine(
                ShowInventoryFullRoutine()
            );
    }

    private void HideInventoryFull()
    {
        if (inventoryFullCoroutine != null)
        {
            StopCoroutine(
                inventoryFullCoroutine
            );

            inventoryFullCoroutine = null;
        }

        if (inventoryFullText != null)
        {
            inventoryFullText.gameObject.SetActive(
                false
            );
        }
    }

    private System.Collections.IEnumerator
        ShowInventoryFullRoutine()
    {
        inventoryFullText.text =
            "Inventar Voll";

        inventoryFullText.gameObject.SetActive(
            true
        );

        yield return new WaitForSeconds(
            1.5f
        );

        inventoryFullText.gameObject.SetActive(
            false
        );

        inventoryFullCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null)
            return;

        Gizmos.DrawWireSphere(
            playerCamera.transform.position +
            playerCamera.transform.forward *
            cameraSearchRange,
            sphereCastRadius
        );
    }
}