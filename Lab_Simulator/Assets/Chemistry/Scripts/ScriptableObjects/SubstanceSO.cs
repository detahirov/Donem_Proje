// Assets/Chemistry/Scripts/ScriptableObjects/SubstanceSO.cs
using UnityEngine;

public enum SubstanceState { Solid, Liquid, Gas, Solution }

[CreateAssetMenu(menuName = "Chemistry/Substance")]
public class SubstanceSO : ScriptableObject
{
    public string displayName;
    public SubstanceState defaultState = SubstanceState.Solid;

    [Header("Game Properties")]
    [Range(0, 14)] public float pH = 7f;          // çözeltideki tipik pH (oyunsal)
    public bool solubleInWater = true;
    public Color baseColor = Color.white;        // sývý/çözelti rengi
    public Sprite icon;

    [Header("Tags (eþleþtirme için)")]
    public bool isAcid;      // HCl, H2SO4...
    public bool isBase;      // NaOH, KOH...
    public bool isIndicator; // fenolftalein, metil oranj...
    public bool isOxidizer;  // KMnO4 gibi
    public bool isMetal;     // Na, K, Zn, Mg...
    public bool reactsViolentlyWithWater; // Na, K gibi
    [Header("Unlock")]
    public bool unlockedByDefault = false;
    [TextArea] public string notes;
}
