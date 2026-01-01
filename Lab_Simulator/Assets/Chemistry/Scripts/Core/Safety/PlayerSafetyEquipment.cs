using UnityEngine;

public class PlayerSafetyEquipment : MonoBehaviour
{
    public static PlayerSafetyEquipment Instance;

    PickupableSafetyEquipment held;

    public Animator equipmentAnimator;

    void Awake()
    {
        Instance = this;
    }

    public void Hold(PickupableSafetyEquipment eq)
    {
        held = eq;
    }

    void Update()
    {
        if (held == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            EquipHeld();
        }
    }

    void EquipHeld()
    {
        var type = held.equipmentType;

        if (SafetyManager.Instance.IsEquipped(type))
            equipmentAnimator.SetTrigger("Remove" + type);
        else
            equipmentAnimator.SetTrigger("Equip" + type);

        held = null;
    }
}
