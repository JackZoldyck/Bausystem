using TMPro;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Camera playerCamera;
    public float pickupRange = 3f;
    public InventoryGridUI inventoryGrid;
    public TMP_Text pickupPromptText;

    private PickupItem currentPickup;

    void Update()
    {
        CheckForPickup();

        if (currentPickup != null && Input.GetKeyDown(KeyCode.E))
        {
            currentPickup.Pickup(inventoryGrid);
            currentPickup = null;
            pickupPromptText.gameObject.SetActive(false);
        }
    }

    void CheckForPickup()
    {
        currentPickup = null;
        pickupPromptText.gameObject.SetActive(false);

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, ~0, QueryTriggerInteraction.Collide))
        {
            PickupItem pickup = hit.collider.GetComponent<PickupItem>();

            if (pickup != null)
            {
                currentPickup = pickup;
                pickupPromptText.text = pickup.GetPromptText();
                pickupPromptText.gameObject.SetActive(true);
            }
        }
    }
}