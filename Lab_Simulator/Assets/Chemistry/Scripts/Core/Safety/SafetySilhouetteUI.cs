using UnityEngine;
using UnityEngine.UI;

public class SafetySilhouetteUI : MonoBehaviour
{
    [Header("Normal Equipment (Parent Alpha)")]
    public RawImage labCoat;
    public RawImage mask;
    public RawImage goggles;

    [Header("Gloves (Children Alpha)")]
    public RawImage glovesParent;              // sadece container
    public RawImage leftGlove;
    public RawImage rightGlove;

    void Update()
    {
        var sm = SafetyManager.Instance;
        if (!sm) return;

        // Normal ekipmanlar
        SetAlpha(labCoat, sm.IsEquipped(SafetyEquipmentType.LabCoat));
        SetAlpha(mask, sm.IsEquipped(SafetyEquipmentType.Mask));
        SetAlpha(goggles, sm.IsEquipped(SafetyEquipmentType.Goggles));

        // Gloves özel durum
        bool glovesOn = sm.IsEquipped(SafetyEquipmentType.Gloves);
        SetAlpha(leftGlove, glovesOn);
        SetAlpha(rightGlove, glovesOn);
    }

    void SetAlpha(RawImage img, bool visible)
    {
        if (img == null) return;

        var c = img.color;
        c.a = visible ? 1f : 0f;
        img.color = c;
    }
}
