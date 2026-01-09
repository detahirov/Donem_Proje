using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Sahnedeki ana kamerayý bul
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Billboard: Sahnede 'MainCamera' etiketli bir kamera bulunamadý!");
        }
    }

    // LateUpdate, kamera hareket ettikten hemen sonra çalýþýr,
    // bu da titremeyi önler.
    void LateUpdate()
    {
        if (mainCamera == null) return;

        // Objenin yönünü, kendi pozisyonundan kameranýn pozisyonuna doðru olan vektöre çevir.
        // Bu iþlem aslýnda objenin "arkasýný" kameraya döndürür, UI elementleri
        // genelde tersten render alýndýðý için doðru okunuþ bu þekildedir.
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }
}