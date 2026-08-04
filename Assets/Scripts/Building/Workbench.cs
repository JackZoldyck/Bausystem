using UnityEngine;

public class Workbench : MonoBehaviour
{
    public GameObject workbenchUI;
    public float interactDistance = 5f;

    private Transform player;
    private bool uiOpen;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        workbenchUI =
            FindAnyObjectByType<WorkbenchUI>(
                FindObjectsInactive.Include
            )?.gameObject;

        if (workbenchUI != null)
            workbenchUI.SetActive(false);
    }

    void Update()
    {
        if (player == null || workbenchUI == null)
            return;

        float distance =
            Vector3.Distance(
                GetComponentInChildren<Collider>().bounds.center,
                player.position
    );

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (distance <= interactDistance)
            {
                ToggleUI();
            }
        }

        if (uiOpen && distance > interactDistance)
        {
            CloseUI();
        }
    }

    void ToggleUI()
    {
        uiOpen = !uiOpen;

        workbenchUI.SetActive(uiOpen);

        Cursor.visible = uiOpen;

        Cursor.lockState =
            uiOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }

    void CloseUI()
    {
        uiOpen = false;

        workbenchUI.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}