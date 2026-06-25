using UnityEngine;

public class BuildMenuUI : MonoBehaviour
{
    public GameObject buildMenuPanel;
    public InventoryUI inventoryUI;
    public PlayerTool playerTool;

    void Start()
    {
        buildMenuPanel.SetActive(false);
    }

    
    public MonoBehaviour playerController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (playerTool == null || !playerTool.hasHammer)
                return;

            bool active = !buildMenuPanel.activeSelf;

            if (active && inventoryUI != null)
                inventoryUI.CloseInventory();

            buildMenuPanel.SetActive(active);

            Cursor.visible = active;
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;

            if (playerController != null)
                playerController.enabled = !active;
        }
    }
    public void CloseMenu()
    {
        buildMenuPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null)
            playerController.enabled = true;
    }
}