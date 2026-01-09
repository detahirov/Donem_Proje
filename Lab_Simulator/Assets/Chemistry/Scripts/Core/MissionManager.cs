using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    FirebaseFirestore db;

    [Header("Görev Listesi (Sýrayla)")]
    public List<MissionSO> missions = new List<MissionSO>();

    [Header("Aktif Görev")]
    public int currentMissionIndex = 0;

    public MissionSO CurrentMission =>
        (currentMissionIndex >= 0 && currentMissionIndex < missions.Count) ?
        missions[currentMissionIndex] : null;

    public delegate void MissionChanged(MissionSO newMission);
    public event MissionChanged OnMissionChanged;

    public delegate void MissionCompleted(MissionSO completed);
    public event MissionCompleted OnMissionCompleted;

    const string SAVE_KEY = "ChemLab_CurrentMissionIndex";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        db = FirebaseFirestore.DefaultInstance;
    }

    void Start()
    {
        
        // 1. Önce yerel veriyi yükle (Ýnternet yoksa bile çalýþsýn)
        LoadProgressLocal();

        // 2. Kullanýcý giriþ yapmýþ mý kontrol et
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnLoggedIn += OnUserLoggedIn;

            // Eðer AuthManager bizden önce hazýr olduysa:
            if (AuthManager.Instance.CurrentUser != null)
            {
                OnUserLoggedIn(AuthManager.Instance.CurrentUser);
            }
        }

        NotifyMissionChanged();
    }

    void OnDestroy()
    {
        if (AuthManager.Instance != null)
            AuthManager.Instance.OnLoggedIn -= OnUserLoggedIn;
    }

    void OnUserLoggedIn(FirebaseUser user)
    {
        Debug.Log($"MissionManager: Kullanýcý giriþi algýlandý ({user.Email}), Firestore verisi çekiliyor...");
        LoadMissionFromFirestore(user);
    }

    #region Save / Load Logic

    void LoadProgressLocal()
    {
        currentMissionIndex = PlayerPrefs.GetInt(SAVE_KEY, 0);
        // Güvenlik: Liste dýþýna taþmayý engelle
        if (currentMissionIndex > missions.Count) currentMissionIndex = missions.Count;
    }

    void SaveProgress()
    {
        // Yerel Kayýt
        PlayerPrefs.SetInt(SAVE_KEY, currentMissionIndex);
        PlayerPrefs.Save();

        // Cloud Kayýt
        SaveMissionToFirestore();
    }

    #endregion

    void NotifyMissionChanged()
    {
        OnMissionChanged?.Invoke(CurrentMission);

        string gorevAdi = (CurrentMission != null) ? CurrentMission.title : "TÜM GÖREVLER BÝTTÝ";
        Debug.Log("Aktif Görev: " + gorevAdi);

        // --- EKLEDÝÐÝMÝZ KISIM BAÞLANGIÇ ---
        // Sahnedeki PlayerHUD scriptini bulup güncelliyoruz
        PlayerHUD hud = FindObjectOfType<PlayerHUD>();
        if (hud != null)
        {
            // currentMissionIndex 0'dan baþlar, o yüzden oyuncuya gösterirken +1 ekliyoruz (Görev 1, Görev 2 gibi)
            // Ýkinci parametre olarak MissionSO içindeki baþlýðý (title) gönderiyoruz.
            hud.UpdateMissionDisplay(currentMissionIndex + 1, gorevAdi);
        }
        // --- EKLEDÝÐÝMÝZ KISIM BÝTÝÞ ---
    }

    public void CompleteCurrentMission()
    {
        var m = CurrentMission;
        if (m == null) return;

        Debug.Log("Görev tamamlandý: " + m.title);
        OnMissionCompleted?.Invoke(m);

        UnlockRewards(m);

        currentMissionIndex++;


        SaveProgress();
        NotifyMissionChanged();
    }

    // --- DÜZELTÝLEN KISIM 1: MergeAll Kullanýmý ---
    void SaveMissionToFirestore()
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null) return;

        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "currentMissionIndex", currentMissionIndex },
            { "lastUpdated", FieldValue.ServerTimestamp }
        };

        db.Collection("users")
          .Document(user.UserId)
          // SetOptions.Merge yerine SetOptions.MergeAll kullanýldý
          .SetAsync(data, SetOptions.MergeAll)
          .ContinueWithOnMainThread(task =>
          {
              if (task.IsFaulted)
                  Debug.LogError("Firestore Kayýt Hatasý: " + task.Exception);
              else
                  Debug.Log("Görev Firestore'a kaydedildi. Index: " + currentMissionIndex);
          });
    }

    // --- DÜZELTÝLEN KISIM 2: ToDictionary ve ContainsKey Kullanýmý ---
    void LoadMissionFromFirestore(FirebaseUser user)
    {
        if (user == null) return;

        db.Collection("users")
          .Document(user.UserId)
          .GetSnapshotAsync()
          .ContinueWithOnMainThread(task =>
          {
              if (task.IsFaulted)
              {
                  Debug.LogError("Firestore Okuma Hatasý: " + task.Exception);
                  return;
              }

              DocumentSnapshot snapshot = task.Result;

              // --- BURASI KRÝTÝK DEÐÝÞÝKLÝK ---
              if (snapshot != null && snapshot.Exists)
              {
                  // MEVCUT KULLANICI: Cloud verisini al
                  Dictionary<string, object> data = snapshot.ToDictionary();

                  if (data.ContainsKey("currentMissionIndex"))
                  {
                      int cloudIndex = System.Convert.ToInt32(data["currentMissionIndex"]);

                      // Cloud verisi yerel veriden farklýysa güncelle
                      // (Burada yerel veri cloud'dan büyük olsa bile Cloud esastýr diyoruz)
                      currentMissionIndex = cloudIndex;
                      Debug.Log("Firestore'dan görev yüklendi: " + currentMissionIndex);

                      NotifyMissionChanged();
                      PlayerPrefs.SetInt(SAVE_KEY, currentMissionIndex);

                      // AÇILMIÞ EÞYALARI DA CLOUD'DAN ÇEKMEK GEREKÝR (Ýleride buraya eklenebilir)
                      // Þimdilik sadece görev sýrasýný düzeltiyoruz.
                  }
              }
              else
              {
                  // YENÝ KULLANICI:
                  Debug.Log("Yeni kullanýcý tespit edildi. Önceki oturum kalýntýlarý temizleniyor...");

                  // 1. Görev sayacýný sýfýrla
                  currentMissionIndex = 0;
                  PlayerPrefs.SetInt(SAVE_KEY, 0);

                  // 2. Açýlmýþ kilitleri (Substances) sýfýrla!
                  // (Bunu yapmazsak yeni kullanýcý Sodyum harici þeyleri de açýk bulabilir)
                  UnlockSystem.ResetAllUnlocks();

                  // 3. UI güncelle
                  NotifyMissionChanged();

                  // 4. Þimdi tertemiz (0) veriyi Cloud'a yaz
                  SaveMissionToFirestore();
              }
          });
    }

    void UnlockRewards(MissionSO m)
    {
        foreach (var s in m.unlockSubstances) if (s != null) UnlockSystem.UnlockSubstance(s);
        foreach (var go in m.unlockTools) if (go != null) UnlockSystem.UnlockTool(go);
    }

    // --- Dýþ Tetikleyiciler ---
    public void NotifyReactionPerformed(ReactionSO reaction)
    {
        var m = CurrentMission;
        if (m != null && m.conditionType == MissionConditionType.PerformReaction && m.targetReaction == reaction)
            CompleteCurrentMission();
    }

    public void NotifySubstanceAdded(SubstanceSO substance)
    {
        var m = CurrentMission;
        if (m != null && m.conditionType == MissionConditionType.AddSubstanceToContainer && m.targetSubstance == substance)
            CompleteCurrentMission();
    }

    public void NotifyToolPickedUp(string toolName)
    {
        var m = CurrentMission;
        if (m != null && m.conditionType == MissionConditionType.PickUpTool && m.toolName == toolName)
            CompleteCurrentMission();
    }
}