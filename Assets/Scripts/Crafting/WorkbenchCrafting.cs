using UnityEngine;
using UnityEngine.UI;

public class WorkbenchCrafting : MonoBehaviour
{
    public InventoryGridUI inventoryGridUI;

    [Header("Resources")]
    public ItemData woodItem;
    public ItemData stoneItem;

    [Header("Tools")]
    public ItemData axeItem;
    public ItemData pickaxeItem;
    public ItemData hammerItem;

    [Header("Buttons")]
    public Button axeButton;
    public Button pickaxeButton;
    public Button hammerButton;

    void OnEnable()
    {
        UpdateButtonStates();
    }

    void Update()
    {
        UpdateButtonStates();
    }

    void UpdateButtonStates()
    {
        axeButton.interactable =
            inventoryGridUI.HasItem(woodItem, 3) &&
            inventoryGridUI.HasItem(stoneItem, 2);

        pickaxeButton.interactable =
            inventoryGridUI.HasItem(woodItem, 3) &&
            inventoryGridUI.HasItem(stoneItem, 3);

        hammerButton.interactable =
            inventoryGridUI.HasItem(woodItem, 3) &&
            inventoryGridUI.HasItem(stoneItem, 1);
    }

    public void CraftAxe()
    {
        if (!inventoryGridUI.HasItem(woodItem, 3) ||
            !inventoryGridUI.HasItem(stoneItem, 2))
            return;

        inventoryGridUI.RemoveItem(woodItem, 3);
        inventoryGridUI.RemoveItem(stoneItem, 2);

        inventoryGridUI.AddItem(axeItem, 1);
    }

    public void CraftPickaxe()
    {
        if (!inventoryGridUI.HasItem(woodItem, 3) ||
            !inventoryGridUI.HasItem(stoneItem, 3))
            return;

        inventoryGridUI.RemoveItem(woodItem, 3);
        inventoryGridUI.RemoveItem(stoneItem, 3);

        inventoryGridUI.AddItem(pickaxeItem, 1);
    }

    public void CraftHammer()
    {
        if (!inventoryGridUI.HasItem(woodItem, 3) ||
            !inventoryGridUI.HasItem(stoneItem, 1))
            return;

        inventoryGridUI.RemoveItem(woodItem, 3);
        inventoryGridUI.RemoveItem(stoneItem, 1);

        inventoryGridUI.AddItem(hammerItem, 1);
    }
}