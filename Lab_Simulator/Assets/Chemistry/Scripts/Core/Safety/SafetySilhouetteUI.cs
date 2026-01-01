using UnityEngine;
using UnityEngine.UI;

public class SafetySilhouetteUI : MonoBehaviour
{
    public RawImage gloves;
    public RawImage labCoat;
    public RawImage mask;
    public RawImage goggles;

    void Update()
    {
        var sm = SafetyManager.Instance;
        if (!sm) return;

        gloves.enabled = sm.IsEquipped(SafetyEquipmentType.Gloves);
        labCoat.enabled = sm.IsEquipped(SafetyEquipmentType.LabCoat);
        mask.enabled = sm.IsEquipped(SafetyEquipmentType.Mask);
        goggles.enabled = sm.IsEquipped(SafetyEquipmentType.Goggles);
    }
}
