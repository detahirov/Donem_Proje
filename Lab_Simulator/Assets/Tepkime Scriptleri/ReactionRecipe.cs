using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Lab/ReactionRecipe")]
public class ReactionRecipe : ScriptableObject
{
    public string recipeName;
    public List<ChemicalKind> requiredKinds = new List<ChemicalKind>(); // örn [Sodium, Water]
    public float requiredTotalAmount = 0f; // opsiyonel
    public float reactionDuration = 0.5f; // kaç saniye sürsün (0 için instant)
    public bool consumesInputs = true;

    // Efekt prefablarý
    public GameObject effectPrefab; // explosion, smoke, colorchange v.b.
    public AudioClip sound;
    public Color resultColor = Color.white; // kap içindeyse sývý rengini deðiþtir
    public float explosionForce = 0f; // 0 ise fiziksel patlama yok
    public float explosionRadius = 2f;
    public bool spawnGas = false;
    public GameObject gasPrefab;
}
