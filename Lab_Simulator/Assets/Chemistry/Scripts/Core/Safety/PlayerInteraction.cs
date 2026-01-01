using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public Camera cam;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            var pickup = hit.collider.GetComponent<PickupableSafetyEquipment>();
            if (pickup != null)
            {
                pickup.PickUp();
            }
        }
    }
}
