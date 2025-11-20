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

    public void Login(string email, string password)
    {
        Auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("Giriþ hatasý: " + task.Exception);
                    return;
                }
                Debug.Log("Giriþ baþarýlý.");
            });
    }

    public void Logout()
    {
        Auth.SignOut();
    }
}
