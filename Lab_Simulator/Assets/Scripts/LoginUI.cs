using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;   // TextMeshPro kullanýyorsan

public class LoginUI : MonoBehaviour
{
    [Header("Inputlar")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    [Header("Butonlar")]
    public Button loginButton;
    public Button registerButton;

    [Header("Durum Yazýsý")]
    public TextMeshProUGUI statusText;

    void Start()
    {
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnLoggedIn += HandleLoggedIn;
        }
        // Butonlara týklama event'leri baðla
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);

        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);

        SetStatus("Lütfen email ve þifre girin.");
    }

    void OnDestroy()
    {
        if (loginButton != null)
            loginButton.onClick.RemoveListener(OnLoginClicked);

        if (registerButton != null)
            registerButton.onClick.RemoveListener(OnRegisterClicked);
    }

    void SetStatus(string msg)
    {
        if (statusText != null)
            statusText.text = msg;
        Debug.Log("[LoginUI] " + msg);
    }

    void OnLoginClicked()
    {
        string email = emailInput.text.Trim();
        string pass = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
        {
            SetStatus("Email ve þifre boþ olamaz.");
            return;
        }

        if (AuthManager.Instance == null)
        {
            SetStatus("AuthManager bulunamadý!");
            return;
        }

        SetStatus("Giriþ yapýlýyor...");

        // AuthManager içinde Login çaðýrýyoruz
        AuthManager.Instance.Login(email, pass);
    }
    void OnDisable()
    {
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnLoggedIn -= HandleLoggedIn;
        }
    }
    void OnRegisterClicked()
    {
        string email = emailInput.text.Trim();
        string pass = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
        {
            SetStatus("Kayýt için email ve þifre girin.");
            return;
        }

        if (AuthManager.Instance == null)
        {
            SetStatus("AuthManager bulunamadý!");
            return;
        }

        SetStatus("Kayýt olunuyor...");

        AuthManager.Instance.Register(email, pass);
    }
    void HandleLoggedIn(Firebase.Auth.FirebaseUser user)
    {
        SetStatus("Giriþ baþarýlý: " + user.Email);

        // Ýstersen login panelini gizle
        gameObject.SetActive(false);

        // Burada oyun sahnesine geçebilirsin:
        // using UnityEngine.SceneManagement;
        SceneManager.LoadScene("TepkimeDeneme");  // senin sahnenin adý
    }
}
