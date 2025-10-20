using UnityEngine;

public enum ChemicalKind
{
    None,
    Water,
    Sodium,
    Potassium,
    Acid,
    Base,
    // ekleyeceksin...
}

[RequireComponent(typeof(Rigidbody))]
public class ChemicalItem : MonoBehaviour
{
    public string chemicalName = "Unknown";
    public ChemicalKind kind = ChemicalKind.None;
    public float amount = 1f; // genel miktar birimi (ör: gram veya ml)
    public bool isReactive = true; // patlayabilir veya reaksiyona girebilir

    // Görsel / materyal referansý
    public Renderer itemRenderer;

    void Reset()
    {
        itemRenderer = GetComponentInChildren<Renderer>();
    }

    public void SetColor(Color c)
    {
        if (itemRenderer != null)
        {
            // materyal instance yap
            if (itemRenderer.material != null)
                itemRenderer.material.color = c;
        }
    }
}
