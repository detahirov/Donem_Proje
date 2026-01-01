using UnityEngine;

public class PickupableSafetyEquipment : MonoBehaviour
{
    public SafetyEquipmentType equipmentType;

    public void PickUp()
    {
        PlayerSafetyEquipment.Instance.Hold(this);
        gameObject.SetActive(false);
    }
}
