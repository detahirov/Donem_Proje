using UnityEngine;

public class PickupableSafetyEquipment : MonoBehaviour
{
    public SafetyEquipmentType equipmentType;

    Vector3 originalPosition;
    Quaternion originalRotation;

    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void PickUp()
    {
        PlayerSafetyEquipment.Instance.Hold(this);
        gameObject.SetActive(false);
    }

    public void DropBack()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        gameObject.SetActive(true);
    }
}
