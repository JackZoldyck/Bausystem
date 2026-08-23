using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorkbenchCrafting : MonoBehaviour
{
    public InventoryGridUI inventoryGridUI;

    public ToolGainPopup toolGainPopup;

    [Header("Resources")]
    public ItemData woodItem;
    public ItemData stoneItem;

    [Header("Tools")]
    public ItemData axeItem;
    public ItemData pickaxeItem;
    public ItemData hammerItem;
    public ItemData swordItem;

    [Header("Buttons")]
    public Button axeButton;
    public Button pickaxeButton;
    public Button hammerButton;
    public Button swordButton;

    [Header("Sword Cost")]
    public int swordWoodCost = 2;
    public int swordStoneCost = 5;

    [Header("Craft Feedback")]
    public Image axeCraftFill;
    public Image pickaxeCraftFill;
    public Image hammerCraftFill;
    public Image swordCraftFill;

    public float craftFeedbackDuration = 0.6f;


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
        if (axeButton != null)
        {
            axeButton.interactable =
                inventoryGridUI.HasItem(woodItem, 3) &&
                inventoryGridUI.HasItem(stoneItem, 2);
        }

        if (pickaxeButton != null)
        {
            pickaxeButton.interactable =
                inventoryGridUI.HasItem(woodItem, 3) &&
                inventoryGridUI.HasItem(stoneItem, 3);
        }

        if (hammerButton != null)
        {
            hammerButton.interactable =
                inventoryGridUI.HasItem(woodItem, 3) &&
                inventoryGridUI.HasItem(stoneItem, 1);
        }

        if (swordButton != null)
        {
            swordButton.interactable =
                inventoryGridUI.HasItem(
                    woodItem,
                    swordWoodCost
                ) &&
                inventoryGridUI.HasItem(
                    stoneItem,
                    swordStoneCost
                );
        }
    }

    public void CraftAxe()
    {
        if (!inventoryGridUI.HasItem(woodItem, 3) ||
            !inventoryGridUI.HasItem(stoneItem, 2))
        {
            return;
        }

        inventoryGridUI.RemoveItem(
            woodItem,
            3
        );

        inventoryGridUI.RemoveItem(
            stoneItem,
            2
        );

        inventoryGridUI.AddItem(
            axeItem,
            1
        );

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

        inventoryGridUI.RemoveItem(
            woodItem,
            3
        );

        inventoryGridUI.RemoveItem(
            stoneItem,
            3
        );

        inventoryGridUI.AddItem(
            pickaxeItem,
            1
        );

        StartCoroutine(CraftFeedbackRoutine(pickaxeCraftFill));

        toolGainPopup?.ShowToolGain(
            pickaxeItem.itemName
        );
    }

    public void CraftHammer()
    {
        if (!inventoryGridUI.HasItem(woodItem, 3) ||
            !inventoryGridUI.HasItem(stoneItem, 1))
        {
            return;
        }

        inventoryGridUI.RemoveItem(
            woodItem,
            3
        );

        inventoryGridUI.RemoveItem(
            stoneItem,
            1
        );

        inventoryGridUI.AddItem(
            hammerItem,
            1
        );

        StartCoroutine(CraftFeedbackRoutine(hammerCraftFill));

        toolGainPopup?.ShowToolGain(
            hammerItem.itemName
        );
    }

    public void CraftSword()
    {
        if (!inventoryGridUI.HasItem(
                woodItem,
                swordWoodCost) ||
            !inventoryGridUI.HasItem(
                stoneItem,
                swordStoneCost))
        {
            return;
        }

        inventoryGridUI.RemoveItem(
            woodItem,
            swordWoodCost
        );

        inventoryGridUI.RemoveItem(
            stoneItem,
            swordStoneCost
        );

        inventoryGridUI.AddItem(
            swordItem,
            1
        );

        StartCoroutine(CraftFeedbackRoutine(swordCraftFill));

        toolGainPopup?.ShowToolGain(
            swordItem.itemName
        );
    }

    private IEnumerator CraftFeedbackRoutine(Image fillImage)
    {
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