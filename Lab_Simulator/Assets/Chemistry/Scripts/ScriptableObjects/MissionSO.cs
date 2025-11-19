// Assets/Chemistry/Scripts/ScriptableObjects/MissionSO.cs
using UnityEngine;

public enum MissionConditionType
{
    None,
    PerformReaction,   // belli bir ReactionSO tetiklensin
    AddSubstanceToContainer, // belli maddeyi bir kaba ekle
    PickUpTool        // belli aracý eline al
}

[CreateAssetMenu(menuName = "Chemistry/Mission")]
public class MissionSO : ScriptableObject
{
    [Header("Görev Metni")]
    public string missionId = "mission_001";     // kaydetmek için benzersiz id
    public string title = "Na + Su Tepkimesi";
    [TextArea] public string description = "Sodyumu suya atarak tepkimeyi gözlemle.";

    [Header("Görev Koþulu")]
    public MissionConditionType conditionType = MissionConditionType.PerformReaction;

    // PerformReaction için:
    public ReactionSO targetReaction;           // örn: Na+Water reaction SO

    // AddSubstanceToContainer için:
    public SubstanceSO targetSubstance;

    // PickUpTool için:
    public string toolName;                     // GameObject adý / tag

    [Header("Ödüller (Açýlacak içerik)")]
    public SubstanceSO[] unlockSubstances;      // açýlacak kimyasallar
    public GameObject[] unlockTools;            // açýlacak araç gereçler (prefab ya da sahnedeki objeler)

    [Header("Diðer")]
    public bool completesGame = false;          // son görev mi?
}
