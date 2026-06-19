using UnityEngine;

public class PlayerTool : MonoBehaviour
{
    public GameObject axeObject;
    public bool hasAxe = true;
    public int axeDamage = 1;

    void Start()
    {
        if (axeObject != null)
            axeObject.SetActive(hasAxe);
    }

    public void GiveAxe()
    {
        hasAxe = true;

        if (axeObject != null)
            axeObject.SetActive(true);
    }
}
