using UnityEngine;

public class InfoRaycaster : MonoBehaviour
{
    public float distance = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            TryShowInfo();
        }
    }

    public void TryShowInfo()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            var holder = hit.collider.GetComponent<InfoHolder>();
            if (holder != null)
            {
                InfoPanelUI.Instance.Show(holder.infoData);
            }
        }
    }
}
