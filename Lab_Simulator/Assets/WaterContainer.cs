using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WaterContainer : MonoBehaviour
{
    [Header("Water visuals")]
    public Renderer waterRenderer; // assign the mesh renderer that shows water (material's color will change)
    public Color baseColor = new Color(0.2f, 0.5f, 0.9f, 0.6f);
    public Color reactionColor = new Color(0.6f, 0.1f, 0.1f, 0.7f);

    [Header("Water state")]
    public bool isFull = true;
    public float waterVolume = 1f; // just demo

    bool reacting = false;

    public void AddChemical(Chemical chemical)
    {
        if (reacting) return; // basit: sadece bir reaksiyon göster
        StartCoroutine(HandleChemical(chemical));
    }

    System.Collections.IEnumerator HandleChemical(Chemical chemical)
    {
        reacting = true;

        // small delay to make feel natural (düþeyde düþüp suya girsin)
        yield return new WaitForSeconds(chemical.reactionDelay);

        // get world point of contact (approx)
        Vector3 contactPoint = chemical.transform.position;

        // call central reaction manager
        ReactionManager.Instance.StartReaction(chemical, this, contactPoint);

        // optionally disable the chemical object (it "reacts away")
        chemical.MarkReacted();
        // hide or destroy the reagent
        Destroy(chemical.gameObject, 0.05f);

        // temporarily tint water color
        if (waterRenderer)
        {
            StartCoroutine(TintWater());
        }

        // finish
        yield return new WaitForSeconds(2.5f);
        reacting = false;
    }

    System.Collections.IEnumerator TintWater()
    {
        if (waterRenderer == null) yield break;
        Material mat = waterRenderer.material;
        Color start = mat.color;
        float dur = 1.2f;
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            mat.color = Color.Lerp(start, reactionColor, t / dur);
            yield return null;
        }
        // fade back
        t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            mat.color = Color.Lerp(reactionColor, start, t / dur);
            yield return null;
        }
        mat.color = start;
    }
}
