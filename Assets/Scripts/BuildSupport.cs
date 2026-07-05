using UnityEngine;
using System.Collections.Generic;

public class BuildSupport : MonoBehaviour
{
    public BuildableObject supportedBy;
    public List<BuildSupport> supportedObjects = new();

    private Rigidbody rb;
    private bool isFallingBecauseUnsupported;

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

        BuildSupport supportComponent =
            supportObject.GetComponent<BuildSupport>();

        if (supportComponent != null)
        {
            supportComponent.supportedObjects.Add(this);
        }
    }

    public void RemoveSupport()
    {
        supportedBy = null;

        foreach (BuildSupport supportedObject in supportedObjects)
        {
            if (supportedObject != null)
            {
                supportedObject.RemoveSupport();
            }
        }

        supportedObjects.Clear();

        Fall();
    }

    void Fall()
    {
        isFallingBecauseUnsupported = true;

        rb.isKinematic = false;
        rb.useGravity = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isFallingBecauseUnsupported)
            return;

        if (collision.gameObject.CompareTag("Ground") ||
    collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            BuildRefund refund = GetComponent<BuildRefund>();

            if (refund != null)
            {
                refund.Refund();
            }

            Destroy(gameObject);
        }
    }
}