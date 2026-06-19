using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public TMP_Text woodText;
    public PlayerInventory inventory;
    public BuildMenuUI buildMenuUI;

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

            if (active)
                UpdateUI();

            if (active && buildMenuUI != null)
            {
                buildMenuUI.CloseMenu();
            }
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
    }
}