// Assets/Chemistry/Scripts/ScriptableObjects/ReactionSO.cs
using UnityEngine;

public enum ReactionEffectType { ColorChange, GasRelease, HeatChange, Foam, Explosion, Precipitate, Decolorize }

[CreateAssetMenu(menuName = "Chemistry/Reaction")]
public class ReactionSO : ScriptableObject
{
    [Header("Reactants (order-independent)")]
    public SubstanceSO[] reactants; // örn: [HCl, NaOH], [NaHCO3, Vinegar], [Na, Water]

    [Header("Conditions")]
    public bool requiresLiquidContainer = true;
    public float minTemp = 0f, maxTemp = 999f;
    public bool onlyIfIndicatorPresent = false; // renk deðiþimi için

    [Header("Resulting Effects")]
    public ReactionEffect[] effects;

    [Header("Inventory Changes (oyunsal)")]
    public bool consumeAllReactants = true;
    public SubstanceSO[] products; // istersen yeni çözelti/ürün ekle (oyunsal)

    [TextArea] public string notes;
}

[System.Serializable]
public class ReactionEffect
{
    public ReactionEffectType type;

    // ColorChange / Decolorize
    public Color targetColor = Color.white;
    public float colorLerpTime = 1.2f;

    // GasRelease
    public GameObject gasPrefab;
    public float gasRate = 6f;         // spawn/saniye
    public float gasDuration = 3f;

    // HeatChange
    public float heatDelta = 10f;      // + ekzotermik, - endotermik

    // Foam
    public GameObject foamPrefab;
    public float foamDuration = 4f;

    // Explosion
    public GameObject explosionPrefab;
    public float explosionForce = 350f;
    public float explosionRadius = 3f;
    public AudioClip explosionSfx;
}
