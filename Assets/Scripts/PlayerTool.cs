using UnityEngine;

public class PlayerTool : MonoBehaviour
{
    public GameObject axeObject;
    public GameObject pickaxeObject;
    public GameObject hammerObject;
    public PickaxeAnimation pickaxeAnimation;

    public bool hasAxe = false;
    public bool hasPickaxe = false;
    public bool hasHammer = false;

    public int axeDamage = 1;
    public int pickaxeDamage = 1;

    void Start()
    {
        UpdateToolState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hasAxe = !hasAxe;

            if (hasAxe)
            {
                hasPickaxe = false;
                hasHammer = false;
            }

            UpdateToolState();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hasPickaxe = !hasPickaxe;

            if (hasPickaxe)
            {
                hasAxe = false;
                hasHammer = false;
            }

            UpdateToolState();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            hasHammer = !hasHammer;

            if (hasHammer)
            {
                hasAxe = false;
                hasPickaxe = false;
            }

            UpdateToolState();
        }
    }

    void UpdateToolState()
    {
        if (axeObject != null)
            axeObject.SetActive(hasAxe);

        if (pickaxeObject != null)
            pickaxeObject.SetActive(hasPickaxe);

        if (hammerObject != null)
            hammerObject.SetActive(hasHammer);
    }

    public void GiveAxe()
    {
        hasAxe = true;
        hasPickaxe = false;
        UpdateToolState();
    }

    public void GivePickaxe()
    {
        hasPickaxe = true;
        hasAxe = false;
        UpdateToolState();
    }
}