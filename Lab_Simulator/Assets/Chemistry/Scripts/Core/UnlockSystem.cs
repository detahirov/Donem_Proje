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
    public static void ResetAllUnlocks()
    {
        // 1. Hafýzadaki listeyi temizle
        unlockedSubstances.Clear();

        // 2. PlayerPrefs'teki tüm "ChemLab_Unlocked_" ile baþlayan kayýtlarý bul ve sil
        // Unity'de belirli keyleri topluca silme yoktur, bu yüzden SubstanceSO'larý tarayýp siliyoruz.
        var allSubstances = Resources.LoadAll<SubstanceSO>("");
        foreach (var so in allSubstances)
        {
            string key = SUBSTANCE_PREFIX + so.name;
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
        PlayerPrefs.Save();

        // 3. Sahnedeki nesneleri gizle (Varsayýlan açýk olanlar hariç)
        var allChemicals = Object.FindObjectsOfType<Chemical>(true);
        foreach (var chem in allChemicals)
        {
            if (chem.substance != null)
            {
                // Eðer varsayýlan olarak kilitli olmasý gerekiyorsa gizle
                if (!chem.substance.unlockedByDefault)
                {
                    chem.gameObject.SetActive(false);
                }
                else
                {
                    // Varsayýlan açýk olanlarý (Sodyum gibi) göster
                    chem.gameObject.SetActive(true);
                }
            }
        }
        Debug.Log("Tüm kilitler ve ilerleme sýfýrlandý.");
    }
    public static void UnlockTool(GameObject tool)
    {
        if (tool == null) return;
        tool.SetActive(true);
        Debug.Log("Araç açýldý: " + tool.name);
    }
}
