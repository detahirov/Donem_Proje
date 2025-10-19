using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform handAttachPoint; // el içindeki attach point (inspector'da ver)
    public float interactRange = 3f;
    public LayerMask interactLayerMask = ~0; // default tüm layer'lar
    public Camera playerCamera;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode pickupKey = KeyCode.Mouse0; // sol týk ile al/ateþ
    public KeyCode dropKey = KeyCode.Q;
    public float dropForce = 2f;

    Pickupable heldItem = null;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    void Update()
    {
        CheckHoverAndInteract();
        HandleInput();
    }

    void CheckHoverAndInteract()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactRange, interactLayerMask))
        {
            Interactable inter = hit.collider.GetComponentInParent<Interactable>();
            if (inter != null)
            {
                // burada UI gösterebilirsin: "E to interact" yani inter.interactionName
                inter.OnHover();

                // görsel crosshair deðiþimi vs. (opsiyonel)
            }
        }
        else
        {
            // nothing hovered
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldItem == null) TryPickUpWithRay();
            else UseHeldItem(); // e.g. ateþ et veya kullan
        }

        if (Input.GetKeyDown(interactKey))
        {
            TryInteractWithRay();
        }

        if (Input.GetKeyDown(dropKey))
        {
            if (heldItem != null) DropHeldItem();
        }
    }

    void TryPickUpWithRay()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactRange, interactLayerMask))
        {
            Pickupable p = hit.collider.GetComponentInParent<Pickupable>();
            if (p != null)
            {
                PickUp(p);
            }
        }
    }

    void TryInteractWithRay()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactRange, interactLayerMask))
        {
            Interactable inter = hit.collider.GetComponentInParent<Interactable>();
            if (inter != null)
            {
                inter.OnInteract(this);
            }
        }
    }

    public void PickUp(Pickupable p)
    {
        if (heldItem != null) return;
        heldItem = p;
        p.OnPick(handAttachPoint);

        // eðer elin Animator'ý varsa, burada parametrelere set at
        var anim = GetComponentInChildren<Animator>();
        if (anim) anim.SetBool("Holding", true);
    }

    public void DropHeldItem()
    {
        if (heldItem == null) return;
        Vector3 forward = playerCamera.transform.forward;
        Vector3 force = forward * dropForce + Vector3.up * (dropForce * 0.2f);
        heldItem.OnDrop(force);
        heldItem = null;

        var anim = GetComponentInChildren<Animator>();
        if (anim) anim.SetBool("Holding", false);
    }

    public void UseHeldItem()
    {
        if (heldItem == null) return;

        // Ray hedefini al
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactRange, interactLayerMask))
        {
            var container = hit.collider.GetComponentInParent<Container>();
            if (container != null)
            {
                // heldItem Reagent ise container'a ekle (veya Reagent.OnUse çaðýr)
                var reagent = heldItem.GetComponent<Reagent>();
                if (reagent != null)
                {
                    // elden býrakmadan "dökme" mantýðý: direkt olarak container'a ekle
                    container.AddReagentFromHand(reagent, /*worldPos*/ hit.point);
                    heldItem = null; // eðer þiþenin boþalmasýný istiyorsan destroy vb.
                    var anim = GetComponentInChildren<Animator>();
                    if (anim) anim.SetTrigger("Pour");
                    return;
                }
            }
        }

        // fallback: debug
        Debug.Log("Kullanýlýyor: " + (heldItem ? heldItem.name : "Yok"));
    }

}
