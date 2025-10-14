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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        originalParent = transform.parent;
    }

    public override void OnInteract(HandController hand)
    {
        if (!canBePickedUp) return;
        hand.PickUp(this);
    }

    public void OnPick(Transform handAttachPoint)
    {
        // kopyasýný kullanmak yerine objeyi parent yapýyoruz
        Vector3 originalScale = transform.lossyScale; // global scale’i kaydet
        transform.SetParent(handAttachPoint, worldPositionStays: true); // world stays TRUE
        transform.localPosition = inHandLocalPosition;
        transform.localEulerAngles = inHandLocalEuler;
        transform.localScale = Vector3.one; // handAttachPoint'in scale'ini etkisiz kýl
        transform.localScale = transform.localScale / handAttachPoint.lossyScale.x; // extra önlem


        if (disablePhysicsWhenHeld)
        {
            rb.isKinematic = true;
            if (col) col.isTrigger = true;
        }
    }

    public void OnDrop(Vector3 dropForce)
    {
        transform.SetParent(originalParent, worldPositionStays: true);
        if (disablePhysicsWhenHeld)
        {
            rb.isKinematic = false;
            if (col) col.isTrigger = false;
        }
        rb.AddForce(dropForce, ForceMode.Impulse);
    }
}
