using UnityEngine;

public class CameraOverlayUI : MonoBehaviour
{
    public GameObject goggles;
    public GameObject mask;

    void Update()
    {
        var sm = SafetyManager.Instance;
        if (!sm) return;

        goggles.SetActive(sm.IsEquipped(SafetyEquipmentType.Goggles));
        mask.SetActive(sm.IsEquipped(SafetyEquipmentType.Mask));
    }
}
