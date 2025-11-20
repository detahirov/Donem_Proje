using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class CloudSaveManager : MonoBehaviour
{
    public static CloudSaveManager Instance { get; private set; }

    FirebaseFirestore db;

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
            Debug.LogWarning("Firebase hazýr deðil, Firestore biraz bekleyecek.");
            return;
        }
        db = FirebaseFirestore.DefaultInstance;

        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnLoggedIn += OnLoggedIn;
        }
    }

    void OnDestroy()
    {
        if (AuthManager.Instance != null)
            AuthManager.Instance.OnLoggedIn -= OnLoggedIn;
    }

    void OnLoggedIn(FirebaseUser user)
    {
        // Kullanýcý giriþ yaptýðýnda buluttan ilerlemeyi çek
        LoadFromCloud(user.UserId);
    }

    // ---- PUBLIC API ----

    public void PushProgressToCloud()
    {
        if (AuthManager.Instance == null || AuthManager.Instance.CurrentUser == null)
        {
            Debug.LogWarning("CloudSave: Kullanýcý yok, cloud save yapýlmadý.");
            return;
        }

        string uid = AuthManager.Instance.CurrentUser.UserId;
        var progress = BuildProgressFromPlayerPrefs();

        SaveProgressDocument(uid, progress);
    }

    public void LoadFromCloud(string uid)
    {
        GetProgressDocument(uid).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogWarning("CloudSave: progress okunamadý, yerel PlayerPrefs kullanýlacak. " + task.Exception);
                return;
            }

            DocumentSnapshot snap = task.Result;
            if (!snap.Exists)
            {
                Debug.Log("CloudSave: bu kullanýcý için progress yok, sýfýrdan baþlýyor.");
                return;
            }

            var data = snap.ToDictionary();

            int currentMissionIndex = data.ContainsKey("currentMissionIndex") ?
                System.Convert.ToInt32(data["currentMissionIndex"]) : 0;

            string[] unlockedSubs = data.ContainsKey("unlockedSubstances") ?
                ((List<object>)data["unlockedSubstances"]).Select(o => o.ToString()).ToArray() :
                new string[0];

            string[] completedMissions = data.ContainsKey("completedMissions") ?
                ((List<object>)data["completedMissions"]).Select(o => o.ToString()).ToArray() :
                new string[0];

            // PlayerPrefs’e uygula
            ApplyProgressToPlayerPrefs(currentMissionIndex, unlockedSubs, completedMissions);
        });
    }

    // ---- Firestore yardýmcýlarý ----

    Task<DocumentSnapshot> GetProgressDocument(string uid)
    {
        return db.Collection("users").Document(uid).Collection("progress").Document("main").GetSnapshotAsync();
    }

    void SaveProgressDocument(string uid, UserProgress progress)
    {
        var docRef = db.Collection("users").Document(uid).Collection("progress").Document("main");

        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "currentMissionIndex", progress.currentMissionIndex },
            { "unlockedSubstances", progress.unlockedSubstances },
            { "completedMissions", progress.completedMissions }
        };

        docRef.SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("CloudSave: progress yazýlamadý: " + task.Exception);
            }
            else
            {
                Debug.Log("CloudSave: progress buluta kaydedildi.");
            }
        });
    }

    // ---- PlayerPrefs <-> Progress dönüþüm ----

    UserProgress BuildProgressFromPlayerPrefs()
    {
        UserProgress p = new UserProgress();

        // MissionManager’ýn kaydettiði index
        p.currentMissionIndex = PlayerPrefs.GetInt("ChemLab_CurrentMissionIndex", 0);

        // Açýlmýþ maddeler (UnlockSystem’ýn prefix’ini kullanýyoruz)
        List<string> unlocked = new List<string>();
        foreach (var so in Resources.LoadAll<SubstanceSO>("")) // tüm SubstanceSO’larý bul
        {
            string key = "ChemLab_Unlocked_" + so.name;
            if (so.unlockedByDefault || PlayerPrefs.GetInt(key, 0) == 1)
                unlocked.Add(so.name);
        }
        p.unlockedSubstances = unlocked.ToArray();

        // Tamamlanan görevler için istersen MissionManager’dan data çekebilirsin
        // Þimdilik boþ kalsýn veya PlayerPrefs’te kendin tuttuðun key’lere göre doldur.
        p.completedMissions = new string[0];

        return p;
    }

    void ApplyProgressToPlayerPrefs(int missionIndex, string[] unlockedSubs, string[] completedMissions)
    {
        // Görev index
        PlayerPrefs.SetInt("ChemLab_CurrentMissionIndex", missionIndex);

        // Tüm substance’larý resetle
        foreach (var so in Resources.LoadAll<SubstanceSO>(""))
        {
            string key = "ChemLab_Unlocked_" + so.name;
            PlayerPrefs.SetInt(key, so.unlockedByDefault ? 1 : 0);
        }

        // Cloud’dan gelen açýlmýþ maddeleri iþaretle
        foreach (var name in unlockedSubs)
        {
            string key = "ChemLab_Unlocked_" + name;
            PlayerPrefs.SetInt(key, 1);
        }

        PlayerPrefs.Save();

        Debug.Log("CloudSave: Progress PlayerPrefs’e uygulandý.");
    }
}
