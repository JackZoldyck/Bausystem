using TMPro;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InventoryGridUI inventoryGrid;
    [SerializeField] private TMP_Text pickupPromptText;

    [Header("Pickup Detection")]
    [SerializeField, Min(0.1f)]
    private float pickupRange = 3f;

    [SerializeField, Min(0.01f)]
    private float sphereCastRadius = 0.3f;

    private PickupItem currentPickup;

    private void Update()
    {
        CheckForPickup();

        if (currentPickup != null &&
            Input.GetKeyDown(KeyCode.E))
        {
            PickupItem pickupToCollect = currentPickup;

            ClearCurrentPickup();

            pickupToCollect.Pickup(inventoryGrid);
        }
    }

    private void CheckForPickup()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.SphereCast(
            ray,
            sphereCastRadius,
            out RaycastHit hit,
            pickupRange,
            ~0,
            QueryTriggerInteraction.Collide))
        {
            PickupItem pickup =
                hit.collider.GetComponentInParent<PickupItem>();

            if (pickup != null)
            {
                SetCurrentPickup(pickup);
                return;
            }
        }

        ClearCurrentPickup();
    }

    private void SetCurrentPickup(PickupItem pickup)
    {
        currentPickup = pickup;

        if (pickupPromptText == null)
            return;

        pickupPromptText.text =
            pickup.GetPromptText();

        pickupPromptText.gameObject.SetActive(true);
    }

    private void ClearCurrentPickup()
    {
        currentPickup = null;

        if (pickupPromptText != null)
        {
            pickupPromptText.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null)
            return;

        Gizmos.DrawWireSphere(
            playerCamera.transform.position
            + playerCamera.transform.forward * pickupRange,
            sphereCastRadius
        );
    }
}