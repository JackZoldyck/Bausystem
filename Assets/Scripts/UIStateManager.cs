using UnityEngine;

public class UIStateManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject craftingPanel;
    public GameObject buildMenuPanel;
    public GameObject survivalHUD;
    public GameObject staminaBar;

    public HotbarUI gameplayHotbar;

    void Update()
    {
        bool menuOpen =
            inventoryPanel.activeSelf ||
            craftingPanel.activeSelf ||
            buildMenuPanel.activeSelf;
        if (staminaBar != null && menuOpen)
            staminaBar.SetActive(false);

        gameplayHotbar.SetVisible(!menuOpen);

        if (survivalHUD != null)
            survivalHUD.SetActive(!menuOpen);
    }
}