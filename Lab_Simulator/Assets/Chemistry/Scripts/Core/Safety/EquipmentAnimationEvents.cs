using UnityEngine;

public class EquipmentAnimationEvents : MonoBehaviour
{
    public void OnEquipGloves()
    {
        SafetyManager.Instance.Equip(SafetyEquipmentType.Gloves);
    }

    public void OnRemoveGloves()
    {
        SafetyManager.Instance.Unequip(SafetyEquipmentType.Gloves);
    }

    public void OnEquipGoggles()
    {
        SafetyManager.Instance.Equip(SafetyEquipmentType.Goggles);
    }

    public void OnRemoveGoggles()
    {
        SafetyManager.Instance.Unequip(SafetyEquipmentType.Goggles);
    }

    public void OnEquipMask()
    {
        SafetyManager.Instance.Equip(SafetyEquipmentType.Mask);
    }

    public void OnRemoveMask()
    {
        SafetyManager.Instance.Unequip(SafetyEquipmentType.Mask);
    }

    public void OnEquipLabCoat()
    {
        SafetyManager.Instance.Equip(SafetyEquipmentType.LabCoat);
    }

    public void OnRemoveLabCoat()
    {
        SafetyManager.Instance.Unequip(SafetyEquipmentType.LabCoat);
    }
}
