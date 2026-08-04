using UnityEngine;

public class UIStateManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject craftingPanel;
    public GameObject buildMenuPanel;
    public GameObject workbenchPanel;

    public GameObject survivalHUD;
    public GameObject staminaBar;

    public HotbarUI gameplayHotbar;
    public PlayerController playerController;


    public bool IsAnyMenuOpen()
    {
        return
            inventoryPanel.activeSelf ||
            craftingPanel.activeSelf ||
            buildMenuPanel.activeSelf ||
            workbenchPanel.activeSelf;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllMenus();
        }

        bool menuOpen = IsAnyMenuOpen();

        if (staminaBar != null && menuOpen)
            staminaBar.SetActive(false);

        if (gameplayHotbar != null)
            gameplayHotbar.SetVisible(!menuOpen);

        if (survivalHUD != null)
            survivalHUD.SetActive(!menuOpen);
    }

    public void CloseAllMenus()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (craftingPanel != null)
            craftingPanel.SetActive(false);

        if (buildMenuPanel != null)
            buildMenuPanel.SetActive(false);

        if (workbenchPanel != null)
            workbenchPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null)
            playerController.lookEnabled = true;
    }
}