using UnityEngine;

public class PlayerTool : MonoBehaviour
{
    public GameObject axeObject;
    public bool hasAxe = false;
    public int axeDamage = 1;

    void Start()
    {
        UpdateAxeState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleAxe();
        }
    }

    public void ToggleAxe()
    {
        hasAxe = !hasAxe;
        UpdateAxeState();
    }

    void UpdateAxeState()
    {
        if (axeObject != null)
            axeObject.SetActive(hasAxe);
    }

    public void GiveAxe()
    {
        hasAxe = true;
        UpdateAxeState();
    }
}