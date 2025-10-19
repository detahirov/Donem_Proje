using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionManager : MonoBehaviour
{
    public static ReactionManager Instance;

    [Header("Prefabs & VFX")]
    public GameObject bubblesPrefab;
    public GameObject steamPrefab;
    public GameObject fizzSplashPrefab;
    public AudioClip sfxFizz;
    public AudioClip sfxPop;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Basit kurallar: acid+base -> fizz + color change + gas
    public void TryReact(Container container, Reagent added)
    {
        // hýzlý karar: eðer container'da daha önce opposite type varsa tetikle
        foreach (var r in container.contents)
        {
            if (r == added) continue;
            if (IsAcidBasePair(r, added))
            {
                StartCoroutine(HandleAcidBaseReaction(container, r, added));
                return;
            }
            // indicator ile karýþým gözlemi
            if (added.reagentType == ReagentType.Indicator || r.reagentType == ReagentType.Indicator)
            {
                StartCoroutine(HandleIndicatorReaction(container, r, added));
            }
        }
    }

    bool IsAcidBasePair(Reagent a, Reagent b)
    {
        return (a.reagentType == ReagentType.Acid && b.reagentType == ReagentType.Base)
               || (a.reagentType == ReagentType.Base && b.reagentType == ReagentType.Acid);
    }

    IEnumerator HandleAcidBaseReaction(Container container, Reagent a, Reagent b)
    {
        // 1) küçük patlama / fizz partikle
        if (bubblesPrefab) Instantiate(bubblesPrefab, container.transform.position + Vector3.up * 0.2f, Quaternion.identity);
        if (fizzSplashPrefab) Instantiate(fizzSplashPrefab, container.transform.position + Vector3.up * 0.1f, Quaternion.identity);
        PlaySfx(sfxFizz, container.transform.position);

        // 2) renk deðiþimi — rafine pH modeli yerine basit LERP: acid/base -> neutral color
        var renderer = container.liquidSurface != null ? container.liquidSurface.GetComponent<Renderer>() : null;
        Color startColor = renderer ? renderer.material.color : Color.white;
        Color target = Color.gray; // nötr sonuç (örnek)
        if (a.reagentType == ReagentType.Acid || b.reagentType == ReagentType.Acid) target = Color.green; // örnek
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 0.8f;
            if (renderer) renderer.material.color = Color.Lerp(startColor, target, t);
            yield return null;
        }

        // 3) gas çýkýþý (kabarcýk)
        if (steamPrefab) Instantiate(steamPrefab, container.transform.position + Vector3.up * 0.15f, Quaternion.identity);
        PlaySfx(sfxPop, container.transform.position);

        // 4) (opsiyonel) içerikleri deðiþtir: bazen bazý kimyasallar tükenir => remove reagent
        // örnek: her ikisi de silinsin, yerine Product býrakýlabilir
        yield return new WaitForSeconds(0.5f);
        container.contents.Remove(a);
        container.contents.Remove(b);
        Destroy(a.gameObject);
        Destroy(b.gameObject);

        // 5) son ürün býrak
        // istersen bir Product prefab instantiate edebilirsin; burayý ihtiyaca göre geniþlet
    }

    IEnumerator HandleIndicatorReaction(Container container, Reagent a, Reagent b)
    {
        // indicator ile karýþýnca renk deðiþimi hýzlý
        var renderer = container.liquidSurface != null ? container.liquidSurface.GetComponent<Renderer>() : null;
        if (renderer)
        {
            Color start = renderer.material.color;
            Color target = (a.reagentType == ReagentType.Indicator) ? a.reagentColor : b.reagentColor;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 2f;
                renderer.material.color = Color.Lerp(start, target, t);
                yield return null;
            }
        }
        yield return null;
    }

    void PlaySfx(AudioClip clip, Vector3 pos)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, pos, 1f);
    }
}
