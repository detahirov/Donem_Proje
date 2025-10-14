using UnityEngine;
using UnityEngine.UI;

public class UI_Crosshair : MonoBehaviour
{
    public Image crosshairImage;
    public Sprite normal;
    public Sprite hover;

    public void SetHover(bool hovering)
    {
        if (crosshairImage == null) return;
        crosshairImage.sprite = hovering ? hover : normal;
    }
}
