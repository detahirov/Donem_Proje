using UnityEngine;

public class HandController : MonoBehaviour
{
    [Header("General Interaction")]
    public Camera playerCamera;
    public float interactRange = 3f;
    public LayerMask interactLayerMask = ~0;
    public KeyCode interactKey = KeyCode.E;   // İstersen F yapabilirsin

    [Header("General Pickup System (Pickupable)")]
    public Transform handAttachPoint;         // normal pickupable'lar için eldeki nokta
    public KeyCode pickupKey = KeyCode.Mouse0;
    public KeyCode dropKey = KeyCode.Q;
    private Pickupable heldItem = null;

    [Header("Bottle System")]
    public LayerMask bottleLayer;            // sadece Bottle layer'ı seç
    public Transform bottleSocket;           // el içindeki BottleSocket
    public Transform exampleHeldBottle;      // BottleSocket altındaki referans şişe
    public KeyCode grabBottleKey = KeyCode.E;

    [Header("Hand Tilt (El Eğme)")]
    public Transform wristBone;              // HandRig altındaki Wrist kemiği
    public float wristTiltAngle = 70f;       // Y ekseninde ne kadar dönecek
    public float wristTiltSpeed = 8f;        // eğme için Lerp hızı
    public KeyCode tiltKey = KeyCode.R;      // R'ye basılı tutarak eğ

    [Header("Animation")]
    public Animator handAnimator;            // elin Animator'ı (HasBottle, Holding parametreleri olmalı)

    // Bottle state
    private Transform currentBottle;
    private Rigidbody currentBottleRb;
    private Collider currentBottleCol;
    private Vector3 heldLocalPos;
    private Quaternion heldLocalRot;

    // Wrist rotasyonu
    private Quaternion wristDefaultRot;

    void Start()
    {
        if (!playerCamera)
            playerCamera = Camera.main;

        // Referans şişeden elde tutuş pozunu al
        if (exampleHeldBottle != null)
        {
            heldLocalPos = exampleHeldBottle.localPosition;
            heldLocalRot = exampleHeldBottle.localRotation;

            // Bu şişe sadece referans, oyunda görünmesine gerek yok
            exampleHeldBottle.gameObject.SetActive(false);
        }

        // Wrist için başlangıç rotasyonu
        if (wristBone != null)
        {
            wristDefaultRot = wristBone.localRotation;
        }
    }

    void Update()
    {
        HandleBottleInput();
        HandleWristTilt();          // el boşken de, şişeliyken de çalışır

        // Şişe varken normal pickup yapma
        if (currentBottle == null)
        {
            HandleNormalPickup();
        }

        HandleInteractionInput();
    }

    // ============================================================
    //                      BOTTLE SYSTEM
    // ============================================================

    void HandleBottleInput()
    {
        // E:
        //  - elde şişe yoksa → şişe al
        //  - elde şişe varsa → kapağı aç/kapa
        if (Input.GetKeyDown(grabBottleKey))
        {
            if (currentBottle == null)
                TryGrabBottle();
            else
                ToggleBottleCap();
        }

        // Q → şişe bırakmak (elde şişe varsa)
        if (currentBottle != null && Input.GetKeyDown(dropKey))
        {
            DropBottle();
        }
    }

    void TryGrabBottle()
    {
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        if (!Physics.Raycast(ray, out RaycastHit hit, interactRange, bottleLayer))
            return;

        Transform bottle = hit.collider.transform;

        currentBottle = bottle;
        currentBottleRb = bottle.GetComponent<Rigidbody>();
        currentBottleCol = bottle.GetComponent<Collider>();

        if (currentBottleRb != null)
        {
            currentBottleRb.isKinematic = true;
            currentBottleRb.useGravity = false;
        }
        if (currentBottleCol != null)
        {
            currentBottleCol.enabled = false;
        }

        bottle.SetParent(bottleSocket);
        bottle.localPosition = heldLocalPos;
        bottle.localRotation = heldLocalRot;

        if (handAnimator != null)
            handAnimator.SetBool("HasBottle", true);

        // Şişeye held bilgisini gönder
        BottleController bc = bottle.GetComponent<BottleController>();
        if (bc != null)
        {
            bc.SetHeld(true);
        }
    }

    void DropBottle()
    {
        if (currentBottle == null) return;

        // BottleController'a held = false ve pourInput = false de
        BottleController bc = currentBottle.GetComponent<BottleController>();
        if (bc != null)
        {
            bc.SetHeld(false);
            bc.pourInput = false;
        }

        if (handAnimator != null)
            handAnimator.SetBool("HasBottle", false);

        currentBottle.SetParent(null);

        if (currentBottleRb != null)
        {
            currentBottleRb.isKinematic = false;
            currentBottleRb.useGravity = true;
        }
        if (currentBottleCol != null)
        {
            currentBottleCol.enabled = true;
        }

        currentBottle = null;
        currentBottleRb = null;
        currentBottleCol = null;

        // Bileği default rotasyona çek
        if (wristBone != null)
            wristBone.localRotation = wristDefaultRot;
    }

    void ToggleBottleCap()
    {
        if (currentBottle == null) return;

        BottleController bc = currentBottle.GetComponent<BottleController>();
        if (bc != null)
        {
            bc.ToggleCap();
        }
    }

    // R'ye basılı tutunca el + tuttuğu her şey Y ekseninde döner
    // ve R bilgisini şişeye pourInput olarak yollar
    void HandleWristTilt()
    {
        if (wristBone == null) return;

        bool isTilting = Input.GetKey(tiltKey);   // R'ye basılı mı?

        // R bilgisini şişeye gönder
        if (currentBottle != null)
        {
            var bc = currentBottle.GetComponent<BottleController>();
            if (bc != null)
                bc.pourInput = isTilting;
        }

        // Default rotasyon
        Quaternion targetRot = wristDefaultRot;

        if (isTilting)
        {
            // Y ekseninde sağa yatma
            targetRot = wristDefaultRot * Quaternion.Euler(0f, -wristTiltAngle, 0f);
        }

        wristBone.localRotation = Quaternion.Lerp(
            wristBone.localRotation,
            targetRot,
            Time.deltaTime * wristTiltSpeed
        );
    }

    // ============================================================
    //                    NORMAL PICKUP SYSTEM
    // ============================================================

    void HandleNormalPickup()
    {
        // Mouse0 ile almak
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldItem == null)
                TryPickUpItem();
        }

        // Q ile bırakmak
        if (Input.GetKeyDown(dropKey) && heldItem != null)
        {
            DropHeldItem();
        }
    }

    void TryPickUpItem()
    {
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayerMask))
        {
            Pickupable p = hit.collider.GetComponentInParent<Pickupable>();
            if (p != null)
            {
                PickUp(p);
            }
        }
    }

    public void PickUp(Pickupable p)
    {
        if (heldItem != null) return;
        if (currentBottle != null) return; // şişe varken normal item alma

        heldItem = p;
        p.OnPick(handAttachPoint);

        if (handAnimator != null)
            handAnimator.SetBool("Holding", true);
    }

    public void DropHeldItem()
    {
        if (heldItem == null) return;

        heldItem.OnDrop(Vector3.up * 2f); // yukarı hafif kuvvet
        heldItem = null;

        if (handAnimator != null)
            handAnimator.SetBool("Holding", false);
    }

    // ============================================================
    //                         INTERACT
    // ============================================================

    void HandleInteractionInput()
    {
        // Şişe eldeyken diğer interactleri devre dışı bırak
        if (currentBottle != null) return;

        if (Input.GetKeyDown(interactKey))
            TryInteractWithRay();
    }

    void TryInteractWithRay()
    {
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayerMask))
        {
            Interactable inter = hit.collider.GetComponentInParent<Interactable>();
            if (inter != null)
                inter.OnInteract(this);
        }
    }
}
