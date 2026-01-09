using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }
    public FirebaseAuth Auth { get; private set; }
    public FirebaseUser CurrentUser { get; private set; }

    public event Action<FirebaseUser> OnLoggedIn;
    public event Action OnLoggedOut;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (!FirebaseInit.IsReady)
        {
            Debug.LogWarning("Firebase henüz hazýr deðil, birazdan Auth kurulacak.");
            return;
        }
        InitAuth();
    }

    public void TryInitAuth()
    {
        if (FirebaseInit.IsReady && Auth == null)
        {
            InitAuth();
        }
    }

    void InitAuth()
    {
        Auth = FirebaseAuth.DefaultInstance;
        Auth.StateChanged += OnAuthStateChanged;
        OnAuthStateChanged(this, null);
    }

    void OnDestroy()
    {
        if (Auth != null)
            Auth.StateChanged -= OnAuthStateChanged;
    }

    void OnAuthStateChanged(object sender, EventArgs e)
    {
        if (Auth.CurrentUser != CurrentUser)
        {
            bool signedIn = Auth.CurrentUser != null;
            if (!signedIn)
            {
                Debug.Log("Firebase user signed out.");
                CurrentUser = null;
                OnLoggedOut?.Invoke();
            }
            else
            {
                CurrentUser = Auth.CurrentUser;
                Debug.Log("Firebase user signed in: " + CurrentUser.UserId);
                OnLoggedIn?.Invoke(CurrentUser);
            }
        }
    }

    // --- GÜNCELLENEN LOGIN FONKSÝYONU ---
    public void Login(string email, string password, Action<string> onSuccess, Action<string> onFail)
    {
        // 1. ÇATIÞMA ÇÖZÜMÜ: Eðer zaten bir oturum açýk görünüyorsa önce kapat.
        if (Auth.CurrentUser != null)
        {
            Auth.SignOut();
        }

        Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                onFail?.Invoke("Giriþ iptal edildi.");
                return;
            }
            if (task.IsFaulted)
            {
                // 2. HATA AYIKLAMA: Hatayý detaylandýrýp Türkçe mesaj üret
                Firebase.FirebaseException firebaseEx = task.Exception.GetBaseException() as Firebase.FirebaseException;
                string errorMessage = "Giriþ Hatasý: Bilinmeyen hata.";

                if (firebaseEx != null)
                {
                    var errorCode = (Firebase.Auth.AuthError)firebaseEx.ErrorCode;
                    switch (errorCode)
                    {
                        case Firebase.Auth.AuthError.WrongPassword:
                            errorMessage = "Hatalý þifre girdiniz.";
                            break;
                        case Firebase.Auth.AuthError.UserNotFound:
                            errorMessage = "Bu email ile kayýtlý kullanýcý bulunamadý.";
                            break;
                        case Firebase.Auth.AuthError.InvalidEmail:
                            errorMessage = "Geçersiz email formatý.";
                            break;
                        case Firebase.Auth.AuthError.UserDisabled:
                            errorMessage = "Kullanýcý hesabý devre dýþý.";
                            break;
                        case Firebase.Auth.AuthError.TooManyRequests:
                            errorMessage = "Çok fazla deneme yaptýnýz. Lütfen bekleyin.";
                            break;
                        default:
                            errorMessage = "Giriþ Baþarýsýz: " + firebaseEx.Message;
                            break;
                    }
                }

                onFail?.Invoke(errorMessage);
                Debug.LogError("Login Error Code: " + (firebaseEx != null ? firebaseEx.ErrorCode.ToString() : "Null"));
                return;
            }

            // Baþarýlý (task.Result yerine Auth.CurrentUser kullanýyoruz, daha güvenli)
            if (Auth.CurrentUser != null)
            {
                onSuccess?.Invoke("Giriþ baþarýlý! Hoþgeldin: " + Auth.CurrentUser.Email);
            }
            else
            {
                // Nadir durum
                onSuccess?.Invoke("Giriþ baþarýlý.");
            }
        });
    }

    public void Register(string email, string password)
    {
        Auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("Kayýt hatasý: " + task.Exception);
                    return;
                }
                Debug.Log("Kayýt baþarýlý, kullanýcý oluþturuldu.");
            });
    }

    public void Logout()
    {
        Auth.SignOut();
    }
}