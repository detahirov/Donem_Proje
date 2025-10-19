using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Container : MonoBehaviour
{
    public List<Reagent> contents = new List<Reagent>();
    public float volume = 1f; // kap büyüklüðü (opsiyonel)
    public Transform liquidSurface; // (opsiyonel) sývý yüzey objesi scale/colora baðlý

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true; // trigger olmasý iþleri kolaylaþtýrýr
    }

    void OnTriggerEnter(Collider other)
    {
        var reagent = other.GetComponentInParent<Reagent>();
        if (reagent != null)
        {
            AddReagent(reagent);
        }
    }

    void AddReagent(Reagent r)
    {
        if (!contents.Contains(r))
        {
            contents.Add(r);
            r.transform.SetParent(this.transform, worldPositionStays: true);
            // konumu ayarlamak istersen: r.transform.localPosition = Vector3.zero + offset
            ReactionManager.Instance.TryReact(this, r);
            UpdateLiquidVisual();
        }
    }

    void UpdateLiquidVisual()
    {
        if (liquidSurface == null) return;

        // Basit: rengin ortalamasý
        Color c = Color.clear;
        foreach (var r in contents) c += r.reagentColor;
        c /= Mathf.Max(contents.Count, 1);
        var rend = liquidSurface.GetComponent<Renderer>();
        if (rend) rend.material.color = c;
    }
    public void AddReagentFromHand(Reagent r, Vector3 atPoint)
    {
        // r'yi kap içine parent et ve positionu ayarla
        r.transform.position = atPoint + Vector3.up * 0.05f;
        r.transform.SetParent(this.transform, worldPositionStays: true);
        if (!contents.Contains(r)) contents.Add(r);
        ReactionManager.Instance.TryReact(this, r);
        UpdateLiquidVisual();
    }

}
