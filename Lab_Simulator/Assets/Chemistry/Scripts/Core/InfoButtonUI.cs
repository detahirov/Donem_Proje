using UnityEngine;

public class InfoButtonUI : MonoBehaviour
{
    public void OnInfoButtonPressed()
    {
        InfoRaycaster raycaster = Camera.main.GetComponent<InfoRaycaster>();

        if (raycaster != null)
        {
            raycaster.TryShowInfo();
        }
    }
}
