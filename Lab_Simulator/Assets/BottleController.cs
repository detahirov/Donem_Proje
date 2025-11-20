using UnityEngine;

public class BottleController : MonoBehaviour
{
    [Header("Cap")]
    public Transform cap;              // Şişe kapağı child objesi
    public bool startsOpen = false;    // Başlangıçta açık mı?
    public float capOpenAngle = -90f;  // Kapağın X ekseninde ne kadar açılacağı

    [Header("Liquid")]
    public float liquidAmount = 100f;  // içindeki sıvı (keyfi bir birim)
    public float pourAngle = 60f;      // world up'a göre bu açıyı geçince dökmeye başla
    public float pourRate = 10f;       // saniyede ne kadar sıvı aksın

    [Header("FX (opsiyonel)")]
    public ParticleSystem pourFx;      // Alt uca koyacağın particle (istersen boş bırak)

    bool isHeld;
    bool isCapOpen;
    Vector3 capClosedEuler;
    Vector3 capOpenEuler;

    void Awake()
    {
        if (cap != null)
        {
            capClosedEuler = cap.localEulerAngles;
            capOpenEuler = capClosedEuler + new Vector3(capOpenAngle, 0f, 0f);
        }

        isCapOpen = startsOpen;
        UpdateCapTransform();
    }

    public void SetHeld(bool held)
    {
        isHeld = held;
        if (!held)
            StopPourFx();
    }

    public void ToggleCap()
    {
        isCapOpen = !isCapOpen;
        UpdateCapTransform();
    }

    void UpdateCapTransform()
    {
        if (cap == null) return;
        cap.localEulerAngles = isCapOpen ? capOpenEuler : capClosedEuler;
    }

    void Update()
    {
        // Şartlar sağlanmıyorsa dökme
        if (!isHeld) return;
        if (!isCapOpen) return;
        if (liquidAmount <= 0f) return;

        // Şişenin "yukarı" yönü ile dünya yukarısı arasındaki açı
        float angle = Vector3.Angle(transform.up, Vector3.up);

        if (angle > pourAngle)
        {
            // Dökme
            float poured = pourRate * Time.deltaTime;
            liquidAmount = Mathf.Max(0f, liquidAmount - poured);

            if (pourFx && !pourFx.isPlaying)
                pourFx.Play();

            // TODO: Burada aşağıya doğru raycast atıp altındaki Container'a
            // hacim ekleyebilirsin. Şimdilik sadece dökülüyor varsayıyoruz.
            // Örn:
            // Ray ray = new Ray(transform.position, -transform.up);
            // if (Physics.Raycast(ray, out RaycastHit hit, 1.0f))
            // {
            //     Container c = hit.collider.GetComponentInParent<Container>();
            //     if (c) c.AddLiquid(someSubstance, poured);
            // }
        }
        else
        {
            StopPourFx();
        }
    }

    void StopPourFx()
    {
        if (pourFx && pourFx.isPlaying)
            pourFx.Stop();
    }
}
