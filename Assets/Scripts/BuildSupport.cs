using UnityEngine;

public class BuildSupport : MonoBehaviour
{
    public BuildableObject supportedBy;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void SetSupport(BuildableObject supportObject)
    {
        supportedBy = supportObject;
    }

    public void RemoveSupport()
    {
        supportedBy = null;
        Fall();
    }

    void Fall()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }
}