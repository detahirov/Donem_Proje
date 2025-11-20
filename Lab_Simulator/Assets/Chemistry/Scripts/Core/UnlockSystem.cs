// Assets/Chemistry/Scripts/Core/UnlockSystem.cs
using System.Collections.Generic;
using UnityEngine;


public static class UnlockSystem
{
    static HashSet<SubstanceSO> unlockedSubstances = new HashSet<SubstanceSO>();

    const string SUBSTANCE_PREFIX = "ChemLab_Unlocked_";

    public static bool IsSubstanceUnlocked(SubstanceSO so)
    {
        if (so == null) return false;
        // Varsayýlan: su gibi bazý þeyler hep açýk olabilir, istersen buraya istisna koy
        if (so.unlockedByDefault)
            return true;
        return unlockedSubstances.Contains(so) || PlayerPrefs.GetInt(SUBSTANCE_PREFIX + so.name, 0) == 1;
    }

    public static void UnlockSubstance(SubstanceSO so)
    {
        if (so == null) return;

        unlockedSubstances.Add(so);
        PlayerPrefs.SetInt(SUBSTANCE_PREFIX + so.name, 1);
        PlayerPrefs.Save();
        Debug.Log("Kimyasal açýldý: " + so.displayName);

        
        
        var allChemicals = UnityEngine.Object.FindObjectsOfType<Chemical>(true);
        foreach (var chem in allChemicals)
        {
            if (chem.substance == so)
            {
                chem.gameObject.SetActive(true);
            }
        }
    }

    public static void UnlockTool(GameObject tool)
    {
        if (tool == null) return;
        tool.SetActive(true);
        Debug.Log("Araç açýldý: " + tool.name);
    }
}
