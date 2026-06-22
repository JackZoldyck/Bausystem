using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public BuildMenuUI buildMenuUI;
    public PlayerController playerController;
    public bool IsOpen()
    {
        return inventoryPanel.activeSelf;
    }

    void Start()
    {
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool active = !inventoryPanel.activeSelf;

            if (active && buildMenuUI != null)
                buildMenuUI.CloseMenu();

            inventoryPanel.SetActive(active);

            Cursor.visible = active;
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;

            if (playerController != null)
                playerController.lookEnabled = !active;
        }
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null)
            playerController.lookEnabled = true;
    }
}