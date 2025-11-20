using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    public static bool IsReady { get; private set; }
    public static FirebaseApp App { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                App = FirebaseApp.DefaultInstance;
                IsReady = true;
                Debug.Log("Firebase hazýr.");
            }
            else
            {
                Debug.LogError("Firebase baðýmlýlýk hatasý: " + status);
            }
        });
    }
}
