using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public TMP_Text woodText;
    public PlayerInventory inventory;
    public BuildMenuUI buildMenuUI;
    public PlayerController playerController;
    public bool IsOpen()
    {
        return inventoryPanel.activeSelf;
    }

    void Start()
    {
        inventoryPanel.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool active = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(active);

            Cursor.visible = active;
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;

            if (playerController != null)
                playerController.lookEnabled = !active;

            if (active)
                UpdateUI();
        }
    }

    public void UpdateUI()
    {
        Debug.Log("UpdateUI aufgerufen");

        woodText.text = "Holz: " + inventory.wood;
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