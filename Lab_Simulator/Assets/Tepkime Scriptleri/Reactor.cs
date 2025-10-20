using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Reactor : Interactable
{
    public List<ChemicalItem> contents = new List<ChemicalItem>();
    public List<ReactionRecipe> recipes = new List<ReactionRecipe>();

    public Transform effectSpawnPoint;
    public Animator animator; // opsiyonel
    public float capacity = 100f; // max amount

    bool isReacting = false;

    void Reset()
    {
        interactionName = "Open";
    }

    public override void OnInteract(HandController hand)
    {
        // bir UI açma veya show contents yapabilirsin
        Debug.Log("Container interacted");
    }

    // HandController çaðýracak: AddChemical(heldItem)
    public bool AddChemical(ChemicalItem item)
    {
        if (item == null) return false;

        float currentTotal = 0f;
        foreach (var c in contents) currentTotal += c.amount;
        if (currentTotal + item.amount > capacity)
        {
            Debug.Log("Capacity exceeded");
            return false;
        }

        // parent item to container for visuals
        item.transform.SetParent(this.transform, worldPositionStays: true);
        contents.Add(item);

        // item artýk tutulmasýn (physics kapat)
        var rb = item.GetComponent<Rigidbody>();
        if (rb) { rb.isKinematic = true; }

        TryStartReaction();
        return true;
    }

    void TryStartReaction()
    {
        if (isReacting) return;

        // Basit approach: tüm recipes üzerinde kontrol et
        foreach (var recipe in recipes)
        {
            if (RecipeMatches(recipe))
            {
                StartCoroutine(RunReaction(recipe));
                return;
            }
        }
    }

    bool RecipeMatches(ReactionRecipe r)
    {
        // Çok basit: requiredKinds subset mi?
        List<ChemicalKind> present = new List<ChemicalKind>();
        foreach (var c in contents) present.Add(c.kind);

        foreach (var need in r.requiredKinds)
        {
            if (!present.Contains(need)) return false;
        }

        // opsiyonel: miktar kontrolü
        if (r.requiredTotalAmount > 0f)
        {
            float total = 0f;
            foreach (var c in contents) total += c.amount;
            if (total < r.requiredTotalAmount) return false;
        }

        return true;
    }

    IEnumerator RunReaction(ReactionRecipe r)
    {
        isReacting = true;
        Debug.Log("Reaction started: " + r.recipeName);
        if (animator) animator.SetBool("IsReacting", true);

        // bekle reactionDuration
        if (r.reactionDuration > 0f)
            yield return new WaitForSeconds(r.reactionDuration);

        // spawn effect
        Vector3 spawnPos = effectSpawnPoint ? effectSpawnPoint.position : transform.position;
        if (r.effectPrefab) Instantiate(r.effectPrefab, spawnPos, Quaternion.identity);

        // ses çal
        if (r.sound) AudioSource.PlayClipAtPoint(r.sound, spawnPos);

        // color change -> kap içindeki sývýyý temsil eden objenin renderer'ý varsa renk deðiþtir
        if (r.resultColor != Color.clear)
        {
            foreach (var c in contents)
            {
                // örnek: suyu renklendir
                if (c.itemRenderer != null)
                    c.itemRenderer.material.color = r.resultColor;
            }
        }

        // fiziksel patlama
        if (r.explosionForce > 0f)
        {
            Collider[] cols = Physics.OverlapSphere(spawnPos, r.explosionRadius);
            foreach (var col in cols)
            {
                Rigidbody rb = col.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    rb.AddExplosionForce(r.explosionForce, spawnPos, r.explosionRadius, 0.5f, ForceMode.Impulse);
                }
            }
        }

        // spawn gas particle and optionally make it float up
        if (r.spawnGas && r.gasPrefab)
        {
            Instantiate(r.gasPrefab, spawnPos + Vector3.up * 0.1f, Quaternion.identity);
        }

        // consume inputs
        if (r.consumesInputs)
        {
            // basit: sil
            foreach (var c in contents)
            {
                Destroy(c.gameObject);
            }
            contents.Clear();
        }

        if (animator) animator.SetTrigger("Explode");
        isReacting = false;
        if (animator) animator.SetBool("IsReacting", false);
    }
}
