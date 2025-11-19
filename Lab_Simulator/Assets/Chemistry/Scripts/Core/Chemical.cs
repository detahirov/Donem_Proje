// Assets/Chemistry/Scripts/Core/Chemical.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Chemical : MonoBehaviour
{
    public SubstanceSO substance;   // hangi madde?
    public float amount = 0.05f;    // kg/oyunsal birim
    public bool consumedOnAdd = true;
    void Start()
    {
        // Eðer bu madde kilitliyse kendini görünmez/yok yap
        if (!UnlockSystem.IsSubstanceUnlocked(substance))
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var cont = other.GetComponentInParent<Container>();
        if (cont != null)
        {
            cont.AddSubstance(substance, amount);
            if (consumedOnAdd) Destroy(gameObject);
        }
    }
}
