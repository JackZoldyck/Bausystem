using UnityEngine;

public class PlayerTool : MonoBehaviour
{
    public GameObject axeObjectFP;
    public GameObject pickaxeObjectFP;
    public GameObject hammerObjectFP;
    public GameObject swordObjectFP;

    public GameObject axeObjectTP;
    public GameObject pickaxeObjectTP;
    public GameObject hammerObjectTP;
    public GameObject swordObjectTP;

    public HotbarUI hotbarUI;
    public AxeAnimation axeAnimation;
    public PickaxeAnimation pickaxeAnimation;
    public BuildManager buildManager;

    public bool hasAxe = false;
    public bool hasPickaxe = false;
    public bool hasHammer = false;
    public bool hasSword = false;

    public bool unlockedAxe = false;
    public bool unlockedPickaxe = false;
    public bool unlockedHammer = false;
    public bool unlockedSword = false;

    public int axeDamage = 1;
    public int pickaxeDamage = 1;
    public int swordDamage = 10;

    private int selectedHotbarIndex = -1;

    [SerializeField] private PlayerZoomCamera playerZoomCamera;
    [SerializeField] private Animator thirdPersonAnimator;

    void Start()
    {
        UpdateToolState();
    }

    void Update()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectHotbarSlot(i);
            }
        }
    }

    void SelectHotbarSlot(int index)
    {
        if (hotbarUI == null || hotbarUI.hotbarData == null)
            return;

        if (selectedHotbarIndex == index)
        {
            selectedHotbarIndex = -1;

            UnequipAllTools();

            hotbarUI.DeselectAll();

            return;
        }

        HotbarSlotData slot =
            hotbarUI.hotbarData.GetSlot(index);

        if (slot == null ||
            slot.IsEmpty() ||
            slot.item == null)
        {
            selectedHotbarIndex = -1;

            UnequipAllTools();

            hotbarUI.DeselectAll();

            return;
        }

        selectedHotbarIndex = index;

        hotbarUI.SetSelectedIndex(index);

        EquipTool(slot.item);
    }

    void EquipTool(ItemData item)
    {
        UnequipAllTools();

        if (item.itemType != ItemType.Tool)
            return;

        switch (item.toolType)
        {
            case ToolType.Axe:
                hasAxe = true;
                break;

            case ToolType.Pickaxe:
                hasPickaxe = true;
                break;

            case ToolType.Hammer:
                hasHammer = true;
                break;

            case ToolType.Sword:
                hasSword = true;
                break;
        }

        UpdateToolState();
    }

    void UnequipAllTools()
    {
        hasAxe = false;
        hasPickaxe = false;
        hasHammer = false;
        hasSword = false;

        if (buildManager != null)
        {
            buildManager.CancelBuildMode();
        }

        UpdateToolState();
    }

    void UpdateToolState()
    {
        bool firstPerson = true;

        if (playerZoomCamera != null)
        {
            firstPerson =
                playerZoomCamera.IsFirstPerson;
        }

        SetFirstPerson(firstPerson);
    }

    public void SetFirstPerson(bool firstPerson)
    {
        if (axeObjectFP != null)
        {
            axeObjectFP.SetActive(
                firstPerson && hasAxe
            );
        }

        if (pickaxeObjectFP != null)
        {
            pickaxeObjectFP.SetActive(
                firstPerson && hasPickaxe
            );
        }

        if (hammerObjectFP != null)
        {
            hammerObjectFP.SetActive(
                firstPerson && hasHammer
            );
        }

        if (swordObjectFP != null)
        {
            swordObjectFP.SetActive(
                firstPerson && hasSword
            );
        }

        if (axeObjectTP != null)
        {
            axeObjectTP.SetActive(
                !firstPerson && hasAxe
            );
        }

        if (pickaxeObjectTP != null)
        {
            pickaxeObjectTP.SetActive(
                !firstPerson && hasPickaxe
            );
        }

        if (hammerObjectTP != null)
        {
            hammerObjectTP.SetActive(
                !firstPerson && hasHammer
            );
        }

        if (swordObjectTP != null)
        {
            swordObjectTP.SetActive(
                !firstPerson && hasSword
            );
        }
    }

    public void GiveAxe()
    {
        unlockedAxe = true;

        UpdateToolState();
    }

    public void GivePickaxe()
    {
        unlockedPickaxe = true;

        UpdateToolState();
    }

    public void GiveHammer()
    {
        unlockedHammer = true;

        UpdateToolState();
    }

    public void GiveSword()
    {
        unlockedSword = true;

        UpdateToolState();
    }

    public void PlayAxeSwing()
    {
        if (!hasAxe)
            return;

        bool firstPerson = true;

        if (playerZoomCamera != null)
        {
            firstPerson =
                playerZoomCamera.IsFirstPerson;
        }

        if (firstPerson)
        {
            axeAnimation?.Swing();
        }
        else
        {
            if (thirdPersonAnimator != null)
            {
                thirdPersonAnimator.SetTrigger(
                    "AxeSwing"
                );
            }
        }
    }

    public void PlayPickaxeSwing()
    {
        if (!hasPickaxe)
            return;

        bool firstPerson = true;

        if (playerZoomCamera != null)
        {
            firstPerson =
                playerZoomCamera.IsFirstPerson;
        }

        if (firstPerson)
        {
            pickaxeAnimation?.Swing();
        }
        else
        {
            if (thirdPersonAnimator != null)
            {
                thirdPersonAnimator.SetTrigger(
                    "PickaxeSwing"
                );
            }
        }
    }
}