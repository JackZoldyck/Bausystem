using UnityEngine;

public enum ItemType
{
    Resource,
    Tool,
    Food
}

public enum ToolType
{
    None,
    Axe,
    Pickaxe,
    Hammer,
    Sword
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    public ItemType itemType = ItemType.Resource;
    public ToolType toolType = ToolType.None;

    public int maxStackSize = 64;

    [Header("Food")]
    public float healthBonus;
    public float staminaBonus;
    public float foodDuration = 300f;
}