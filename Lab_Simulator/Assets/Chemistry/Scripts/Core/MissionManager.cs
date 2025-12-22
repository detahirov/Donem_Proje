// Assets/Chemistry/Scripts/Core/MissionManager.cs
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("Görev Listesi (Sýrayla)")]
    public List<MissionSO> missions = new List<MissionSO>();

    [Header("Aktif Görev")]
    public int currentMissionIndex = 0;
    public MissionSO CurrentMission =>
        (currentMissionIndex >= 0 && currentMissionIndex < missions.Count) ? missions[currentMissionIndex] : null;

    public delegate void MissionChanged(MissionSO newMission);
    public event MissionChanged OnMissionChanged;

    public delegate void MissionCompleted(MissionSO completed);
    public event MissionCompleted OnMissionCompleted;

    const string SAVE_KEY = "ChemLab_CurrentMissionIndex";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProgress();
    }

    void Start()
    {
        // Oyuna girince ilk görevi UI'ye bildirelim
        
        NotifyMissionChanged();
    }

    #region Save / Load
    void LoadProgress()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            currentMissionIndex = PlayerPrefs.GetInt(SAVE_KEY, 0);
        }
        else
        {
            currentMissionIndex = 0;
        }
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt(SAVE_KEY, currentMissionIndex);
        PlayerPrefs.Save();
    }
    #endregion

    void NotifyMissionChanged()
    {
        OnMissionChanged?.Invoke(CurrentMission);
        Debug.Log("Aktif Görev: " + (CurrentMission ? CurrentMission.title : "YOK"));
    }

    public void CompleteCurrentMission()
    {
        var m = CurrentMission;
        if (m == null) return;

        Debug.Log("Görev tamamlandý: " + m.title);
        OnMissionCompleted?.Invoke(m);

        // Ödülleri aç
        UnlockRewards(m);

        // Son görev deðilse sýradaki
        currentMissionIndex++;
        SaveProgress();
        NotifyMissionChanged();
    }

    void UnlockRewards(MissionSO m)
    {
        // Kimyasallarý aç
        foreach (var s in m.unlockSubstances)
        {
            if (s == null) continue;
            UnlockSystem.UnlockSubstance(s);
        }

        // Araç gereçleri aç
        foreach (var go in m.unlockTools)
        {
            if (go == null) continue;
            UnlockSystem.UnlockTool(go);
        }
    }

    // --- Aþaðýdakiler dýþarýdan çaðrýlacak ---

    // 1) Reaksiyon gerçekleþtiðinde ReactionManager burayý çaðýracak
    public void NotifyReactionPerformed(ReactionSO reaction)
    {
        var m = CurrentMission;
        if (m == null) return;
        if (m.conditionType != MissionConditionType.PerformReaction) return;

        if (m.targetReaction == reaction)
        {
            CompleteCurrentMission();
        }
    }

    // 2) Kap içine belli madde eklendiðinde Container burayý çaðýrabilir (istersen)
    public void NotifySubstanceAdded(SubstanceSO substance)
    {
        var m = CurrentMission;
        if (m == null) return;
        if (m.conditionType != MissionConditionType.AddSubstanceToContainer) return;

        if (m.targetSubstance == substance)
        {
            CompleteCurrentMission();
        }
    }

    // 3) Bir aracý eline aldýðýnda HandController ya da Tool script'i çaðýrabilir
    public void NotifyToolPickedUp(string toolName)
    {
        var m = CurrentMission;
        if (m == null) return;
        if (m.conditionType != MissionConditionType.PickUpTool) return;

        if (!string.IsNullOrEmpty(m.toolName) && m.toolName == toolName)
        {
            CompleteCurrentMission();
        }
    }
    public MissionSO GetCurrentMission()
    {
        if (currentMissionIndex < missions.Count)
            return missions[currentMissionIndex];
        return null;
    }
}
