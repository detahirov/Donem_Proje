using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI Elemanlarý")]
    public TextMeshProUGUI emailText;
    public TextMeshProUGUI missionText;

    // Görev sistemin henüz tam entegre deðilse diye varsayýlan deðer
    private int currentMissionIndex = 1;

    void Start()
    {
        // 1. Email Bilgisini AuthManager'dan Çek
        if (AuthManager.Instance != null && AuthManager.Instance.CurrentUser != null)
        {
            emailText.text = "Oyuncu: " + AuthManager.Instance.CurrentUser.Email;
        }
        else
        {
            emailText.text = "Oyuncu: Misafir";
            Debug.LogWarning("AuthManager veya Kullanýcý bulunamadý, sahne direkt açýlmýþ olabilir.");
        }

        // 2. Baþlangýçta Görev Yazýsýný Güncelle
        // Eðer bir MissionManager'ýn varsa oradan veri çekeceðiz, þimdilik manuel baþlatýyoruz.
        UpdateMissionDisplay(currentMissionIndex, "Baþlangýç");
    }

    // Bu fonksiyonu Görev Sistemi scriptinden çaðýracaðýz
    public void UpdateMissionDisplay(int missionNo, string missionDescription = "")
    {
        if (missionText != null)
        {
            // Örnek çýktý: "GÖREV 1: Tüpleri Karýþtýr"
            missionText.text = $"GÖREV {missionNo}: {missionDescription}";
        }
    }
}