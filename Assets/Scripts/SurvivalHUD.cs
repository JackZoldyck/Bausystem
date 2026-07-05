using UnityEngine;

public class SurvivalHUD : MonoBehaviour
{
    public PlayerStats playerStats;

    public GameObject inventoryPanel;
    public GameObject craftingPanel;
    public GameObject buildMenuPanel;

    public RectTransform healthBar;
    public RectTransform healthFill;

    public float baseHealthBarWidth = 160f;
    public float widthPerHealth = 4f;

    public RectTransform staminaBar;
    public RectTransform staminaFill;

    public float baseStaminaBarWidth = 160f;
    public float widthPerStamina = 2f;

    public FoodSlotUI[] foodSlots;

    public GameObject staminaBarObject;
    public float staminaVisibleDuration = 1.5f;

    private float lastStaminaValue;
    private float lastMaxStaminaValue;
    private float staminaVisibleTimer;


    void Start()
    {
        if (playerStats != null)
        {
            lastStaminaValue = playerStats.currentStamina;
            lastMaxStaminaValue = playerStats.GetMaxStamina();
        }

        if (staminaBarObject != null)
            staminaBarObject.SetActive(false);
    }

    void Update()
    {
        bool menuOpen =
            (inventoryPanel != null && inventoryPanel.activeSelf) ||
            (craftingPanel != null && craftingPanel.activeSelf) ||
            (buildMenuPanel != null && buildMenuPanel.activeSelf);

        if (menuOpen)
        {
            if (staminaBarObject != null)
                staminaBarObject.SetActive(false);

            return;
        }

        if (playerStats == null)
            return;

        float maxHealth = playerStats.GetMaxHealth();
        float healthPercent = playerStats.currentHealth / maxHealth;

        float targetWidth =
            baseHealthBarWidth +
            ((maxHealth - playerStats.baseMaxHealth) * widthPerHealth);

        healthBar.sizeDelta =
            new Vector2(targetWidth, healthBar.sizeDelta.y);

        healthFill.localScale =
            new Vector3(healthPercent, 1f, 1f);

        UpdateFoodSlots();

        if (staminaBar != null && staminaFill != null)
        {
            float maxStamina = playerStats.GetMaxStamina();
            float staminaPercent = playerStats.currentStamina / maxStamina;

            float staminaWidth =
                baseStaminaBarWidth +
                ((maxStamina - playerStats.baseMaxStamina) * widthPerStamina);

            staminaBar.sizeDelta =
                new Vector2(staminaWidth, staminaBar.sizeDelta.y);

            staminaFill.localScale =
                new Vector3(staminaPercent, 1f, 1f);
        }
        bool staminaChanged =
            Mathf.Abs(playerStats.currentStamina - lastStaminaValue) > 0.01f ||
            Mathf.Abs(playerStats.GetMaxStamina() - lastMaxStaminaValue) > 0.01f;

        if (staminaChanged)
        {
            staminaVisibleTimer = staminaVisibleDuration;

            if (staminaBarObject != null)
                staminaBarObject.SetActive(true);
        }

        staminaVisibleTimer -= Time.deltaTime;

        if (staminaVisibleTimer <= 0f && staminaBarObject != null)
            staminaBarObject.SetActive(false);

        lastStaminaValue = playerStats.currentStamina;
        lastMaxStaminaValue = playerStats.GetMaxStamina();
    }

    void UpdateFoodSlots()
    {
        if (foodSlots == null)
            return;

        for (int i = 0; i < foodSlots.Length; i++)
        {
            if (foodSlots[i] == null)
                continue;

            if (i < playerStats.activeFoods.Count)
                foodSlots[i].SetFood(playerStats.activeFoods[i]);
            else
                foodSlots[i].Clear();
        }
    }
}