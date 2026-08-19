using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveFood
{
    public ItemData item;
    public float remainingTime;
}

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseMaxHealth = 25f;
    public float baseMaxStamina = 50f;

    [Header("Current Stats")]
    public float currentHealth;
    public float currentStamina;

    [Header("Food Slots")]
    public int baseFoodSlots = 3;
    public int bonusFoodSlots = 0;
    public int biomeFoodSlotModifier = 0;

    public List<ActiveFood> activeFoods = new List<ActiveFood>();

    [Header("Regeneration")]
    public float healthRegenPerSecond = 1f;
    public float staminaRegenWithFood = 10f;
    public float staminaRegenEmptyStomach = 4f;
    public float staminaRegenDelay = 1f;
    private float staminaRegenTimer = 0f;

    void Start()
    {
        currentHealth = GetMaxHealth();
        currentStamina = GetMaxStamina();
    }

    void Update()
    {
        UpdateFoodTimers();
        Regenerate();
    }

    public int GetMaxFoodSlots()
    {
        return Mathf.Max(1, baseFoodSlots + bonusFoodSlots + biomeFoodSlotModifier);
    }

    public float GetMaxHealth()
    {
        float total = baseMaxHealth;

        foreach (ActiveFood food in activeFoods)
        {
            if (food.item != null)
                total += food.item.healthBonus;
        }

        return total;
    }

    public float GetMaxStamina()
    {
        float total = baseMaxStamina;

        foreach (ActiveFood food in activeFoods)
        {
            if (food.item != null)
                total += food.item.staminaBonus;
        }

        return total;
    }

    void UpdateFoodTimers()
    {
        for (int i = activeFoods.Count - 1; i >= 0; i--)
        {
            activeFoods[i].remainingTime -= Time.deltaTime;

            if (activeFoods[i].remainingTime <= 0f)
            {
                activeFoods.RemoveAt(i);

                currentHealth = Mathf.Clamp(currentHealth, 0f, GetMaxHealth());
                currentStamina = Mathf.Clamp(currentStamina, 0f, GetMaxStamina());
            }
        }
    }

    void Regenerate()
    {
        bool hasFood = activeFoods.Count > 0;

        if (hasFood)
            currentHealth += healthRegenPerSecond * Time.deltaTime;

        if (staminaRegenTimer > 0f)
        {
            staminaRegenTimer -= Time.deltaTime;

            currentHealth = Mathf.Clamp(
                currentHealth,
                0f,
                GetMaxHealth()
            );

            currentStamina = Mathf.Clamp(
                currentStamina,
                0f,
                GetMaxStamina()
            );

            return;
        }

        float staminaRegen = hasFood
            ? staminaRegenWithFood
            : staminaRegenEmptyStomach;

        currentStamina += staminaRegen * Time.deltaTime;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            GetMaxHealth()
        );

        currentStamina = Mathf.Clamp(
            currentStamina,
            0f,
            GetMaxStamina()
        );
    }

    public bool EatFood(ItemData foodItem)
    {
        if (foodItem == null || foodItem.itemType != ItemType.Food)
            return false;

        if (activeFoods.Count >= GetMaxFoodSlots())
        {
            return false;
        }

        ActiveFood activeFood = new ActiveFood();
        activeFood.item = foodItem;
        activeFood.remainingTime = foodItem.foodDuration;

        activeFoods.Add(activeFood);

        currentHealth = Mathf.Clamp(
            currentHealth + foodItem.healthBonus,
            0f,
            GetMaxHealth()
        );

        currentStamina = Mathf.Clamp(
            currentStamina + foodItem.staminaBonus,
            0f,
            GetMaxStamina()
        );
        return true;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, GetMaxHealth());
    }

    public void UseStamina(float amount)
    {
        currentStamina -= amount;

        currentStamina = Mathf.Clamp(
            currentStamina,
            0f,
            GetMaxStamina()
        );

        staminaRegenTimer = staminaRegenDelay;
    }
}