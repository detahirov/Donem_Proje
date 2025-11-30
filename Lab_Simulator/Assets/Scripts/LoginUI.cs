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

    bool userPressedLoginOrRegister = false;
    bool autoLoggedInWithoutClick = false;

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

        // Eðer Firebase zaten login ise ve bu login otomatik geldiyse
        // ve kullanýcý email'i deðiþtirmediyse bunu "Devam Et" say.
        if (AuthManager.Instance != null &&
            AuthManager.Instance.CurrentUser != null &&
            autoLoggedInWithoutClick &&
            AuthManager.Instance.CurrentUser.Email == email)
        {
            // Þifre sormadan devam et
            SetStatus("Kaydedilmiþ hesapla devam ediliyor: " + email);
            GoToGameScene();
            return;
        }

        // Buraya düþtüysek  normal login denemesi
        if (string.IsNullOrEmpty(pass))
        {
            SetStatus("Bu hesapla ilk kez giriþ yapacaksan þifre de girmelisin.");
            return;
        }

        if (AuthManager.Instance == null)
        {
            SetStatus("AuthManager bulunamadý!");
            return;
        }

        userPressedLoginOrRegister = true;
        SetStatus("Giriþ yapýlýyor...");

        AuthManager.Instance.Login(email, pass);
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

        // Eðer bu login, bizim Login/Register butonuna basmamýz SONRASI geldiyse
        if (userPressedLoginOrRegister)
        {
            SetStatus("Giriþ baþarýlý: " + user.Email);
            GoToGameScene();   // birazdan yazacaðýz
        }
        else
        {
            // Bu, oyun açýlýrken otomatik oturum restore edildiði durum
            autoLoggedInWithoutClick = true;

            // Email kutusunu doldur
            if (emailInput != null)
                emailInput.text = user.Email;

            if (passwordInput != null)
                passwordInput.text = ""; // þifreyi bilemeyiz

            SetStatus("Otomatik giriþ algýlandý: " + user.Email +
                      "\nBu hesapla devam etmek için Login'e basabilir veya email/þifreyi deðiþtirip baþka hesapla giriþ yapabilirsin.");
            // DÝKKAT: Burada SAHNE deðiþtirmiyoruz, sadece UI güncellendi.
        }
    }
    void GoToGameScene()
    {
        // Login panelini gizlemek istersen:
        // gameObject.SetActive(false);

        // Buraya laboratuvar sahnenin ismini yaz
        SceneManager.LoadScene("Original Lab");  // kendi sahne adýný yaz
    }
}
