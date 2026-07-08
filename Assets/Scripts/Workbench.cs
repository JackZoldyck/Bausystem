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

        Debug.Log("Player gefunden: " + player);

        workbenchUI =
            FindAnyObjectByType<WorkbenchUI>(
                FindObjectsInactive.Include
            )?.gameObject;

        Debug.Log("Workbench UI gefunden: " + workbenchUI);

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
            Debug.Log("E wurde gedrückt");
            Debug.Log("Distanz zur Werkbank: " + distance);

            if (distance <= interactDistance)
            {
                Debug.Log("ToggleUI wird aufgerufen!");
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