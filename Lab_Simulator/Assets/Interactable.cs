using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // Raycast tarafýndan çaðrýlacak
    public float interactionDistance = 3f;

    // UI için kýsa isim
    public string interactionName = "Interact";

    // etkileþim UI göstermek istersen override et
    public virtual void OnHover() { }

    // e tuþuna basýldýðýnda çaðrýlýr
    public abstract void OnInteract(HandController hand);
}
