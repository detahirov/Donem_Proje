// Assets/Chemistry/Scripts/Core/Container.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Container : MonoBehaviour
{
    [Header("Liquid visuals")]
    public Renderer liquidRenderer;       // iç sývý mesh materyali
    public Transform surfacePoint;        // gaz/köpük spawn noktasý
    public Color defaultColor = new Color(0.2f, 0.5f, 0.9f, 0.7f);

    [Header("State")]
    public float temperature = 20f;
    public float volume = 0f;             // ml
    public float maxVolume = 250f;

    [System.Serializable]
    public class SubstanceStack { public SubstanceSO so; public float amount; }
    [SerializeField] List<SubstanceStack> contents = new();

    public IReadOnlyList<SubstanceStack> Contents => contents;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        if (liquidRenderer) liquidRenderer.material.color = defaultColor;
    }

    public void AddSubstance(SubstanceSO so, float amount)
    {
        var stack = contents.FirstOrDefault(c => c.so == so);
        if (stack == null)
        {
            stack = new SubstanceStack { so = so, amount = 0f };
            contents.Add(stack);
        }
        stack.amount += Mathf.Max(0f, amount);
        volume = Mathf.Clamp(volume + amount * 1000f, 0f, maxVolume); // kaba yaklaþým

        // pH/renk güncelle + reaksiyon dene
        ReactionManager.Instance.TryReact(this);
        UpdateLiquidColorByIndicators();
    }

    public bool Contains(SubstanceSO so) => contents.Any(c => c.so == so);

    public bool ContainsAll(SubstanceSO[] arr)
    {
        foreach (var s in arr) if (!Contains(s)) return false;
        return true;
    }

    public float GetPH()
    {
        // Basit oyun modeli: hacim aðýrlýklý pH ortalamasý
        if (contents.Count == 0) return 7f;
        float sum = 0f, total = 0f;
        foreach (var c in contents) { sum += c.so.pH * c.amount; total += c.amount; }
        return Mathf.Clamp(total > 0 ? sum / total : 7f, 0f, 14f);
    }

    void UpdateLiquidColorByIndicators()
    {
        // içerikte indicator varsa, ReactionManager’dan renk al
        Color? indicatorColor = ReactionManager.Instance.GetIndicatorColor(this);
        if (indicatorColor.HasValue && liquidRenderer)
        {
            StopAllCoroutines();
            StartCoroutine(EffectRoutines.LerpColor(liquidRenderer.material, indicatorColor.Value, 0.8f));
        }
    }

    // Reaktanlarý “kullan”
    public void Consume(SubstanceSO[] reactants)
    {
        foreach (var r in reactants)
        {
            var st = contents.FirstOrDefault(c => c.so == r);
            if (st != null)
            {
                st.amount -= Mathf.Max(0.01f, st.amount * 0.5f); // oyunsal: bir kýsmýný tüket
                if (st.amount <= 0.001f) contents.Remove(st);
            }
        }
    }
}
