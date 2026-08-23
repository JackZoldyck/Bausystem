using UnityEngine;

[System.Serializable]
public class CraftingIngredient
{
    public ItemData item;
    public int amount;
}

[CreateAssetMenu(fileName = "NewCraftingRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Recipe Info")]
    public string recipeName;
    public Sprite recipeIcon;

    [Header("Result")]
    public ItemData resultItem;
    public int resultAmount = 1;

    [Header("Ingredients")]
    public CraftingIngredient[] ingredients;

    [Header("Requirements")]
    public bool requiresWorkbench = false;
}