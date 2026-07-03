using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public HotbarUI hotbarUI;
    public BuildMenuUI buildMenuUI;
    public PlayerController playerController;
    public GameObject craftingPanel;
    public GameObject buildMenuPanel;
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
            if ((craftingPanel != null && craftingPanel.activeSelf) ||
                (buildMenuUI != null && buildMenuUI.buildMenuPanel.activeSelf))
            {
                return;
            }

            bool active = !inventoryPanel.activeSelf;

            if (active && buildMenuUI != null)
                buildMenuUI.CloseMenu();

            inventoryPanel.SetActive(active);
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

        if (hotbarUI != null)
            hotbarUI.SetVisible(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null)
            playerController.lookEnabled = true;
    }
}