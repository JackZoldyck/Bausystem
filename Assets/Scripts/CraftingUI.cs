using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    public GameObject craftingPanel;

    public PlayerInventory inventory;
    public PlayerTool playerTool;
    public InventoryUI inventoryUI;
    public BuildMenuUI buildMenuUI;
    public HotbarUI hotbarUI;
    public PlayerController playerController;
    public InventoryGridUI inventoryGridUI;
    public ItemData woodItem;
    public ItemData stoneItem;

    public ItemData axeItem;
    public ItemData pickaxeItem;
    public ItemData hammerItem;
    public Button axeButton;
    public Button pickaxeButton;
    public Button hammerButton;

    void Start()
    {
        craftingPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            bool active = !craftingPanel.activeSelf;

            if (active)
            {
                inventoryUI?.CloseInventory();
                buildMenuUI?.CloseMenu();
            }

            craftingPanel.SetActive(active);

            Cursor.visible = active;
            Cursor.lockState = active
                ? CursorLockMode.None
                : CursorLockMode.Locked;

            if (playerController != null)
                playerController.lookEnabled = !active;    
        }
       UpdateButtonStates();
    }

    void UpdateButtonStates()
    {
        axeButton.interactable =
            inventoryGridUI.HasItem(woodItem, 3) &&
            inventoryGridUI.HasItem(stoneItem, 2) &&
            !playerTool.unlockedAxe;

        pickaxeButton.interactable =
            inventoryGridUI.HasItem(woodItem, 3) &&
            inventoryGridUI.HasItem(stoneItem, 3) &&
            !playerTool.unlockedPickaxe;

        hammerButton.interactable =
            inventoryGridUI.HasItem(woodItem, 2) &&
            inventoryGridUI.HasItem(stoneItem, 1) &&
            !playerTool.unlockedHammer;
    }

    public void CraftAxe()
    {
        Debug.Log("CraftAxe gedrückt");

        if (!inventoryGridUI.HasItem(woodItem, 3) ||
            !inventoryGridUI.HasItem(stoneItem, 2))
        {
            Debug.Log("Nicht genug Ressourcen");
            return;
        }

        inventory.RemoveResources(3, 2);

        inventoryGridUI.RemoveItem(woodItem, 3);
        inventoryGridUI.RemoveItem(stoneItem, 2);

        Debug.Log("Axe Item: " + axeItem);

        inventoryGridUI.AddItem(axeItem, 1);
    }

    public void CraftPickaxe()
    {
        if (!inventoryGridUI.HasItem(woodItem, 3) ||
            !inventoryGridUI.HasItem(stoneItem, 3))
        {
            Debug.Log("Nicht genug Ressourcen");
            return;
        }

        inventory.RemoveResources(3, 3);

        inventoryGridUI.RemoveItem(woodItem, 3);
        inventoryGridUI.RemoveItem(stoneItem, 3);

        inventoryGridUI.AddItem(pickaxeItem, 1);
    }

    public void CraftHammer()
    {
        if (!inventoryGridUI.HasItem(woodItem, 2) ||
            !inventoryGridUI.HasItem(stoneItem, 1))
        {
            Debug.Log("Nicht genug Ressourcen");
            return;
        }

        inventory.RemoveResources(2, 1);

        inventoryGridUI.RemoveItem(woodItem, 2);
        inventoryGridUI.RemoveItem(stoneItem, 1);

        inventoryGridUI.AddItem(hammerItem, 1);
    }
}