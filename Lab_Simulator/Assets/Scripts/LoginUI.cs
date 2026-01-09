using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    bool userPressedLoginOrRegister = false;
    bool autoLoggedInWithoutClick = false;

    void Start()
    {
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnLoggedIn += HandleLoggedIn;
        }

        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);

        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);

        SetStatus("Lütfen email ve þifre girin.");
    }

    void OnEnable()
    {
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnLoggedIn += HandleLoggedIn;
        }
    }

    void OnDisable()
    {
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnLoggedIn -= HandleLoggedIn;
        }
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

        if (string.IsNullOrEmpty(email))
        {
            SetStatus("Email boþ olamaz.");
            return;
        }

        // Otomatik giriþ yapýlmýþ bir kullanýcý, emailini deðiþtirmeden login'e bastýysa devam et
        if (AuthManager.Instance != null &&
            AuthManager.Instance.CurrentUser != null &&
            autoLoggedInWithoutClick &&
            AuthManager.Instance.CurrentUser.Email == email)
        {
            SetStatus("Kaydedilmiþ hesapla devam ediliyor: " + email);
            GoToGameScene();
            return;
        }

        if (string.IsNullOrEmpty(pass))
        {
            SetStatus("Giriþ yapmak için þifre girmelisiniz.");
            return;
        }

        if (AuthManager.Instance == null)
        {
            SetStatus("AuthManager bulunamadý!");
            return;
        }

        userPressedLoginOrRegister = true;
        SetStatus("Giriþ yapýlýyor...");

        // GÜNCELLENMÝÞ LOGIN ÇAÐRISI (CALLBACK ÝLE)
        AuthManager.Instance.Login(email, pass,
            (successMsg) => {
                // Baþarýlý olduðunda
                SetStatus(successMsg);
                // Not: Sahne geçiþini HandleLoggedIn zaten yapacak,
                // ama burasý ekstra bir onay mesajý için iyi.
            },
            (failMsg) => {
                // Hata olduðunda (Yanlýþ þifre vs.)
                SetStatus(failMsg);
                // UI'da kullanýcýnýn tekrar denemesine izin ver
                userPressedLoginOrRegister = false;
            }
        );
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

        userPressedLoginOrRegister = true;
        SetStatus("Kayýt olunuyor...");

        AuthManager.Instance.Register(email, pass);
    }

    void HandleLoggedIn(Firebase.Auth.FirebaseUser user)
    {
        if (user == null) return;

        // Login veya Register butonuna basýldýktan sonra gelen giriþ
        if (userPressedLoginOrRegister)
        {
            SetStatus("Giriþ baþarýlý: " + user.Email);
            GoToGameScene();
        }
        else
        {
            // Oyun açýlýr açýlmaz gelen otomatik giriþ (cache)
            autoLoggedInWithoutClick = true;

            if (emailInput != null)
                emailInput.text = user.Email;

            if (passwordInput != null)
                passwordInput.text = "";

            SetStatus("Otomatik giriþ algýlandý: " + user.Email +
                      "\nDevam etmek için Login'e bas, ya da bilgileri deðiþtir.");
        }
    }

    void GoToGameScene()
    {
        SceneManager.LoadScene("Original Lab");
    }
}