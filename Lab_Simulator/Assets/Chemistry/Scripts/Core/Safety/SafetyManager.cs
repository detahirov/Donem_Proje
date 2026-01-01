using System.Collections.Generic;
using UnityEngine;

public class SafetyManager : MonoBehaviour
{
    public static SafetyManager Instance;

    HashSet<SafetyEquipmentType> equipped = new HashSet<SafetyEquipmentType>();

    void Awake()
    {
        Instance = this;
    }

    public void Equip(SafetyEquipmentType type)
    {
        equipped.Add(type);
        Debug.Log(type + " takýldý");
    }

    public void Unequip(SafetyEquipmentType type)
    {
        equipped.Remove(type);
        Debug.Log(type + " çýkarýldý");
    }

    public bool IsEquipped(SafetyEquipmentType type)
    {
        return equipped.Contains(type);
    }

    public bool HasAll(List<SafetyEquipmentType> required)
    {
        foreach (var r in required)
            if (!equipped.Contains(r))
                return false;
        return true;
    }
}
