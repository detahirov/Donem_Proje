using UnityEngine;

public class BottleController : MonoBehaviour
{
    [Header("Cap")]
    public Transform cap;              // Şişe kapağı child objesi (Lid)
    public bool startsOpen = false;    // Başlangıçta açık mı?
    public float capOpenAngle = -90f;  // Kapağın X ekseninde ne kadar açılacağı

    [Header("Liquid")]
    public float liquidAmount = 100f;  // içindeki sıvı (keyfi bir birim)
    public float pourAngle = 50f;      // ELDEKİ NORMAL POZA GÖRE bu açıyı geçince dökmeye başla
    public float pourRate = 4f;        // saniyede ne kadar sıvı aksın (yavaş)

    [Header("Liquid Mesh (opsiyonel)")]
    public Transform liquidMesh;       // HCl prefabındaki "Liquid" objesi
    public float minFillY = 0.05f;     // tamamen boşken bile tabanda biraz sıvı kalsın

    [Header("FX (particle sıvı)")]
    public ParticleSystem pourFx;      // Şişe ağzına koyduğun particle system

    bool isHeld;
    bool isCapOpen;
    Vector3 capClosedEuler;
    Vector3 capOpenEuler;

    float maxLiquidAmount;
    Vector3 liquidScaleInit;

    // eldeki "normal" pozun up yönü
    Vector3 heldUpDirection;
    bool hasHeldUpDirection = false;

    // R tuşu bilgisi buraya geliyor (HandController set ediyor)
    [HideInInspector] public bool pourInput = false;

    void Awake()
    {
        // Kapak rotasyonları
        if (cap != null)
        {
            capClosedEuler = cap.localEulerAngles;
            capOpenEuler = capClosedEuler + new Vector3(capOpenAngle, 0f, 0f);
        }

        isCapOpen = startsOpen;
        UpdateCapTransform();

        // Liquid miktarı
        maxLiquidAmount = Mathf.Max(1f, liquidAmount);
        liquidAmount = Mathf.Clamp(liquidAmount, 0f, maxLiquidAmount);

        // Liquid mesh başlangıç scale'i
        if (liquidMesh != null)
        {
            liquidScaleInit = liquidMesh.localScale;
        }

        // Particle başta kapalı olsun
        if (pourFx != null)
        {
            var emission = pourFx.emission;
            emission.enabled = false;
            pourFx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    // HandController çağırıyor
    public void SetHeld(bool held)
    {
        // elde yeni tutulmaya başlandıysa, o anki yönü referans al
        if (held && !isHeld)
        {
            heldUpDirection = transform.up;
            hasHeldUpDirection = true;
        }

        if (!held)
        {
            hasHeldUpDirection = false;
            pourInput = false;
            StopPourFx();
        }

        isHeld = held;
    }

    // HandController E tuşunda çağırıyor
    public void ToggleCap()
    {
        isCapOpen = !isCapOpen;

        if (cap != null)
            cap.gameObject.SetActive(!isCapOpen);

        UpdateCapTransform();
    }


    void UpdateCapTransform()
    {
        if (cap == null) return;
        cap.localEulerAngles = isCapOpen ? capOpenEuler : capClosedEuler;
    }

    void Update()
    {
        HandlePourLogic();
        UpdateLiquidMesh();
    }

    void HandlePourLogic()
    {
        // Şartlar sağlanmıyorsa dökme
        if (!isHeld) { StopPourFx(); return; }
        if (!isCapOpen) { StopPourFx(); return; }
        if (!pourInput) { StopPourFx(); return; }   // R'ye basılmıyorsa akma
        if (liquidAmount <= 0f) { StopPourFx(); return; }

        if (!hasHeldUpDirection)
        {
            heldUpDirection = transform.up;
            hasHeldUpDirection = true;
        }

        // Şişenin ŞU ANKİ yönü ile ELDEKİ NORMAL yönü arasındaki açı
        float angle = Vector3.Angle(transform.up, heldUpDirection);

        if (angle > pourAngle)
        {
            // Dökme
            float poured = pourRate * Time.deltaTime;
            liquidAmount = Mathf.Max(0f, liquidAmount - poured);

            PlayPourFx();
        }
        else
        {
            StopPourFx();
        }
    }

    void PlayPourFx()
    {
        if (pourFx == null) return;

        var emission = pourFx.emission;
        emission.enabled = true;
        // görsel yoğunluk için biraz çarpan
        emission.rateOverTime = pourRate * 5f;

        if (!pourFx.isPlaying)
            pourFx.Play();
    }

    void StopPourFx()
    {
        if (pourFx == null) return;

        var emission = pourFx.emission;
        emission.enabled = false;

        if (pourFx.isPlaying)
            pourFx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    void UpdateLiquidMesh()
    {
        if (liquidMesh == null) return;

        // 0–1 arası doluluk
        float t = Mathf.Clamp01(liquidAmount / maxLiquidAmount);

        // Y ekseninde scale'i küçült (seviye azalsın)
        Vector3 s = liquidScaleInit;
        float minY = liquidScaleInit.y * minFillY;
        float maxY = liquidScaleInit.y;
        s.y = Mathf.Lerp(minY, maxY, t);

        liquidMesh.localScale = s;
    }
}
