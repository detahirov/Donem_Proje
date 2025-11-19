// Assets/Chemistry/Scripts/Core/ReactionManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReactionManager : MonoBehaviour
{
    public static ReactionManager Instance { get; private set; }

    [Header("Databases")]
    public List<ReactionSO> reactions = new();
    public List<IndicatorSO> indicators = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void TryReact(Container container)
    {
        float ph = container.GetPH();
        var list = reactions;

        foreach (var rx in list)
        {
            // Koþul: reaktanlarýn hepsi var mý?
            if (!container.ContainsAll(rx.reactants)) continue;

            // Sýcaklýk ve diðer koþullar
            if (container.temperature < rx.minTemp || container.temperature > rx.maxTemp) continue;

            // Likit kap gereksinimi (beher/erlen zaten likit)
            // Ýstersen kap tipine göre flag ekleyebilirsin

            // Eþleþti: uygula
            ApplyReaction(rx, container);
            break; // tek seferde bir reaksiyon
        }
    }

    void ApplyReaction(ReactionSO rx, Container container)
    {
        foreach (var eff in rx.effects)
        {
            switch (eff.type)
            {
                case ReactionEffectType.ColorChange:
                    if (container.liquidRenderer)
                        StartCoroutine(EffectRoutines.LerpColor(container.liquidRenderer.material, eff.targetColor, eff.colorLerpTime));
                    break;

                case ReactionEffectType.GasRelease:
                    if (eff.gasPrefab && container.surfacePoint)
                        StartCoroutine(EffectRoutines.EmitGas(eff.gasPrefab, container.surfacePoint, eff.gasRate, eff.gasDuration));
                    break;

                case ReactionEffectType.HeatChange:
                    container.temperature += eff.heatDelta;
                    break;

                case ReactionEffectType.Foam:
                    if (eff.foamPrefab && container.surfacePoint)
                        StartCoroutine(EffectRoutines.SpawnTimed(eff.foamPrefab, container.surfacePoint.position, eff.foamDuration));
                    break;

                case ReactionEffectType.Explosion:
                    if (eff.explosionPrefab)
                        Instantiate(eff.explosionPrefab, container.transform.position, Quaternion.identity);
                    // fiziksel itme
                    var hits = Physics.OverlapSphere(container.transform.position, eff.explosionRadius);
                    foreach (var h in hits)
                    {
                        var rb = h.attachedRigidbody;
                        if (rb != null && !rb.isKinematic)
                            rb.AddExplosionForce(eff.explosionForce, container.transform.position, eff.explosionRadius);
                    }
                    // kamera shake
                    var cam = Camera.main;
                    var shake = cam ? cam.GetComponent<CameraShake>() : null;
                    if (shake) shake.Shake(0.25f, 0.5f);
                    if (eff.explosionSfx) AudioSource.PlayClipAtPoint(eff.explosionSfx, container.transform.position);
                    break;

                case ReactionEffectType.Decolorize:
                    if (container.liquidRenderer)
                        StartCoroutine(EffectRoutines.LerpColor(container.liquidRenderer.material, Color.clear, eff.colorLerpTime));
                    break;

                case ReactionEffectType.Precipitate:
                    // Ýstersen çökelti prefabý ekleyip yüzeyin altýna spawn edebilirsin
                    break;
            }
        }

        if (rx.consumeAllReactants) container.Consume(rx.reactants);
        // products eklemek istersen burada container.AddSubstance(...) ile ekleyebilirsin
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.NotifyReactionPerformed(rx);
        }
    }

    // Gösterge rengi: container’da bir indicator var ise pH’a göre renk döndür
    public Color? GetIndicatorColor(Container container)
    {
        // içerikte indicator olan var mý?
        var indicatorStacks = container.Contents.Where(s => s.so.isIndicator).ToList();
        if (indicatorStacks.Count == 0) return null;

        float ph = container.GetPH();
        foreach (var ind in indicators)
        {
            if (indicatorStacks.Any(s => s.so == ind.indicator))
                return ind.GetColorForPH(ph);
        }
        return null;
    }
}
