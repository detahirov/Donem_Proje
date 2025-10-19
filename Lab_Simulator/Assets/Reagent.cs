using UnityEngine;

public enum ReagentType { Acid, Base, Indicator, Neutral }

[RequireComponent(typeof(Rigidbody))]
public class Reagent : MonoBehaviour
{
    public ReagentType reagentType = ReagentType.Neutral;
    public float concentration = 1f; // 0..10 gibi
    public Color reagentColor = Color.white; // görsel için
    public GameObject pourStreamPrefab; // isteðe baðlý: dökme efekti
    public AudioClip pourSound;

    [Header("Runtime")]
    public bool isPouring = false;

    Renderer rend;
    Rigidbody rb;
    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();
        ApplyColor();
    }

    public void ApplyColor()
    {
        if (rend != null)
        {
            // materyalin ana rengini ayarla
            rend.material.color = reagentColor;
        }
    }

    // Elden "use" ile dökmek istersen HandController.UseHeldItem -> held.SendMessage("OnUse", ...)
    public void OnUse(Transform targetContainer)
    {
        // Basit: instantiate küçük damla/stream ve býrak
        if (pourStreamPrefab != null && targetContainer != null)
        {
            var s = Instantiate(pourStreamPrefab, transform.position, Quaternion.identity);
            var ps = s.GetComponent<ParticleSystem>();
            s.transform.SetParent(null);
            // hedefe doðru yönlendirebilirsin (uyarlama gerekebilir)
            Destroy(s, 3f);
        }
    }
}
