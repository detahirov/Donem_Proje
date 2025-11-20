using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickupable : Interactable
{
    [Header("Pickup settings")]
    public bool canBePickedUp = true;
    public Vector3 inHandLocalPosition = Vector3.zero;
    public Vector3 inHandLocalEuler = Vector3.zero;
    public bool disablePhysicsWhenHeld = true;

    Rigidbody rb;
    Collider col;
    Transform originalParent;

    Vector3 originalScale;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        originalParent = transform.parent;
        originalScale = transform.localScale;
    }

    public override void OnInteract(HandController hand)
    {
        if (!canBePickedUp) return;
        hand.PickUp(this);
    }

    public void OnPick(Transform handAttachPoint)
    {
        // kopyasını kullanmak yerine objeyi parent yapıyoruz
        transform.SetParent(handAttachPoint, worldPositionStays: false);
        transform.localPosition = inHandLocalPosition;
        transform.localEulerAngles = inHandLocalEuler;

        if (disablePhysicsWhenHeld)
        {
            rb.isKinematic = true;
            if (col) col.isTrigger = true;
        }
    }
    public void OnDrop(Vector3 dropForce)
    {
        transform.SetParent(originalParent, worldPositionStays: true);
        transform.localScale = originalScale;
        if (disablePhysicsWhenHeld)
        {
            rb.isKinematic = false;
            if (col) col.isTrigger = false;
        }
        rb.AddForce(dropForce, ForceMode.Impulse);
    }
}
