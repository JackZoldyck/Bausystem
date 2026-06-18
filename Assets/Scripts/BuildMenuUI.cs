using UnityEngine;

public class BuildMenuUI : MonoBehaviour
{
    public GameObject buildMenuPanel;

    void Start()
    {
        buildMenuPanel.SetActive(false);
    }

    
    public MonoBehaviour playerController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            bool active = !buildMenuPanel.activeSelf;

            buildMenuPanel.SetActive(active);

            Cursor.visible = active;
            Cursor.lockState = active
                ? CursorLockMode.None
                : CursorLockMode.Locked;

            if (playerController != null)
                playerController.enabled = !active;
        }
    }
    public void CloseMenu()
    {
        buildMenuPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null)
            playerController.enabled = true;
    }
}