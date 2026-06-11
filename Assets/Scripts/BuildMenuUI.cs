using UnityEngine;

public class BuildMenuUI : MonoBehaviour
{
    public GameObject buildMenuPanel;

    void Start()
    {
        buildMenuPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildMenuPanel.SetActive(!buildMenuPanel.activeSelf);
        }
    }
}