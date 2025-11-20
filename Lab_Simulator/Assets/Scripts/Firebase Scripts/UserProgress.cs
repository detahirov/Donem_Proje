[System.Serializable]
public class UserProgress
{
    public int currentMissionIndex;
    public string[] unlockedSubstances;   // SubstanceSO.name
    public string[] completedMissions;    // MissionSO.missionId
}
