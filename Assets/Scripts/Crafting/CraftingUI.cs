using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CraftingUI : MonoBehaviour
{
    public GameObject craftingPanel;

    public ToolGainPopup toolGainPopup;

    public PlayerInventory inventory;
    public PlayerTool playerTool;
    public InventoryUI inventoryUI;
    public BuildMenuUI buildMenuUI;
    public HotbarUI hotbarUI;
    public PlayerController playerController;
    public InventoryGridUI inventoryGridUI;
    public UIStateManager uiStateManager;
    public ItemData woodItem;
    public ItemData stoneItem;

    public ItemData axeItem;
    public ItemData pickaxeItem;
    public ItemData hammerItem;
    public Button axeButton;
    public Button pickaxeButton;
    public Button hammerButton;

    [Header("Craft Feedback")]
    public Image axeCraftFill;
    public Image pickaxeCraftFill;
    public Image hammerCraftFill;

    public float craftFeedbackDuration = 0.6f;

    void Start()
    {
        craftingPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            bool active = !craftingPanel.activeSelf;

            if (active && uiStateManager != null && uiStateManager.IsAnyMenuOpen())
                return;

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

        if (!inventoryGridUI.HasItem(woodItem, 3) ||
            !inventoryGridUI.HasItem(stoneItem, 2))
        {
            return;
        }

        inventory.RemoveResources(3, 2);

        inventoryGridUI.RemoveItem(woodItem, 3);
        inventoryGridUI.RemoveItem(stoneItem, 2);

        inventoryGridUI.AddItem(axeItem, 1);

        StartCoroutine(CraftFeedbackRoutine(axeCraftFill));

        toolGainPopup?.ShowToolGain(
            axeItem.itemName
        );
    }

    public void CraftPickaxe()
    {
        if (!inventoryGridUI.HasItem(woodItem, 3) ||
            !inventoryGridUI.HasItem(stoneItem, 3))
        {
            return;
        }

        inventory.RemoveResources(3, 3);

        inventoryGridUI.RemoveItem(woodItem, 3);
        inventoryGridUI.RemoveItem(stoneItem, 3);

        inventoryGridUI.AddItem(pickaxeItem, 1);

        StartCoroutine(CraftFeedbackRoutine(pickaxeCraftFill));

        toolGainPopup?.ShowToolGain(
            pickaxeItem.itemName
        );
    }

    public void CraftHammer()
    {
        if (!inventoryGridUI.HasItem(woodItem, 2) ||
            !inventoryGridUI.HasItem(stoneItem, 1))
        {
            return;
        }

        inventory.RemoveResources(2, 1);

        inventoryGridUI.RemoveItem(woodItem, 2);
        inventoryGridUI.RemoveItem(stoneItem, 1);

        inventoryGridUI.AddItem(hammerItem, 1);

        StartCoroutine(CraftFeedbackRoutine(hammerCraftFill));

        toolGainPopup?.ShowToolGain(
            hammerItem.itemName
        );
    }

    private IEnumerator CraftFeedbackRoutine(Image fillImage)
    {
        if (fillImage == null)
            yield break;

        fillImage.fillAmount = 0f;

        float elapsed = 0f;

        while (elapsed < craftFeedbackDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            fillImage.fillAmount = Mathf.Clamp01(
                elapsed / craftFeedbackDuration
            );

            yield return null;
        }

        fillImage.fillAmount = 1f;

        yield return new WaitForSecondsRealtime(0.1f);

        fillImage.fillAmount = 0f;
    }
}